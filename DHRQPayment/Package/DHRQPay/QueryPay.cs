using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Package.DHRQPay
{
    class QueryPay : DHRQPaymentPay
    {
        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            //if (!string.IsNullOrEmpty(PayEntity.signBankCardNo))
            //{
            //    SendPackage.SetString(2, PayEntity.signBankCardNo);
            //}
            SendPackage.SetString(3, "354000");
            SendPackage.SetArrayData(4, Encoding.Default.GetBytes("0"));
            PayEntity.PayTraceNo = GetTraceNo();
            SendPackage.SetString(11, PayEntity.PayTraceNo);
            SendPackage.SetString(12, DateTime.Now.ToString("HHmmss"));
            SendPackage.SetString(13, DateTime.Now.ToString("yyyyMMdd"));
            SendPackage.SetArrayData(18, Encoding.Default.GetBytes("STA"));
            SendPackage.SetArrayData(41, Encoding.ASCII.GetBytes(GetMerchantNo()));
            SendPackage.SetArrayData(42, Encoding.ASCII.GetBytes(GetTerminalNo()));
            SendPackage.SetString(47, PayEntity.unitNo + PayEntity.cardinfo.cardNo.PadRight(42, ' '));

            if (string.Compare(PayEntity.cardinfo.cardType, "0") == 0)
            {
                //工业卡充钱
                PayEntity.buyNums = 0;
            }
            SendPackage.SetArrayData(48, Encoding.Default.GetBytes("ICDATA" + PayEntity.cardinfo.cardNo + "|" + PayEntity.cardinfo.icMark + "|" + PayEntity.cardinfo.icNum + "|" + GetTerminalNo() + "|" + PayEntity.cardinfo.strEnCrypt + "|" + PayEntity.buyNums.ToString()));
            SendPackage.SetString(49, "156");

        }

        protected override void OnSucc()
        {
            //IC卡圈存申请返回信息（ 缴费通返回："ICDATA" + IC卡圈存申请返回信息）：公司代码|客户姓名|客户地址|购气单价|本次最大购气量|帐户余额|折扣类型（折扣类型：1全额折扣；2比例折扣；3赠送气量）|折扣率|折扣金额|赠送气量|总金额

            try
            {
                byte[] field48 = RecvPackage.GetArrayData(48);
                string strField48 = Encoding.Default.GetString(field48);
                Log.Info("[QueryPay][OnSucc] field48:" + strField48);
                string[] strs = strField48.Split('|');
                if (strs.Length == 12)
                {
                    PayEntity.returnCardInfo.companyCode = strs[0];
                    PayEntity.returnCardInfo.userName = strs[1];
                    PayEntity.returnCardInfo.userAddr = strs[2];
                    PayEntity.returnCardInfo.unitPrice = strs[3];
                    PayEntity.returnCardInfo.maxNums = strs[4];
                    PayEntity.returnCardInfo.accountRemind = strs[5];
                    PayEntity.returnCardInfo.discountType = strs[6];
                    PayEntity.returnCardInfo.discount = strs[7];
                    PayEntity.returnCardInfo.discountAmount = strs[8];
                    PayEntity.returnCardInfo.presentQue = strs[9];
                    if (string.Compare(PayEntity.cardinfo.cardType, "1") == 0)
                    {
                        PayEntity.Amount = strs[10];
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("[QueryPay][OnSucc] err", ex);
            }

        }

        protected override bool UnPackFix()
        {
            PayEntity.returnMsg = RecvPackage.GetString(44);
            PayEntity.returnCode = RecvPackage.GetString(39);

            return base.UnPackFix();
        }

    }
}