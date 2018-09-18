using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks.Iso8583;

namespace DHRQPayment.Package.EMV.DHRQPaymentEMV
{
    class CDHRQDownParam : DHRQPaymentPay
    {         

        //private int m_process = 0;

        #region 下载参数

        #endregion

        public CDHRQDownParam()
        {
        }

        public bool DownParam()
        {
            bool result = false;
            try
            {
                if (ParamDown() != TransResult.E_SUCC)
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

            System.Threading.Thread.Sleep(2000);//四川版每个交易之间要停顿2S

            eRet = Communicate();
            return eRet;
        }

        protected override void Packet()
        {
            SendPackage.SetString(0, "0800");
            SendPackage.SetString(60, "00" + GetBatchNo() + "360");
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
            SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //记录批次号
            byte[] bField62 = RecvPackage.GetArrayData(62);
            int pos = 0;
            int lenth = 0;
            try
            {
                while (true)
                {
                    if (pos >= bField62.Length)
                        break;
                    ParamConfigValidate(bField62, ref pos, ref lenth);
                    pos += lenth;
                }
            }
            catch
            {
                Log.Info("参数下载验证失败！pos:" + pos + ",lenth:" + bField62.Length);
                throw;
            }
            //pos = 0;
            //lenth = 0;
            //Global.ParamInfo = new ParamInfo();
            //try
            //{
            //    while (true)
            //    {
            //        if (pos >= bField62.Length)
            //            break;
            //        ParamConfigOperate(bField62, ref pos, ref lenth, Global.ParamInfo);
            //        pos += lenth;
            //    }
            //    //TransAccessFactory factory = new TransAccessFactory();
            //    //factory.InsertParam(info);
            //}
            //catch
            //{
            //    Log.Info("参数下载失败！pos:" + pos + ",lenth:" + bField62.Length);
            //    throw;
            //}
        }

        private string GetString(byte[] bField, int pos, int lenth)
        {
            byte[] temp = new byte[lenth];
            Array.Copy(bField, pos, temp, 0, lenth);
            return Encoding.GetEncoding("gb2312").GetString(temp).TrimEnd('\0');
        }

        private void ParamConfigOperate(byte[] bField, ref int pos, ref int lenth,ParamInfo info)
        {
            string index = GetString(bField, pos, 2);
            pos += 2;
            lenth += 2;
            switch (index)
            {
                case "29"://pos终端号
                    lenth = 8;
                    info.TerminalNo = GetString(bField, pos, lenth);
                    break;
                case "11":
                case "12":
                    lenth = 2;
                    break;
                case "13":
                    lenth = 1;
                    break;
                case "14":
                           lenth = 14;
                    info.Tel1 = GetString(bField, pos, lenth);
                    break;
                case "15":
                           lenth = 14;
                    info.Tel2 = GetString(bField, pos, lenth);
                    break;
                case "16":
                           lenth = 14;
                    info.Tel3 = GetString(bField, pos, lenth);
                    break;
                case "17":
                    lenth = 14;
                    break;
                case "18":
                    lenth = 1;
                    break;
                case "19":
                    lenth = 2;
                    break;
                case "20":
                case "21":
                    lenth = 1;
                    break;
                case "22":
                    lenth = 40;
                    info.MerchantName = GetString(bField, pos, lenth);
                    break;
                case "23":
                case "24":
                case "25":
                    lenth = 1;
                    break;
                case "26":
                    lenth = 8;
                    byte[] tempB=new byte[8];
                    Array.Copy(bField, pos, tempB, 0, 8);
                    info.SupportType = Utility.bcd2str(tempB, lenth);
                    break;
                case "27":
                    lenth = 40;
                    info.MerchantNameE = GetString(bField, pos, lenth);
                    break;
                case "28":
                    lenth = 15;
                    info.MerchantNo = GetString(bField, pos, lenth);
                    break;
                case "30":
                    lenth = 6;
                    break;
                case "31":
                    lenth = 6;
                    break;
                case "32":
                    lenth = 19;
                    info.BankCardNo1 = GetString(bField, pos, lenth).Trim();
                    break;
                case "33":
                    lenth = 19;
                    info.BankCardNo2 = GetString(bField, pos, lenth);
                    break;
                case "34":
                    lenth = 19;
                    info.BankCardNo3 = GetString(bField, pos, lenth);
                    break;
                case "35":
                    lenth = 2;
                    break;
                case "36":
                    lenth = 12;
                    break;
                case "37":
                    lenth = 12;
                    info.PayLimit = GetString(bField, pos, lenth);
                    break;
                case "38":
                    lenth = 12;
                    break;
                case "39":
                    lenth = 12;
                    info.TransLimit = GetString(bField, pos, lenth);
                    break;
                case "40":
                    lenth = 2;
                    break;
            }
        }

        //校验终端号是否合法
        private bool ParamConfigValidate(byte[] bField,ref int pos,ref int lenth)
        {
            string index = GetString(bField, pos, 2);
            pos += 2;
            lenth += 2;
            switch (index)
            {
                case "29":
                    lenth = 8;
                    byte[] terminal = new byte[8];
                    Array.Copy(bField, pos, terminal, 0, 8);
                    string tValue = Encoding.ASCII.GetString(terminal);
                    string terminalNo = GetTerminalNo();
                    if (terminalNo != tValue)
                    {
                        Log.Info("终端号和本地终端号不匹配。终端号：" + tValue);
                        throw new Exception("终端号和本地终端号不匹配。终端号：" + tValue);
                    }
                    break;
                case "11":
                case "12":
                    lenth = 2;
                    break;
                case "13":
                    lenth = 1;
                    break;
                case "14":
                case "15":
                case "16":
                case "17":
                    lenth = 14;
                    break;
                case "18":
                    lenth = 1;
                    break;
                case "19":
                    lenth = 2;
                    break;
                case "20":
                case "21":
                    lenth = 1;
                    break;
                case "22":
                    lenth = 40;
                    break;
                case "23":
                case "24":
                case "25":
                    lenth = 1;
                    break;
                case "26":
                    lenth = 8;
                    break;
                case "27":
                    lenth = 40;
                    break;
                case "28":
                    lenth = 15;
                    break;
                case "30":
                    lenth = 6;
                    break;
                case "31":
                    lenth = 6;
                    break;
                case "32":
                    lenth = 19;
                    break;
                case "33":
                    lenth = 19;
                    break;
                case "34":
                    lenth = 19;
                    break;
                case "35":
                    lenth = 2;
                    break;
                case "36":
                    lenth = 12;
                    break;
                case "37":
                    lenth = 12;
                    break;
                case "38":
                    lenth = 12;
                    break;
                case "39":
                    lenth = 12;
                    break;
                case "40":
                    lenth = 12;
                    break;
                default:
                    Log.Info("编码没定义。编码:" + index);
                    throw new Exception("编码没定义。编码:" + index);
            }
            return false;
        }
    }
}
