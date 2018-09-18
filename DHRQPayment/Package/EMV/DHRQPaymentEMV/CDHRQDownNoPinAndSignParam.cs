using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Package.EMV.DHRQPaymentEMV
{
    class CDHRQDownNoPinAndSignParam : DHRQPaymentPay
    {
        private int m_process = 0;

        public bool DownParam()
        {
            bool result = false;
            try
            {
                m_process = 0;
                if (ParamDown() != TransResult.E_SUCC)
                    return false;
                m_process = 1;
                if (ParamEnd() != TransResult.E_SUCC)
                    return false;
                result = true;
            }
            catch (System.Exception ex)
            {
                result = false;
                Log.Error("[CParamDown]Error", ex);
            }
            return result;
        }

        protected TransResult ParamDown()
        {
            TransResult eRet = TransResult.E_SUCC;
            eRet = Communicate();
            return eRet;
        }

        protected TransResult ParamEnd()
        {
            TransResult eRet = TransResult.E_SUCC;
            eRet = Communicate();
            return eRet;
        }

        protected override void Packet()
        {
            switch (m_process)
            {
                case 0:
                    {
                        SendPackage.SetString(0, "0800");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "394");
                    }
                    break;
                case 1:
                    {
                        SendPackage.SetString(0, "0800");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "395");
                    }
                    break;
            }
        }

        protected override bool NeedCalcMac()
        {
            return false;
        }

        protected override void OnBeforeTrans()
        {
            SendPackage.Clear();
            RecvPackage.Clear();
        }

        protected override void OnSucc()
        {
            try
            {
                switch (m_process)
                {
                    case 0:
                        {
                            SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //记录批次号

                            byte[] bField62 = RecvPackage.GetArrayData(62);

                            string strFile62 = Utility.bcd2str(bField62, bField62.Length);
                            SaveBCDFile("NoPinParam.txt", strFile62);
                            Global.gNoPinAndSignParamData.InstanseNoPinAndSignParam();
                        }
                        break;
                    case 1:
                        {
                            SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //记录批次号

                        }
                        break;
                }
            }
            catch
            {
                Log.Error("解包免密免签参数失败！");
                throw;
            }
        }

        private string GetString(byte[] bField, int pos, int lenth)
        {
            byte[] temp = new byte[lenth];
            Array.Copy(bField, pos, temp, 0, lenth);
            return Encoding.GetEncoding("gb2312").GetString(temp).TrimEnd('\0');
        }
    }
}
