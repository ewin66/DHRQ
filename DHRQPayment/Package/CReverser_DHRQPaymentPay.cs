using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Package
{
    class CReverser_DHRQPaymentPay : DHRQPaymentPay
    {
        public string Reason;
        public CReverser_DHRQPaymentPay()
        {
            RestorePackageFromFile();
        }

        public CReverser_DHRQPaymentPay(PackageBase pb)
            : base(pb) 
        {
        }

        public void SetField55Value(byte[] field55, int len)
        {
            if (len != 0)
                SendPackage.SetArrayData(55, field55, 0, len);
        }

        public void CreateReverseFile(string reason)
        {
            if (string.IsNullOrEmpty(reason))
                reason = "06";
            Reason = reason;
            SavePackageToFile();
        }

        public void ClearReverseFile()
        {
            DeletePackageFile();
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            if (!string.IsNullOrEmpty(PayEntity.cardinfo.cardNo))
            {
                SendPackage.SetString(2, PayEntity.cardinfo.cardNo);
            }

            SendPackage.SetString(3, "399000");
            PayEntity.PayTraceNo = GetTraceNo();
            SendPackage.SetString(11, PayEntity.PayTraceNo);
            SendPackage.SetString(12, DateTime.Now.ToString("HHmmss"));
            SendPackage.SetString(13, DateTime.Now.ToString("yyyyMMdd"));
            SendPackage.SetArrayData(18, Encoding.Default.GetBytes("STA"));
            SendPackage.SetArrayData(41, Encoding.ASCII.GetBytes(GetMerchantNo()));
            SendPackage.SetArrayData(42, Encoding.ASCII.GetBytes(GetTerminalNo()));
            SendPackage.SetString(49, "159");

            PackReverse(Reason);
        }

        protected override void OnRecvFail()
        {
            SavePackageToFile();
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            DeletePackageFile();
        }

        protected override void OnSucc()
        {
            DeletePackageFile();
        }

        protected override void OnOtherResult()
        {
            SavePackageToFile();
        }

    }
}
