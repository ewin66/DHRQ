using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Package.EMV.DHRQPaymentEMV
{
    class CDHRQDownNoPinNewCardBin : DHRQPaymentPay
    {
        private int m_process = 0;

        public bool DownCardBin()
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
                        SendPackage.SetString(60, "00" + GetBatchNo() + "396");
                    }
                    break;
                case 1:
                    {
                        SendPackage.SetString(0, "0800");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "397");
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

                            GetNewCardBin(bField62);
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
                Log.Error("失败！");
                throw;
            }
        }

        private string GetString(byte[] bField, int pos, int lenth)
        {
            byte[] temp = new byte[lenth];
            Array.Copy(bField, pos, temp, 0, lenth);
            return Encoding.GetEncoding("gb2312").GetString(temp).TrimEnd('\0');
        }

        private void GetNewCardBin(byte[] bInfiled)
        {
            int pos = 0;
            int len = 0;
            try
            {
                if (bInfiled[0] == 0xFF && bInfiled[1] == 0x80 && bInfiled[2] == 0x5B)
                {
                    pos += 3;
                    len = bInfiled[pos] * 256 + bInfiled[pos + 1];
                    pos += 2;
                    while (pos < bInfiled.Length)
                    {
                        if (bInfiled.Length < pos + 8)
                        {
                            throw new Exception("get new card bin err");
                        }
                        else
                        {
                            byte[] tempbyte = new byte[8];
                            Array.Copy(bInfiled, pos, tempbyte, 0, 8);
                            string tempstr = Utility.bcd2str(tempbyte, tempbyte.Length);
                            Global.gNoPinAndSignParamData.Add2CardbinB(tempstr);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }


    }
}
