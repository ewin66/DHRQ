using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Package.DHRQPay
{
    class MsgDownload : DHRQPaymentPay
    {
        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            SendPackage.SetString(3, "350000");
            PayEntity.PayTraceNo = GetTraceNo();
            SendPackage.SetString(11, PayEntity.PayTraceNo);
            SendPackage.SetString(12, DateTime.Now.ToString("HHmmss"));
            SendPackage.SetString(13, DateTime.Now.ToString("yyyyMMdd"));
            SendPackage.SetArrayData(18, Encoding.Default.GetBytes("STA"));
            SendPackage.SetArrayData(41, Encoding.ASCII.GetBytes(GetMerchantNo()));
            SendPackage.SetArrayData(42, Encoding.ASCII.GetBytes(GetTerminalNo()));
            SendPackage.SetString(47, PayEntity.unitNo + DateTime.Now.ToString("yyyyMMddHHmm"));
        }

        protected override void OnSucc()
        {
            byte[] field60 = RecvPackage.GetArrayData(60);
            PayEntity.unitMsg = Encoding.Default.GetString(field60);
        }

        protected override bool UnPackFix()
        {
            PayEntity.returnMsg = RecvPackage.GetString(44);
            PayEntity.returnCode = RecvPackage.GetString(39);

            return base.UnPackFix();
        }

    }
}
