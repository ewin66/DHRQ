using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Landi.FrameWorks.HardWare
{
    public class EC_PBOC
    {
        #region ec_pboc.dll

        [DllImport("ec_pboc.dll")]
        protected static extern int EMVL2APPInit(int CardReaderType, long hand);

        [DllImport("ec_pboc.dll")]
        protected static extern int EMVL2StartEmvApp(int aidNo,int cProtocol, int inTransType, byte[] inTrace, byte[] inDay, byte[] inTime, byte[] inAmount, byte[] inOtherAmount);

        [DllImport("ec_pboc.dll")]
        protected static extern int EMVL2TermRiskManageProcessRestrict();

        [DllImport("ec_pboc.dll")]
        protected static extern int EMVL2CardHolderValidate(ref int cTimes);

        [DllImport("ec_pboc.dll")]
        protected static extern int EMVL2ContinueCardHolderValidate(int type, ref int cTimes);

        [DllImport("ec_pboc.dll")]
        protected static extern int EMVL2SelectApp(int cProtocol,byte[] appList, ref int nListNum);

        [DllImport("ec_pboc.dll")]
        protected static extern int EMVActionAnalysis();

        [DllImport("ec_pboc.dll")]
        protected static extern int EMVL2OnlineDataProcess(int retResult, byte[] buf55, int buf55Len, byte[] auth_id, int authLen);

        [DllImport("ec_pboc.dll")]
        private static extern void EMVGetDate(byte[] ptag, int tagLen, byte[] res, ref int retLen);


        [DllImport("ec_pboc.dll")]
        protected static extern void EMVL2GetCardNo(byte[] cardNo, ref int lenCard, byte[] cardTrack2, ref int track2Len, byte[] expData, ref int expLen, byte[] cardSeqNum);        

        [DllImport("ec_pboc.dll")]
        protected static extern void EMVSetCardReaderType(int CardReaderType);

        [DllImport("ec_pboc.dll")]
        protected static extern void EMVGetField55(byte[] field55,ref int field55Len);

        [DllImport("ec_pboc.dll")]
        private static extern void EMVSetLog(int logTag);


        [DllImport("ec_pboc.dll")]
        protected static extern void EMVL2SetOfflinePinData(byte[] pin,int pinLen);

        [DllImport("ec_pboc.dll")]
        protected static extern void setLoadAmount(byte[] amount);
        [DllImport("ec_pboc.dll")]
        private static extern void EMV_ucSetAPDUInterface(string icdll, string icfunc, string rfdll, string rffunc);
        [DllImport("ec_pboc.dll")]
        private static extern void EMVSetParam(string traceNo, string batchNo, string custName, string terminal, string branchNo);


        [DllImport("ec_pboc.dll")]
        protected static extern void EMVL2GetECashBal(byte[] bal);
        [DllImport("ec_pboc.dll")]
        protected static extern int EMVL2GetECashLog(int aidNo, byte[] line, byte[] row, byte[] log);
        [DllImport("ec_pboc.dll")]
        protected static extern void EMVSetAidAndCAFileName(byte[] fileName, int nameLen);
        #endregion   

        public int App_EMVLInit(int CardReaderType, long hand)
        {
            try
            {
                return EMVL2APPInit(CardReaderType, hand);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void App_EMVSetAidAndCAFileName(byte[] fileName, int nameLen)
        {
            EMVSetAidAndCAFileName(fileName, nameLen);
        }

        public int App_EMVStartEmvApp(int aidNo,int cProtocol, int inTransType, byte[] inTrace, byte[] inDay, byte[] inTime, byte[] inAmount, byte[] inOtherAmount)
        {
            return EMVL2StartEmvApp(aidNo,cProtocol, inTransType, inTrace, inDay, inTime, inAmount, inOtherAmount);
        }
        public void App_EMV_ucSetAPDUInterface(string icdll, string icfunc, string rfdll, string rffunc)
        {
            EMV_ucSetAPDUInterface(icdll, icfunc, rfdll, rffunc);
        }
        public int App_EMVL2SelectApp(int cProtocol, byte[] appList, ref int nListNum)
        {
            return EMVL2SelectApp(cProtocol,appList, ref nListNum);
        }

        public void App_EMVSetParam(string traceNo, string batchNo, string custName, string terminal, string branchNo)
        {
            EMVSetParam(traceNo, batchNo, custName, terminal, branchNo);
        }

        /// <summary>
        /// 2,打日志
        /// </summary>
        /// <param name="logTag"></param>
        public void App_EMVSetLog(int logTag)
        {
            EMVSetLog(logTag);
        }


        public int App_EMVOnlineDataProcess(int retResult,byte[] buf55,int buf55Len,byte[] auth_id,int authLen)
        {
            return EMVL2OnlineDataProcess(retResult, buf55, buf55Len, auth_id, authLen);
        }

        public void App_EMVGetECashBal(byte[] bal)
        {
            EMVL2GetECashBal(bal);
        }

        public int App_EMVGetECashLog(int aidNo,byte[] line, byte[] row, byte[] log)
        {
            return EMVL2GetECashLog(aidNo, line, row, log);
        }

        public int App_EMVTermRiskManageProcessRestrict()
        {
            return EMVL2TermRiskManageProcessRestrict();
        }


        public void App_EMVGetCardNo(byte[] cardNo, ref int lenCard, byte[] cardTrack2, ref int track2Len, byte[] expData, ref int expLen, byte[] cardSeqNum)
        {
            EMVL2GetCardNo(cardNo, ref lenCard, cardTrack2, ref track2Len, expData, ref expLen, cardSeqNum);
        }

        public int App_EMVCardHolderValidate(ref int cTimes)
        {
            return EMVL2CardHolderValidate(ref cTimes);
        }

        public int App_EMVContinueCardHolderValidate(int type,ref int cTimes)
        {
            return EMVL2ContinueCardHolderValidate(type, ref cTimes);
        }

        

        public void App_EMVGetField55(byte[] field55, ref int field55Len)
        {
            EMVGetField55(field55, ref field55Len);
        }
        

        public int App_EMVActionAnalysis()
        {
            return EMVActionAnalysis();
        }

        public void App_EMVSetOfflinePinData(byte[] pin,int pinLen)
        {
            EMVL2SetOfflinePinData(pin, pinLen);
        }

        public void setAmount(byte[] amount)
        {
            setLoadAmount(amount);
        }

        /// <summary>
        /// 提取TLV标签
        /// string str9F10 = pboc.App_EMVGetData("9F10", ref iValueLen, ref strValue);
        /// </summary>
        /// <param name="strTag"></param>
        /// <param name="iValueLen"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public string App_EMVGetData(string strTag, ref int iValueLen, ref string strValue)
        {
            try
            {
                //PubFunc pubCom = new PubFunc();
                string strRet = "";
                byte[] ptag = PubFunc.HexStringToByteArray(strTag);
                byte[] res = new byte[2048];
                int retLen = 0;
                int tagLen = ptag.Length;

                EMVGetDate(ptag, tagLen, res, ref  retLen);
                byte[] res2 = new byte[retLen];
                Array.Copy(res, res2, retLen);

                strValue = PubFunc.ByteArrayToHexString(res2, res2.Length);

                iValueLen = retLen;

                strRet = String.Format("{0}{1:X2}{2}", strTag, iValueLen, strValue);

                return strRet;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 提取TLV标签
        /// string str9F10 = pboc.App_EMVGetData("9F10", ref iValueLen, ref strValue);
        /// 
        /// </summary>
        /// <param name="strTag"></param>
        /// <param name="iValueLenSet">期望长度</param>
        /// <param name="iValueLen"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public string App_EMVGetData(string strTag, int iValueLenSet, ref int iValueLen, ref string strValue)
        {
            try
            {
                string strRet = "";
                byte[] ptag = PubFunc.HexStringToByteArray(strTag);
                byte[] res = new byte[2048];
                int retLen = 0;
                int tagLen = ptag.Length;

                EMVGetDate(ptag, tagLen, res, ref  retLen);
                byte[] res2 = new byte[retLen];
                Array.Copy(res, res2, retLen);

                strValue = PubFunc.ByteArrayToHexString(res2, res2.Length);

                iValueLen = retLen;
                if (iValueLenSet > 0 && iValueLen == 0)
                {
                    strValue = strValue.PadRight(iValueLenSet * 2, '0');
                    iValueLen = iValueLenSet;
                }

                strRet = String.Format("{0}{1:X2}{2}", strTag, iValueLen, strValue);

                return strRet;
            }
            catch
            {
                throw;
            }
        }


      
    }
}
