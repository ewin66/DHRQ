using Landi.FrameWorks;
using Landi.FrameWorks.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DHRQPayment.Package.EMV.DHRQPaymentEMV
{
    class CDHRQDownNoPinNewBlackList : DHRQPaymentPay
    {
        private int m_process = 0;
        public bool DownBlackList()
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
            }
            catch (System.Exception ex)
            {
                result = false;
                Log.Error("[CPowerDownCA][DownPublishCA]Error", ex);
            }
            return result;
        }

        protected override void Packet()
        {
            switch (m_process)
            {
                case 0:
                    {
                        SendPackage.SetString(0, "0800");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "398");
                    }
                    break;
                case 1:
                    {
                        SendPackage.SetString(0, "0800");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "399");
                    }
                    break;
            }
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

                            GetBlackCardBin(bField62);
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

        private void GetBlackCardBin(byte[] bInfiled)
        {
            int pos = 0;
            int len = 0;
            try
            {
                if (bInfiled[0] == 0xFF && bInfiled[1] == 0x80 && bInfiled[2] == 0x5D)
                {
                    pos += 3;
                    len = bInfiled[pos] * 256 + bInfiled[pos + 1];
                    pos += 2;
                    while (pos < bInfiled.Length)
                    {
                        if (bInfiled.Length < pos + 8)
                        {
                            throw new Exception("get blcak card bin err");
                        }
                        else
                        {
                            byte[] tempbyte = new byte[8];
                            Array.Copy(bInfiled, pos, tempbyte, 0, 8);
                            string tempstr = Utility.bcd2str(tempbyte,tempbyte.Length);
                            Global.gNoPinAndSignParamData.Add2CardbinC(tempstr);
                            pos += 8;
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
