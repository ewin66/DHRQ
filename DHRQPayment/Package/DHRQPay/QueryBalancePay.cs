using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Package.DHRQPay
{
    class QueryBalancePay : DHRQPaymentPay
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

            SendPackage.SetString(3, "351000");
            SendPackage.SetArrayData(4, Encoding.Default.GetBytes(Utility.AmountToString(CommonData.Amount.ToString())));

            PayEntity.PayTraceNo = GetTraceNo();
            SendPackage.SetString(11, PayEntity.PayTraceNo);
            SendPackage.SetString(12, DateTime.Now.ToString("HHmmss"));
            SendPackage.SetString(13, DateTime.Now.ToString("yyyyMMdd"));
            SendPackage.SetArrayData(18, Encoding.Default.GetBytes("STA"));

            if (bIsIC)
                SendPackage.SetArrayData(22, Utility.str2Bcd("051".PadRight(28, '0')));
            if (!string.IsNullOrEmpty(CommonData.BankCardSeqNum) && CommonData.BankCardSeqNum.Length != 0)//卡序列号
            {
                SendPackage.SetArrayData(23, Encoding.Default.GetBytes(CommonData.BankCardSeqNum));
            }
            //SendPackage.SetArrayData(25, Encoding.Default.GetBytes("91")); //服务点条件代码


            if (!string.IsNullOrEmpty(CommonData.Track2) && CommonData.Track2.Length != 0)
            {
                SendPackage.SetArrayData(35, Encoding.Default.GetBytes(CommonData.Track2.Replace('=', 'D')));
            }
            SendPackage.SetArrayData(41, Encoding.ASCII.GetBytes(GetMerchantNo()));
            SendPackage.SetArrayData(42, Encoding.ASCII.GetBytes(GetTerminalNo()));
            SendPackage.SetString(47, PayEntity.unitNo + PayEntity.cardinfo.cardNo.PadRight(42, ' '));
            SendPackage.SetArrayData(48, Encoding.Default.GetBytes(PayEntity.cardinfo.cardNo + "|" + PayEntity.cardinfo.icMark + "|" + PayEntity.cardinfo.icNum + "|" + GetTerminalNo() + "|" + "" + "|" + PayEntity.cardinfo.strEnCrypt));
            SendPackage.SetString(49, "159");
            SendPackage.SetArrayData(52, Utility.str2Bcd(CommonData.BankPassWord));
            if ((bIsIC) && PayEntity.SendField55 != null && PayEntity.SendField55.Length != 0)
            {
                SendPackage.SetString(55, Utility.bcd2str(PayEntity.SendField55, PayEntity.SendField55.Length));
            }

            //SendPackage.SetString(60, ("00" + GetBatchNo() + "000"));

        }

        protected override void OnSucc()
        {
            base.OnSucc();
        }

        protected override bool UnPackFix()
        {
            PayEntity.returnMsg = RecvPackage.GetString(44);
            PayEntity.returnCode = RecvPackage.GetString(39);

            return base.UnPackFix();
        }

    }

    
}
