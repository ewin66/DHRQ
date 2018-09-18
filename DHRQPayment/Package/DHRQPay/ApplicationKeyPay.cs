using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Package.DHRQPay
{
    class ApplicationKeyPay : DHRQPaymentPay
    {
        public ApplicationKeyPay()
        {}
        protected override void Packet()
        {
            try
            {
                SendPackage.SetString(0, "0100");
                SendPackage.SetString(3, "349000");
                SendPackage.SetString(11, GetTraceNo());
                SendPackage.SetString(12, DateTime.Now.ToString("HHmmss"));
                SendPackage.SetString(13, DateTime.Now.ToString("yyyyMMdd"));
                //SendPackage.SetString(18, "STA");
                SendPackage.SetArrayData(18, Encoding.ASCII.GetBytes("STA"));
                SendPackage.SetArrayData(41, Encoding.ASCII.GetBytes(GetMerchantNo()));
                SendPackage.SetArrayData(42, Encoding.ASCII.GetBytes(GetTerminalNo()));
                SendPackage.SetArrayData(64, Encoding.ASCII.GetBytes("00000000"));

            }
            catch (Exception ex)
            {
                Log.Error("[ApplicationKeyPay][Packet] err", ex);
            }
        }

        protected override void OnSucc()
        {
            DoSignInSucc();
            if (Result == TransResult.E_SUCC)
            {
                HasSignIn = true;
            }
            else
                HasSignIn = false;
        }

        protected override void OnBeforeTrans()
        {
            if (!RealEnv)
            {
                HasSignIn = true;

                //不进行交易的时候，将手动的存入密钥，以用于后来mac计算
                byte[] key = new byte[KeyLength];
                for (int i = 0; i < KeyLength; i++)
                {
                    key[i] = 0x01;
                }
                KeyManager.SetEnPinKey(SectionName, key);
                KeyManager.SetEnMacKey(SectionName, key);
                KeyManager.SetDePinKey(SectionName, key);
                KeyManager.SetDeMacKey(SectionName, key);
                KeyManager.SetEnTraKey(SectionName, key);
                KeyManager.SetDeTraKey(SectionName, key);
            }
            else
                HasSignIn = false;
        }
    }
}
