using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Package.DHRQPay
{
    class DetailQueryPay : DHRQPaymentPay
    {
        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            SendPackage.SetString(3, "381008");
            PayEntity.PayTraceNo = GetTraceNo();
            SendPackage.SetString(11, PayEntity.PayTraceNo);
            SendPackage.SetString(12, DateTime.Now.ToString("HHmmss"));
            SendPackage.SetString(13, DateTime.Now.ToString("yyyyMMdd"));
            SendPackage.SetArrayData(18, Encoding.Default.GetBytes("STA"));
            SendPackage.SetArrayData(41, Encoding.ASCII.GetBytes(GetMerchantNo()));
            SendPackage.SetArrayData(42, Encoding.ASCII.GetBytes(GetTerminalNo()));
            SendPackage.SetString(47, PayEntity.unitNo + PayEntity.cardinfo.cardNo.PadRight(20, ' '));
            SendPackage.SetArrayData(48, Encoding.Default.GetBytes(PayEntity.cardinfo.cardNo + "|" + PayEntity.cardinfo.icMark + "|" + PayEntity.cardinfo.icNum + "|" + GetTerminalNo() + "|"));
            SendPackage.SetString(49, "159");
        }

        protected override void OnSucc()
        {
            //IC卡圈存申请返回信息（ 缴费通返回："ICDATA" + IC卡圈存申请返回信息）：公司代码|客户姓名|客户地址|购气单价|本次最大购气量|帐户余额|折扣类型（折扣类型：1全额折扣；2比例折扣；3赠送气量）|折扣率|折扣金额|赠送气量|总金额

            try
            {

                byte[] field48 = RecvPackage.GetArrayData(48);
                string strField48 = Encoding.Default.GetString(field48);
                Log.Info("[DetailQueryPay][OnSucc] field48: " + strField48);
                string resmsg = "";
                if (Unpack48(strField48, ref resmsg,ref PayEntity.detailinfolist) != 0)
                {
                    throw new Exception(resmsg);
                }
            }
            catch (Exception ex)
            {
                Log.Error("[DetailQueryPay][OnSucc] err", ex);
            }

        }

        protected override bool UnPackFix()
        {
            PayEntity.returnMsg = RecvPackage.GetString(44);
            PayEntity.returnCode = RecvPackage.GetString(39);

            return base.UnPackFix();
        }

        private int Unpack48(string str48, ref string retmsg,ref List<DHRQPayment.Entity.DHRQPaymentEntity.DetailInfo> list)
        {
            Log.Info("str48 : " + str48);
            int ret = -1;
            if (str48.Length <= 0)
            {
                retmsg = "报数据长度为0";
                return ret;
            }

            int packNums;
            if (!int.TryParse(str48.Substring(0, 3), out packNums))
            {
                retmsg = "48域解包出错";
                return ret;
            }
            if ((str48.Length - 3) % 170 != 0)
            {
                Log.Info("if ((str48.Length - 3) % 170 != 0 || str48.Length / 48 != packNums)");
                Log.Debug("(str48.Length - 3) % 170 = " + (str48.Length - 3) % 170);
                retmsg = "48域解包出错";
                return ret;
            }
            packNums = (str48.Length - 3) / 170;
            if (packNums > 0)
            {
                int cardpos = 3;
                for (int i = 0; i < packNums; i++)
                {
                    DHRQPayment.Entity.DHRQPaymentEntity.DetailInfo d = new DHRQPayment.Entity.DHRQPaymentEntity.DetailInfo();
                    //  30	购气编号
                    d.TransCode = str48.Substring(cardpos, 30).Trim();                
                    cardpos += 30;
                    //  20	卡号                                                           
                    d.CardNo = str48.Substring(cardpos, 20).Trim();                   
                    cardpos += 20;                                             
                    //  2	卡备注                                              
                    d.CardRemark = str48.Substring(cardpos, 2).Trim();                
                    cardpos += 2;
                    //  2	发卡次数
                    d.IssuingCardTimes = str48.Substring(cardpos, 2).Trim();          
                    cardpos += 2;
                    //  2	业务类型                  
                    d.TransType = str48.Substring(cardpos, 2).Trim();                 
                    cardpos += 2;
                    //  8	购气日期
                    d.TransDate = str48.Substring(cardpos, 8).Trim();                 
                    cardpos += 8;
                    //  6	购气时间
                    d.TransTime = str48.Substring(cardpos, 6).Trim();
                    cardpos += 6;
                    //  10	购气总量
                    d.TransGasVolume = str48.Substring(cardpos, 10).Trim();
                    cardpos += 10;
                    //  10	实交金额
                    d.TransAmount = str48.Substring(cardpos, 10).Trim();
                    cardpos += 10;
                    //  10	一阶气量
                    d.GasVolume1 = str48.Substring(cardpos, 10).Trim();
                    cardpos += 10;
                    //  10	一阶单价
                    d.GasPrices1 = str48.Substring(cardpos, 10).Trim();
                    cardpos += 10;
                    //  10	二阶气量
                    d.GasVolume2 = str48.Substring(cardpos, 10).Trim();
                    cardpos += 10;
                    //  10	二阶单价
                    d.GasPrices2 = str48.Substring(cardpos, 10).Trim();
                    cardpos += 10;
                    //  10	三阶气量
                    d.GasVolume3 = str48.Substring(cardpos, 10).Trim();
                    cardpos += 10;
                    //  10	三阶单价
                    d.GasPrices3 = str48.Substring(cardpos, 10).Trim();
                    cardpos += 10;
                    //  10	四阶气量
                    d.GasVolume4 = str48.Substring(cardpos, 10).Trim();
                    cardpos += 10;
                    //  10	四阶气价
                    d.GasPrices4 = str48.Substring(cardpos, 10).Trim();
                    cardpos += 10;
                    list.Add(d);
                }
                ret = 0;
                retmsg = "解包成功";
            }
            return ret;
        }


    }
}
