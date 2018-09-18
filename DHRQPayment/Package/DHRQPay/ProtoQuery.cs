using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Package.DHRQPay
{
    class ProtoQuery : DHRQPaymentPay
    {
        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            if (!string.IsNullOrEmpty(PayEntity.cardinfo.cardNo))
            {
                SendPackage.SetString(2, PayEntity.cardinfo.cardNo);
            }
            SendPackage.SetString(3, "300644");
            PayEntity.PayTraceNo = GetTraceNo();
            SendPackage.SetString(11, PayEntity.PayTraceNo);
            SendPackage.SetString(12, DateTime.Now.ToString("HHmmss"));
            SendPackage.SetString(13, DateTime.Now.ToString("yyyyMMdd"));
            SendPackage.SetArrayData(18, Encoding.Default.GetBytes("STA"));
            SendPackage.SetArrayData(41, Encoding.ASCII.GetBytes(GetMerchantNo()));
            SendPackage.SetArrayData(42, Encoding.ASCII.GetBytes(GetTerminalNo()));
            SendPackage.SetString(47, PayEntity.unitNo + PayEntity.cardinfo.cardNo.PadRight(20, ' '));
            SendPackage.SetString(49, "156");

        }

        protected override void OnSucc()
        {
            //IC卡号|签约账号（为空表示未绑定银行卡）|
            byte[] field48 = RecvPackage.GetArrayData(48);
            string strField48 = Encoding.Default.GetString(field48);
            string[] strs = strField48.Split('|');
            if (strs.Length >= 2 && !string.IsNullOrEmpty(strs[1]) && ReturnCode == "0000")
            {
                PayEntity.isSign = true;
                PayEntity.signBankCardNo = strs[1];
            }
            else if (string.IsNullOrEmpty(strs[1]) && ReturnCode == "0740")
            {
                //未签约账户
                PayEntity.isSign = false;
            }

        }

        protected override bool UnPackFix()
        {
            PayEntity.returnMsg = RecvPackage.GetString(44);

            return base.UnPackFix();
        }

    }
}
