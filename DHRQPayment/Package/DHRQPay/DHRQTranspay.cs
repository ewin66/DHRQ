using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Package.DHRQPay
{
    class DHRQTranspay : DHRQPaymentPay
    {
        protected override void Packet()
        {
            bool bIsIC = false;

            if (CommonData.UserCardType == UserBankCardType.ICCard ||
    CommonData.UserCardType == UserBankCardType.IcMagCard)
                bIsIC = true;
            SendPackage.SetString(0, "0100");
            if (!string.IsNullOrEmpty(CommonData.BankCardNum) && CommonData.BankCardNum.Length != 0)
            {
                SendPackage.SetString(2, CommonData.BankCardNum);
            }

            SendPackage.SetString(3, "300645");
//#if DEBUG
//            SendPackage.SetArrayData(4, Encoding.Default.GetBytes(Utility.AmountToString("100.00")));
//#else
            SendPackage.SetArrayData(4, Encoding.Default.GetBytes(Utility.AmountToString(CommonData.Amount.ToString())));
//#endif

            PayEntity.PayTraceNo = GetTraceNo();
            SendPackage.SetString(11, PayEntity.PayTraceNo);
            SendPackage.SetString(12, DateTime.Now.ToString("HHmmss"));
            SendPackage.SetString(13, DateTime.Now.ToString("yyyyMMdd"));
            SendPackage.SetArrayData(18, Encoding.Default.GetBytes("STA"));

            if (!PayEntity.isSign)//22
            {
                if(bIsIC)
                    SendPackage.SetArrayData(22, Utility.str2Bcd("051".PadRight(28, '0')));
                //else
                //    SendPackage.SetArrayData(22, Encoding.Default.GetBytes("021".PadRight(28, ' ')));
                if (!string.IsNullOrEmpty(CommonData.BankCardSeqNum) && CommonData.BankCardSeqNum.Length != 0)//卡序列号
                {
                    SendPackage.SetArrayData(23, Encoding.Default.GetBytes(CommonData.BankCardSeqNum));
                }
                SendPackage.SetArrayData(25, Encoding.Default.GetBytes("91")); //服务点条件代码

                SendPackage.SetString(60, ("00" + GetBatchNo() + "000"));

            }

            if (!string.IsNullOrEmpty(CommonData.Track2) && CommonData.Track2.Length != 0)
            {
                SendPackage.SetArrayData(35, Encoding.Default.GetBytes(CommonData.Track2.Replace('=', 'D')));
            }
            //if (!string.IsNullOrEmpty(CommonData.Track3) && CommonData.Track3.Length != 0)
            //{
            //    SendPackage.SetString(36, CommonData.Track3.Replace('=', 'D'));
            //}
            SendPackage.SetArrayData(41, Encoding.ASCII.GetBytes(GetMerchantNo()));
            SendPackage.SetArrayData(42, Encoding.ASCII.GetBytes(GetTerminalNo()));
            SendPackage.SetString(47, PayEntity.unitNo + PayEntity.cardinfo.cardNo.PadRight(42, ' '));
            SendPackage.SetArrayData(48, Encoding.Default.GetBytes( PayEntity.cardinfo.cardNo + "|" + PayEntity.cardinfo.icMark + "|" + PayEntity.cardinfo.icNum + "|" + GetTerminalNo() + "|" + "" + "|" + PayEntity.cardinfo.strEnCrypt));
            SendPackage.SetString(49, "159");
            if (!PayEntity.isSign)
            {
                SendPackage.SetArrayData(52, Utility.str2Bcd(CommonData.BankPassWord));
                if ((bIsIC) && PayEntity.SendField55 != null && PayEntity.SendField55.Length != 0)
                {
                    SendPackage.SetString(55, Utility.bcd2str(PayEntity.SendField55, PayEntity.SendField55.Length));
                }
            }
        }

        protected override bool UnPackFix()
        {
            PayEntity.returnMsg = RecvPackage.GetString(44);
            PayEntity.returnCode = RecvPackage.GetString(39);

            return base.UnPackFix();
        }


        //protected override void Packet()
        //{
        //    //bool bIsIC = false;
        //    //bool bIsQpboc = false;
        //    //if (CommonData.UserCardType == UserBankCardType.ICCard ||
        //    //    CommonData.UserCardType == UserBankCardType.IcMagCard)
        //    //    bIsIC = true;
        //    //if (CommonData.UserCardType == UserBankCardType.Noconnect)
        //    //    bIsQpboc = true;

        //    try
        //    {
        //        SendPackage.SetString(0, "0200");
        //        //if (!string.IsNullOrEmpty(CommonData.BankCardNum) && CommonData.BankCardNum.Length != 0)
        //        {
        //            SendPackage.SetString(2, "6225758343029350");
        //        }
        //        SendPackage.SetString(3, "000000");
        //        SendPackage.SetString(4, "000000000001");
        //        PayEntity.PayTraceNo = GetTraceNo();
        //        SendPackage.SetString(11, PayEntity.PayTraceNo);
        //        //if (!string.IsNullOrEmpty(CommonData.BankCardExpDate) && CommonData.BankCardExpDate.Length != 0)//卡有效期
        //        {
        //            SendPackage.SetString(14, "2010");
        //        }
        //        //string field22 = "";
        //        //if (CommonData.UserCardType == UserBankCardType.ICCard ||
        //        //    CommonData.UserCardType == UserBankCardType.IcMagCard)//22
        //        //    field22 = "05";
        //        //else if (CommonData.UserCardType == UserBankCardType.Magcard)
        //        //    field22 = "02";
        //        //else if (CommonData.UserCardType == UserBankCardType.Noconnect)
        //        //    field22 = "07";
        //        //if (CommonData.IsNoPin)
        //        //    field22 += "2";
        //        //else
        //        //    field22 += "1";
        //        SendPackage.SetString(22, "071");
        //        //if (!string.IsNullOrEmpty(CommonData.BankCardSeqNum) && CommonData.BankCardSeqNum.Length != 0)//卡序列号
        //        {
        //            SendPackage.SetString(23, "001");
        //        }
        //        SendPackage.SetString(25, "00"); //服务点条件代码
        //        //if (!CommonData.IsNoPin)
        //        //    SendPackage.SetString(26, "06");
        //        //if (!string.IsNullOrEmpty(CommonData.Track2) && CommonData.Track2.Length != 0)
        //        {
        //            SendPackage.SetString(35, "6225758343029350D201020113092789");
        //        }
        //        //if (!string.IsNullOrEmpty(CommonData.Track3) && CommonData.Track3.Length != 0)
        //        //{
        //        //    SendPackage.SetString(36, CommonData.Track3.Replace('=', 'D'));
        //        //}
        //        //SendPackage.SetArrayData(48, PacketField48());
        //        //SendPackage.SetString(41, "00000001");
        //        //SendPackage.SetString(42, "105501375239015");

        //        SendPackage.SetString(49, "156");
        //        //if (!CommonData.IsNoPin)
        //        //{
        //        //    SendPackage.SetArrayData(52, Utility.str2Bcd(CommonData.BankPassWord));
        //        //}
        //        //switch (DType)
        //        //{
        //        //    case DesType.Des:
        //        //        if (CommonData.IsNoPin)
        //        //        {
        //        //            SendPackage.SetString(53, "0000000000000000");
        //        //        }
        //        //        else
        //        //        {
        //        //            SendPackage.SetString(53, "2000000000000000");
        //        //        }
        //        //        break;
        //        //    case DesType.TripleDes:
        //        //        if (CommonData.IsNoPin)
        //        //        {
        //        //            SendPackage.SetString(53, "0600000000000000");
        //        //        }
        //        //        else
        //        {
        //            SendPackage.SetString(53, "2600000000000000");
        //        }
        //        //        break;
        //        //}
        //        //55
        //        //if ((bIsIC || bIsQpboc) && PayEntity.SendField55 != null && PayEntity.SendField55.Length != 0)
        //        {
        //            string field55 = "9F2608116CCDC219F4A4CB9F2701809F101307010103A0B002010A010000020000F193895B9F37044DCFEA989F36020007950500800408009A031605139C01009F02060000000000015F2A02015682027C009F1A0201569F03060000000000009F3303E0E1C89F34030203009F3501229F1E0831323334353637388408A0000003330101029F0902008C9F4104000000009F6300";
        //            SendPackage.SetArrayData(55, Utility.str2Bcd(field55));
        //        }

        //        //if (PayEntity.UseICCard && bIsIC && CommonData.UserCardType != UserBankCardType.Noconnect)
        //        //    SendPackage.SetString(60, "22" + GetBatchNo() + "000500");
        //        //else if (CommonData.UserCardType == UserBankCardType.Noconnect)
        //        //    SendPackage.SetString(60, "22" + GetBatchNo() + "000601");
        //        //else
        //        SendPackage.SetString(60, "22000001000601");


        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //    }
        //    //创建冲正文件 98 96 06
        //    CReverser_DHRQPaymentPay cr = new CReverser_DHRQPaymentPay(this);
        //    cr.CreateReverseFile("98");
        //}

        protected override void OnSucc()
        {

            //脱机电子钱包钱包类型：01 有限余额 02 不限余额 11 有限次 12 不限次
            //IC卡申请信息（通过48域提交缴费通）：
            //IC卡号|备注信息|发卡次数|终端代码|发票号（20个0）|加密串|

            //IC卡圈存申请返回信息（ 缴费通返回："ICDATA" + IC卡圈存申请返回信息）：
            //客户姓名|客户地址|购气单价|本次购气量|上次余额|最新余额|加密串|折扣金额|赠送气量|



            //PayEntity.paytime = RecvPackage.GetString(12); //时间
            //PayEntity.paydate = RecvPackage.GetString(13); //日期

            //37域 系统参考号
            //PayEntity.PayReferenceNo = RecvPackage.GetString(37);
            ////38域
            //PayEntity.RecvField38 = RecvPackage.ExistValue(38) ? RecvPackage.GetString(38) : "";
            ////55域
            //PayEntity.RecvField55 = RecvPackage.ExistValue(55) ? RecvPackage.GetArrayData(55) : new byte[0];
            
        }
    }
}
