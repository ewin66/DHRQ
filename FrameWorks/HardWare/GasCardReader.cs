using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Landi.FrameWorks.HardWare
{
    class GasCardReader0
    {
        #region CardDll0.dll
        [DllImport("dll\\0\\DEMO\\CardDLL.dll", EntryPoint = "XinaoDeCrypt", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        protected static extern int XinaoDeCrypt(StringBuilder Source, StringBuilder Result);
        [DllImport("dll\\0\\DEMO\\CardDLL.dll", EntryPoint = "XinaoEnCrypt", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        protected static extern int XinaoEnCrypt(StringBuilder Source, StringBuilder Result);
        [DllImport("dll\\0\\DEMO\\CardDLL.dll", EntryPoint = "WriteICCard", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        protected static extern int WriteICCard(StringBuilder COMID, StringBuilder COMHZ, StringBuilder Password, StringBuilder GASCOUNT, StringBuilder ICErroy);
        [DllImport("dll\\0\\DEMO\\CardDLL.dll", EntryPoint = "ReadICCard", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        protected static extern int ReadICCard(StringBuilder COMID, StringBuilder COMHZ, StringBuilder ICId, StringBuilder GASCOUNT, StringBuilder ICCSpare, StringBuilder ICNum, StringBuilder ICMark, StringBuilder Password, StringBuilder ICErroy);
        #endregion
        /// <summary>
        /// 读卡
        /// </summary>
        /// <param name="COMID"></param>
        /// <param name="COMHZ"></param>
        /// <param name="ICId"></param>
        /// <param name="GASCOUNT"></param>
        /// <param name="ICCSpare"></param>
        /// <param name="ICNum"></param>
        /// <param name="ICMark"></param>
        /// <param name="Password"></param>
        /// <param name="ICErroy"></param>
        /// <returns></returns>
        public static int ReadCard(string COMID, string COMHZ, ref string ICId, ref string GASCOUNT, ref string ICCSpare, ref string ICNum, ref string ICMark, ref string Password, ref string ICErroy)
        {
            int ret = -1;
            try
            {
                StringBuilder newICId = new StringBuilder(10);
                StringBuilder newGasCount = new StringBuilder(5);
                StringBuilder newIccspare = new StringBuilder(5);
                StringBuilder newIcnum = new StringBuilder(10);
                StringBuilder newIcmark = new StringBuilder(10);
                StringBuilder newpass = new StringBuilder(300);
                StringBuilder newicerroy = new StringBuilder(50);
                if (!string.IsNullOrEmpty(COMID) && !string.IsNullOrEmpty(COMHZ))
                {
                    if (COMID.StartsWith("COM"))
                    {
                        COMID = COMID.Substring(3);
                    }
                    StringBuilder newCOMID = new StringBuilder((int.Parse(COMID) - 1).ToString());
                    StringBuilder newCOMHZ = new StringBuilder(COMHZ);
                    ret = ReadICCard(newCOMID, newCOMHZ, newICId, newGasCount, newIccspare, newIcnum, newIcmark, newpass, newicerroy);

                }
                if (ret == 0)
                {
                    ICId = newICId.ToString();
                    GASCOUNT = newGasCount.ToString();
                    ICCSpare = newIccspare.ToString();
                    ICNum = newIcnum.ToString();
                    ICMark = newIcmark.ToString();
                    Password = newpass.ToString();
                    ICErroy = newicerroy.ToString();
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[GasCardReader0][ReadCard]Error!\n" + e.ToString());
                return -1;
            }
        }
        
        /// <summary>
        /// 写卡
        /// </summary>
        /// <param name="COMID"></param>
        /// <param name="COMHZ"></param>
        /// <param name="Password"></param>
        /// <param name="GASCOUNT"></param>
        /// <param name="ICErroy"></param>
        /// <returns></returns>
        public static int WriteCard(string COMID, string COMHZ, string Password, string GASCOUNT, ref string ICErroy)
        {
            int ret = -1;
            try
            {
                StringBuilder newicerroy = new StringBuilder(50);

                if (!string.IsNullOrEmpty(COMID) && !string.IsNullOrEmpty(COMHZ) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(GASCOUNT))
                {
                    StringBuilder newCOMID = new StringBuilder((int.Parse(COMID) - 1).ToString());
                    StringBuilder newCOMHZ = new StringBuilder(COMHZ);
                    StringBuilder newpass = new StringBuilder(Password);
                    StringBuilder newGasCount = new StringBuilder(GASCOUNT);
                    ret = WriteICCard(newCOMID, newCOMHZ, newpass, newGasCount, newicerroy);
                }
                if (ret == 0)
                {
                    ICErroy = newicerroy.ToString();
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[GasCardReader0][WriteCard]Error!\n" + e.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public static int EnCrypt(string Source, ref string Result)
        {
            int ret = -1;
            try
            {
                StringBuilder newResult = new StringBuilder(300);
                if (!string.IsNullOrEmpty(Source))
                {
                    StringBuilder newSource = new StringBuilder(Source);
                    ret = XinaoEnCrypt(newSource, newResult);
                }
                if (ret == 0)
                {
                    Result = newResult.ToString();
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[GasCardReader0][EnCrypt]Error!\n" + e.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public static int DeCrypt(string Source, ref string Result)
        {
            int ret = -1;
            try
            {
                StringBuilder newResult = new StringBuilder(300);
                if (!string.IsNullOrEmpty(Source))
                {
                    StringBuilder newSource = new StringBuilder(Source);
                    ret = XinaoDeCrypt(newSource, newResult);
                }
                if (ret == 0)
                {
                    Result = newResult.ToString();
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[GasCardReader0][DeCrypt]Error!\n" + e.ToString());
                return -1;
            }
        }
    }

    class GasCardReader1
    {
        #region CardDll1.dll
        [DllImport("dll\\1\\DEMO\\CardDLL.dll", EntryPoint = "XinaoDeCrypt", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        protected static extern int XinaoDeCrypt(StringBuilder Source, StringBuilder Result);
        [DllImport("dll\\1\\DEMO\\CardDLL.dll", EntryPoint = "XinaoEnCrypt", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        protected static extern int XinaoEnCrypt(StringBuilder Source, StringBuilder Result);
        [DllImport("dll\\1\\DEMO\\CardDLL.dll", EntryPoint = "WriteICCard", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        protected static extern int WriteICCard(StringBuilder COMID, StringBuilder COMHZ, StringBuilder Password, StringBuilder GASCOUNT, StringBuilder ICErroy);
        [DllImport("dll\\1\\DEMO\\CardDLL.dll", EntryPoint = "ReadICCard", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        protected static extern int ReadICCard(StringBuilder COMID, StringBuilder COMHZ, StringBuilder ICId, StringBuilder GASCOUNT, StringBuilder ICCSpare, StringBuilder ICNum, StringBuilder ICMark, StringBuilder Password, StringBuilder ICErroy);
        #endregion


        /// <summary>
        /// 读卡
        /// </summary>
        /// <param name="COMID"></param>
        /// <param name="COMHZ"></param>
        /// <param name="ICId"></param>
        /// <param name="GASCOUNT"></param>
        /// <param name="ICCSpare"></param>
        /// <param name="ICNum"></param>
        /// <param name="ICMark"></param>
        /// <param name="Password"></param>
        /// <param name="ICErroy"></param>
        /// <returns></returns>
        public static int ReadCard(string COMID, string COMHZ, ref string ICId, ref string GASCOUNT, ref string ICCSpare, ref string ICNum, ref string ICMark, ref string Password, ref string ICErroy)
        {
            int ret = -1;
            try
            {
                //COMID = "3";
                //StringBuilder comid = new StringBuilder((int.Parse(COMID) - 1).ToString());
                //StringBuilder comhz = new StringBuilder("9600");
                //StringBuilder newICId = new StringBuilder(300);

                //StringBuilder newGasCount = new StringBuilder(300);
                //StringBuilder newIccspare = new StringBuilder(300);
                //StringBuilder newIcnum = new StringBuilder(300);
                //StringBuilder newIcmark = new StringBuilder(300);
                //StringBuilder newpass = new StringBuilder(300);
                //StringBuilder newicerroy = new StringBuilder(300);

                //ret = ReadICCard(comid, comhz, newICId, newGasCount, newIccspare, newIcnum, newIcmark, newpass, newicerroy);




                StringBuilder newICId = new StringBuilder(300);
                StringBuilder newGasCount = new StringBuilder(300);
                StringBuilder newIccspare = new StringBuilder(300);
                StringBuilder newIcnum = new StringBuilder(300);
                StringBuilder newIcmark = new StringBuilder(300);
                StringBuilder newpass = new StringBuilder(300);
                StringBuilder newicerroy = new StringBuilder(300);
                if (!string.IsNullOrEmpty(COMID) && !string.IsNullOrEmpty(COMHZ))
                {
                    if (COMID.StartsWith("COM"))
                    {
                        COMID = COMID.Substring(3);
                    }
                    StringBuilder newCOMID = new StringBuilder((int.Parse(COMID) - 1).ToString());
                    StringBuilder newCOMHZ = new StringBuilder(COMHZ);
                    ret = ReadICCard(newCOMID, newCOMHZ, newICId, newGasCount, newIccspare, newIcnum, newIcmark, newpass, newicerroy);

                }
                if (ret == 0)
                {
                    ICId = newICId.ToString();
                    GASCOUNT = newGasCount.ToString();
                    ICCSpare = newIccspare.ToString();
                    ICNum = newIcnum.ToString();
                    ICMark = newIcmark.ToString();
                    Password = newpass.ToString();
                    ICErroy = newicerroy.ToString();
                }

                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[GasCardReader1][ReadCard]Error!\n" + e.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 写卡
        /// </summary>
        /// <param name="COMID"></param>
        /// <param name="COMHZ"></param>
        /// <param name="Password"></param>
        /// <param name="GASCOUNT"></param>
        /// <param name="ICErroy"></param>
        /// <returns></returns>
        public static int WriteCard(string COMID, string COMHZ, string Password, string GASCOUNT, ref string ICErroy)
        {
            int ret = -1;
            try
            {
                StringBuilder newicerroy = new StringBuilder(50);

                if (!string.IsNullOrEmpty(COMID) && !string.IsNullOrEmpty(COMHZ) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(GASCOUNT))
                {
                    if (COMID.StartsWith("COM"))
                    {
                        COMID = COMID.Substring(3);
                    }

                    StringBuilder newCOMID = new StringBuilder((int.Parse(COMID) - 1).ToString());
                    StringBuilder newCOMHZ = new StringBuilder(COMHZ);
                    StringBuilder newpass = new StringBuilder(Password);
                    StringBuilder newGasCount = new StringBuilder(GASCOUNT);
                    ret = WriteICCard(newCOMID, newCOMHZ, newpass, newGasCount, newicerroy);
                }
                if (ret == 0)
                {
                    ICErroy = newicerroy.ToString();
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[GasCardReader1][WriteCard]Error!\n" + e.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public static int EnCrypt(string Source,ref string Result)
        {
            int ret = -1;
            try
            {
                StringBuilder newResult = new StringBuilder(300);
                if (!string.IsNullOrEmpty(Source))
                {
                    StringBuilder newSource = new StringBuilder(Source);
                    ret = XinaoEnCrypt(newSource, newResult);
                }
                if (ret == 0)
                {
                    Result = newResult.ToString();
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[GasCardReader1][EnCrypt]Error!\n" + e.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Result"></param>
        /// <returns></returns>
        public static int DeCrypt(string Source,ref string Result)
        {
            int ret = -1;
            try
            {
                StringBuilder newResult = new StringBuilder(300);
                if (!string.IsNullOrEmpty(Source))
                {
                    StringBuilder newSource = new StringBuilder(Source);
                    ret = XinaoDeCrypt(newSource, newResult);
                }
                if (ret == 64)
                {
                    Result = newResult.ToString();
                }
                return ret;
            }
            catch (System.Exception e)
            {
                Log.Error("[GasCardReader1][DeCrypt]Error!\n" + e.ToString());
                return -1;
            }
        }
    }

    class IcSmartCard
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        static extern int LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
        static extern IntPtr GetProcAddress(int hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
        static extern bool FreeLibrary(int hModule);

        [DllImport("ZJWXGas.dll", EntryPoint = "ReadICCard", CallingConvention = CallingConvention.Cdecl)]
        static extern int ReadICCard(int port, int comHz, byte[] sCardId, ref int iGasCount, ref int iICCSpare, ref int iIcNum, ref int iIcMark, byte[] sPassWord, ref int iIcErroy, byte[] sErrMsg);

        [DllImport(@"Xinao_Decrypt.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void XinaoEnCrypt(StringBuilder source, StringBuilder destination);

        [DllImport(@"Xinao_Decrypt.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern void XinaoDeCrypt(StringBuilder source, StringBuilder destination);


        /// <summary>
        /// 读卡事件
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="comHz">波特率</param>
        /// <param name="sCardId">卡号</param>
        /// <param name="iGasCount">购气量</param>
        /// <param name="iICCSpare"></param>
        /// <param name="iIcNum"></param>
        /// <param name="iIcMark"></param>
        /// <param name="sPassWord">卡密码</param>
        /// <param name="iIcErroy">错误号</param>
        /// <param name="sErrMsg">错误信息</param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int MyReadIcCardEventHandler(int port, int comHz, byte[] sCardId, ref int iGasCount, ref int iICCSpare, ref int iIcNum, ref int iIcMark, byte[] sPassWord, ref int iIcErroy, byte[] sErrMsg);

        /// <summary>
        /// 解灰事件
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="comHx">波特率</param>
        /// <param name="iIcErroy">错误号</param>
        /// <param name="sErrMsg">错误信息</param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int MyGrayIcCardEventHandler(int port, int comHx, ref int iIcErroy, byte[] sErrMsg);

        /// <summary>
        /// 写卡事件
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="comHx">波特率</param>
        /// <param name="sPassWord">卡密码</param>
        /// <param name="iGasCount">购气量</param>
        /// <param name="iIcErroy">错误号</param>
        /// <param name="sErrMsg">错误信息</param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int MyWriteICCardEventHandler(int port, int comHx, byte[] sPassWord, ref int iGasCount, ref int iIcErroy, byte[] sErrMsg);

        /// <summary>
        /// 读卡
        /// </summary>
        /// <param name="path">动态库路径</param>
        /// <param name="port">端口号</param>
        /// <param name="comHz">波特率</param>
        /// <param name="sCardId">卡号</param>
        /// <param name="iGasCount">购气次数</param>
        /// <param name="iICCSpare">剩余气量</param>
        /// <param name="iIcNum">发卡次数</param>
        /// <param name="iIcMark">备注信息</param>
        /// <param name="sPassWord">密文</param>
        /// <param name="iIcErroy">读卡结果</param>
        /// <param name="sErrMsg">读卡结果描述</param>
        /// <returns></returns>
        public static int ReadCard(string path, int port, int comHz, byte[] sCardId, ref int iGasCount, ref int iICCSpare, ref int iIcNum, ref int iIcMark, byte[] sPassWord, ref int iIcErroy, ref string sErrMsg)
        {
            int hModule = LoadLibrary(path);
            if (hModule == 0)
            {
                return -2;//文件没找到
            }
            int ret = -1;
            try
            {
                IntPtr intPtr = GetProcAddress(hModule, "ReadICCard");
                MyReadIcCardEventHandler myReadIcCard = (MyReadIcCardEventHandler)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(MyReadIcCardEventHandler));
                byte[] bErrMsg = new byte[1024];
                ret = myReadIcCard(port, comHz, sCardId, ref iGasCount, ref iICCSpare, ref iIcNum, ref iIcMark, sPassWord, ref iIcErroy, bErrMsg);

                //ret = ReadICCard(port, comHz, sCardId, ref iGasCount, ref iICCSpare, ref iIcNum, ref iIcMark, sPassWord, ref iIcErroy, bErrMsg);
                ret = iIcErroy;
                string cardID = System.Text.Encoding.Default.GetString(sCardId).Replace("\0", "");
                string passWord = System.Text.Encoding.Default.GetString(sPassWord).Replace("\0", "");
                sErrMsg = System.Text.Encoding.Default.GetString(bErrMsg).Replace("\0", "");
            }
            catch (Exception e11)
            {
                string mes = e11.Message.ToString();
                sErrMsg = mes;
                ret = -3;
            }
            finally
            {
                FreeLibrary(hModule);
            }
            return ret;
        }

        /// <summary>
        /// 灰卡处理
        /// </summary>
        /// <param name="path">动态库路径</param>
        /// <param name="port">端口号</param>
        /// <param name="comHz">波特率</param>
        /// <param name="iIcErroy">处理结果</param>
        /// <param name="sErrMsg">处理结果描述</param>
        /// <returns></returns>
        public static int GrayCard(string path, int port, int comHz, ref int iIcErroy, ref string sErrMsg)
        {
            int ret = -1;
            int hModule = LoadLibrary(path);
            if (hModule == 0)
            {
                return -2;//文件没找到
            }
            try
            {
                IntPtr intPtr = GetProcAddress(hModule, "GrayICCard");
                MyGrayIcCardEventHandler myGrayIcCard = (MyGrayIcCardEventHandler)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(MyGrayIcCardEventHandler));
                byte[] bErrMsg = new byte[1024];
                ret = myGrayIcCard(port, comHz, ref iIcErroy, bErrMsg);//调用动态库方法
                ret = iIcErroy;
                sErrMsg = System.Text.Encoding.Default.GetString(bErrMsg).Replace("\0", "");
            }
            catch (Exception e11)
            {
                string mes = e11.Message.ToString();
            }
            finally
            {
                FreeLibrary(hModule);
            }
            return ret;
        }

        /// <summary>
        /// 写卡处理
        /// </summary>
        /// <param name="path">动态库路径</param>
        /// <param name="port">端口号</param>
        /// <param name="comHz">波特率</param>
        /// <param name="sPassWord">秘钥</param>
        /// <param name="iGasCount">购气次数</param>
        /// <param name="iIcErroy">写卡结果</param>
        /// <param name="sErrMsg">写卡结果描述</param>
        /// <returns></returns>
        public static int WriteCard(string path, int port, int comHz, byte[] sPassWord, ref int iGasCount, ref int iIcErroy, ref string sErrMsg)
        {
            int ret = -1;
            int hModule = LoadLibrary(path);
            if (hModule == 0)
            {
                return -2;//文件没找到
            }
            try
            {
                IntPtr intPtr = GetProcAddress(hModule, "WriteICCard");
                MyWriteICCardEventHandler myWriteIcCard = (MyWriteICCardEventHandler)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(MyWriteICCardEventHandler));
                byte[] bErrMsg = new byte[1024];
                ret = myWriteIcCard(port, comHz, sPassWord, ref iGasCount, ref iIcErroy, bErrMsg);//调用动态库方法
                ret = iIcErroy;
                sErrMsg = System.Text.Encoding.Default.GetString(bErrMsg).Replace("\0", "");
            }
            catch (Exception e11)
            {
                string mes = e11.Message.ToString();
                if (sErrMsg == "")
                {
                    sErrMsg = mes;
                }
                return -3;//写卡异常
            }
            finally
            {
                FreeLibrary(hModule);
            }
            return ret;
        }
    }



    public class GasCardReader : HardwareBase<GasCardReader, GasCardReader.Status>, Landi.FrameWorks.IManagedHardware
    {

        /// <summary>
        /// 外接读卡函数
        /// </summary>
        /// <param name="cardinfo"></param>
        /// <returns></returns>
        public static int ReadGasCard(ref cardInfo cardinfo)
        {
            //if (!IsUse) return 0;
            if(!IsUse)
            {
                int resDebug = 0;
                cardinfo.cardNo = "2063838";
                cardinfo.cardType = "1";
                cardinfo.gasCount = "1";
                cardinfo.iccSpare = "0";
                cardinfo.icErroy = "";
                cardinfo.icNum = "1";

                return 0;
            }

            cardInfo cardinfoTemp = new cardInfo();
            string enCrypt = "";
            int res = 0;
            try
            {
                //苍南表读卡
                res = GasCardReader1.ReadCard(Port, Bps.ToString(), ref cardinfoTemp.cardNo, ref cardinfoTemp.gasCount, ref cardinfoTemp.iccSpare, ref cardinfoTemp.icNum, ref cardinfoTemp.icMark, ref cardinfoTemp.strEnCrypt, ref cardinfoTemp.icErroy);
                if (res == 6)
                {
                    //天信表读卡
                    int port = -1;
                    if (Port.StartsWith("COM"))
                    {
                        port = int.Parse(Port.Substring(3)) - 1;
                    }
                    byte[] sCardId = new byte[200];
                    int iGasCount = 0;
                    int iICCSpare = 0;
                    int iIcNum = 0;
                    int iIcMark = 0;
                    byte[] sPassWord = new byte[200];
                    int iIcErroy = -1;
                    string sErrMsg = null;

                    res = IcSmartCard.ReadCard("Tancy_IC.dll", port, Bps, sCardId, ref iGasCount, ref iICCSpare, ref iIcNum, ref iIcMark, sPassWord, ref iIcErroy, ref sErrMsg);
                    res = iIcErroy;
                    //cardinfoTemp.icErroy = sErrMsg;

                    string cardID = System.Text.Encoding.Default.GetString(sCardId).Replace("\0", "");
                    string passWord = System.Text.Encoding.Default.GetString(sPassWord).Replace("\0", "");
                    if (sErrMsg != null)
                        sErrMsg = (sErrMsg).Replace("\0", "");
                    cardinfoTemp.icErroy = sErrMsg;
                    if (iIcErroy != 0)
                    {
                        return res;
                    }
                    cardinfoTemp.cardNo = string.IsNullOrEmpty(cardID) ? "" : cardID;
                    cardinfoTemp.gasCount = string.IsNullOrEmpty(iGasCount.ToString()) ? "" : iGasCount.ToString();
                    cardinfoTemp.iccSpare = string.IsNullOrEmpty(iICCSpare.ToString()) ? "" : iICCSpare.ToString();
                    cardinfoTemp.icNum = string.IsNullOrEmpty(iIcNum.ToString()) ? "" : iIcNum.ToString();
                    cardinfoTemp.icMark = string.IsNullOrEmpty(iIcMark.ToString()) ? "" : iIcMark.ToString();
                    if (!string.IsNullOrEmpty(passWord.ToString()))
                    {
                        cardinfoTemp.strEnCrypt = passWord.ToString();
                        StringBuilder temp = new StringBuilder(65);
                        StringBuilder pass = new StringBuilder(cardinfoTemp.strEnCrypt);
                        IcSmartCard.XinaoDeCrypt(pass, temp);
                        cardinfoTemp.strDeCrypt = temp.ToString();
                    }
                    else
                    {
                        cardinfoTemp.strEnCrypt = "";
                        cardinfoTemp.strDeCrypt = "";
                    }
                    //res = GasCardReader0.ReadCard(Port, Bps.ToString(), ref cardinfoTemp.cardNo, ref cardinfoTemp.gasCount, ref cardinfoTemp.iccSpare, ref cardinfoTemp.icNum, ref cardinfoTemp.icMark, ref cardinfoTemp.strEnCrypt, ref cardinfoTemp.icErroy);

                    cardinfoTemp.cardType = "0";
                    //res = GasCardReader1.DeCrypt(cardinfoTemp.strEnCrypt, ref cardinfoTemp.strDeCrypt);
                    //if (res != 64)
                    //    throw new Exception("GasCardReader0.DeCrypt err");
                    cardinfo = cardinfoTemp;
                    //res = 0;
                    return res;
                }
                else if (res == 0)
                {
                    cardinfoTemp.cardType = "1";
                    res = GasCardReader1.DeCrypt(cardinfoTemp.strEnCrypt, ref cardinfoTemp.strDeCrypt);
                    if (res != 64)
                        throw new Exception("GasCardReader1.DeCrypt err");
                    //Log.Info("read De before:" + cardinfoTemp.strDeCrypt);

                    //Log.Info("read En before:" + cardinfoTemp.strEnCrypt);

                    cardinfoTemp.strDeCrypt = cardinfoTemp.strDeCrypt.Substring(0, 64);
                    cardinfoTemp.strEnCrypt = cardinfoTemp.strEnCrypt.Substring(0, 128);
                    Log.Info("read De:" + cardinfoTemp.strDeCrypt);
                    Log.Info("read En:" + cardinfoTemp.strEnCrypt);

                    cardinfo = cardinfoTemp;
                    res = 0;
                    return res;
                }
                else
                {
                    return res;
                }
            }
            catch (Exception ex)
            {
                Log.Error("[GasCardReaderHelper][ReadGasCard]Error!\n" + ex.ToString());
                return -1;
            }
        }

        ///// <summary>
        ///// 外接写卡函数
        ///// </summary>
        ///// <param name="cardinfo"></param>
        ///// <param name="gasGardType"></param>
        ///// <returns></returns>
        public static int WriteCard(cardInfo cardinfo, int gasGardType)
        {
            if (!IsUse) return 0;

            cardInfo cardinfoTemp = new cardInfo();
            string outError = "";
            int res = 0;
            try
            {
                if (gasGardType == 0)
                {
                    //天信
                    StringBuilder source = new StringBuilder(cardinfo.strDeCrypt);
                    StringBuilder destination = new StringBuilder(65);
                    IcSmartCard.XinaoEnCrypt(source, destination);
                    string path = "Tancy_IC.dll";
                    int port = -1;
                    if (Port.StartsWith("COM"))
                    {
                        port = int.Parse(Port.Substring(3)) - 1;
                    }
                    //int comHz = int.Parse(text_baud.Text);
                    byte[] sPassWord = Encoding.ASCII.GetBytes(destination.ToString());
                    int iGasCount = int.Parse(cardinfo.gasCount);
                    int iIcErroy = -1;
                    string sErrMsg = null;
                    IcSmartCard.WriteCard(path, port, Bps, sPassWord, ref iGasCount, ref iIcErroy, ref sErrMsg);
                    res = iIcErroy;
                    //res = GasCardReader0.EnCrypt(cardinfo.strDeCrypt, ref cardinfoTemp.strEnCrypt);
                    if (res == 0)
                    {
                        //res = IcSmartCard.WriteCard(Port.ToString(), Bps.ToString(), cardinfoTemp.strEnCrypt, cardinfo.gasCount + 1, ref outError);
                        return res;
                    }
                    else
                        throw new Exception("天信写卡错误:" + sErrMsg);
                }
                else if (gasGardType == 1)
                {
                    //苍南
                    res = GasCardReader1.EnCrypt(cardinfo.strDeCrypt, ref cardinfoTemp.strEnCrypt);
                    Log.Info("write enCrypt: " + cardinfoTemp.strEnCrypt);

                    if (res == 0)
                    {
                        res = GasCardReader1.WriteCard(Port.ToString(), Bps.ToString(), cardinfoTemp.strEnCrypt, cardinfo.gasCount, ref outError);
                        return res;
                    }
                    else
                        throw new Exception("GasCardReader1.EnCrypt err");
                }
                else
                {
                    throw new Exception("cardType Err");
                }
                //else
                //{
                //    res = GasCardReader1.EnCrypt(cardinfo.strDeCrypt, ref cardinfoTemp.strEnCrypt);
                //    if (res == 1)
                //    {
                //        res = GasCardReader0.EnCrypt(cardinfo.strDeCrypt, ref cardinfoTemp.strEnCrypt);
                //        if (res != 0)
                //            throw new Exception("GasCardReader0.EnCrypt err");
                //        res = GasCardReader0.WriteCard(Port.ToString(), Bps.ToString(), cardinfoTemp.strEnCrypt, cardinfo.gasCount + 1,ref outError);
                //        return res;
                //    }
                //    else if (res == 0)
                //    {
                //        res = GasCardReader1.WriteCard(Port.ToString(), Bps.ToString(), cardinfoTemp.strEnCrypt, cardinfo.gasCount + 1,ref outError);
                //        return res;
                //    }
                //    else
                //    {
                //        throw new Exception("GasCardReader1.EnCrypt err");
                //    }
                //}
            }
            catch (Exception ex)
            {
                Log.Error("[GasCardReaderHelper][WriteCard]Error!\n" + ex.ToString());
                return -1;
            }
        }

        public object Open()
        {
            return Status.CARD_SUCC;
        }

        public object Close()
        {
            return Status.CARD_SUCC;
        }

        public object CheckStatus()
        {
            return Status.CARD_SUCC;
        }

        public bool MeansError(object status)
        {
            return false;
        }

        public enum Status
        {
            CARD_SUCC = 0,        // 正确执行
            CARD_FAIL = 1,        // 通讯错误
            CARD_ERR_INS = 2,     // 命令处理错误,卡座返回 'N'
            CARD_WAIT = 3,        // 等待插卡
            CARD_ERR_PARAM = 4,   // 错误参数
            CARD_NOT_POWERUP = 5, // 卡未上电
            CARD_NORESPONE = 6,   // 返回的resp长度不和要求
            CARD_ERR = 7,           // 卡处理错误
        }

    }

    public class cardInfo
    {
        public string cardNo;     //卡号
        public string gasCount;   //购气次数
        public string iccSpare;   //卡内余量
        public string icNum;      //发卡次数
        public string icMark;     //备注信息
        public string icErroy;    //错误信息
        public string strDeCrypt; //解密后明文信息
        public string strEnCrypt; //加密数据
        public string cardType;   //燃气卡类型 0--cpu 工业卡(天信 充值)  1--逻辑加密卡 民用（苍南 购气） 2--商用卡
    }

}
