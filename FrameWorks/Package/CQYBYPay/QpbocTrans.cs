using Landi.FrameWorks.HardWare;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Landi.FrameWorks.Package
{
    public class QpbocTrans : EMVTransProcess
    {

        public class EMVData
        {
            public string CardNum; //银卡卡号
            public string CardSeqNum;//银行卡序列号 IC
            public string CardExpDate;//银行卡有效日期
            public string Track2; //磁道2
            public string Track3; //磁道3
            public string CommonErrorMessage; //错误说明

            public byte[] SendField55;
            public byte[] EndField55;
            public byte[] AutoField55;
            public string RecvField38;
            //public string SendField55;
            public void Clear()
            {
                CardNum = null;
                CardSeqNum = null;
                CardExpDate = null;
                Track2 = null;
                Track3 = null;
                CommonErrorMessage = "请确认插入的卡片是否为银联卡.";
                SendField55 = null;
                EndField55 = null;
                AutoField55 = null;
                RecvField38 = null;
            }
        }
        //private static NoPinAndSignParamData _noPinData;

        ///// <summary>
        ///// 免密免签参数值
        ///// </summary>
        //public static NoPinAndSignParamData gNoPinAndSignParamData
        //{
        //    get
        //    {
        //        if (_noPinData == null)
        //        {
        //            _noPinData = new NoPinAndSignParamData();
        //            _noPinData.InstanseNoPinAndSignParam();
        //        }
        //        return _noPinData;
        //    }
        //}
        /// <summary>
        /// EMV信息
        /// </summary>
        public EMVData EMVInfo = null;
        /// <summary>
        /// 支付模式：0:IC接触 1:IC感应/R80 2:qpboc脱机 3：qpboc联机
        /// </summary>
        public int PayType = 3;
        private string gICAid;
        private EC_PBOC pboc;

        public QpbocTrans()
        {
            pboc = new EC_PBOC();
            pboc.App_EMVSetLog(2);
        }

        public int QPBOCReadRFCard(double dInAmount, Landi.FrameWorks.EMVTransProcess.PbocTransType pbocType)
        {
            int state = 0;
            byte[] answer = new byte[128];
            int pnLen = 0;
            int pnChipProtocol = 0;
            EMVInfo = new EMVData();
            long hand = 0;
            try
            {
                //pboc.App_EMVSetAidAndCAFileName("yl");
                hand = R80.GetHandle();
                pboc.App_EMV_ucSetAPDUInterface("CardReader.dll", "Card_ChipIO", "R80.dll", "EA_mifare_sICAppCMDTransfer");
                //pboc.App_EMVSetParam(, BankCardConfig.gBatchNo, "", BankCardConfig.gTerminalNo, BankCardConfig.gBranchNo);

                //pboc.App_EMV_ucSetAPDUInterface("CardReader.dll", "Card_ChipIO", "R80.dll", "Card_ChipIO");
                state = pboc.App_EMVLInit(PayType, hand);
                if (state != 0)
                {
                    Log.Warn("卡片初始化失败");
                    return -1;
                }

                #region 获取卡片应用

                byte[] appList = new byte[256];
                int nListNum = 0;
                pboc.App_EMVL2SelectApp(pnChipProtocol, appList, ref nListNum);
                string[] strEmvList = System.Text.Encoding.Default.GetString(appList).Trim().Replace("\0", "").Split('|');
                if (nListNum < 1)
                {
                    Log.Warn("卡片无可用的应用");
                    return -1;
                }

                byte[] inTrace = Encoding.Default.GetBytes("000000");
                byte[] inDay = Encoding.Default.GetBytes(DateTime.Now.ToString("yyMMdd"));
                byte[] inTime = Encoding.Default.GetBytes(DateTime.Now.ToString("HHmmss"));
                byte[] inAmount = Encoding.Default.GetBytes(Utility.AmountToString(dInAmount.ToString())); ;
                byte[] inOtherAmount = Encoding.Default.GetBytes("000000000000");
                int iAppId = 1;
                bool bEmvOk = false;
                foreach (string tempEmv in strEmvList)
                {
                    //A000000333010101|银联
                    //if (!String.IsNullOrEmpty(tempEmv) && tempEmv.StartsWith("A000000333"))
                    //{
                    //    state = pboc.App_EMVStartEmvApp(iAppId, pnChipProtocol, (int)PbocTransType.PURCHASE, inTrace, inDay, inTime, inAmount, inOtherAmount);
                    //    if (state == 0)
                    //    {
                    //        bEmvOk = true;
                    //        break;
                    //    }
                    //}
                    state = pboc.App_EMVStartEmvApp(iAppId, pnChipProtocol, (int)pbocType, inTrace, inDay, inTime, inAmount, inOtherAmount);
                    if (state == 0 || state == 8)
                    {
                        bEmvOk = true;
                        gICAid = tempEmv;
                        break;
                    }
                    iAppId++;
                }
                if (!bEmvOk)
                {
                    Log.Warn("没有支持的应用");
                    return -1;
                }

                #endregion

                #region 获取卡片卡号信息

                byte[] cardNo = new byte[21];
                int cardNoLen = 0;
                byte[] track2 = new byte[38];
                int track2Len = 0;
                byte[] expData = new byte[5];
                int expLen = 0;
                byte[] cardSeqNum = new byte[2];

                pboc.App_EMVGetCardNo(cardNo, ref cardNoLen, track2, ref track2Len, expData, ref expLen, cardSeqNum);
                EMVInfo.CardNum = Encoding.Default.GetString(cardNo).Trim('\0');
                EMVInfo.Track2 = Encoding.Default.GetString(track2).Trim('\0');
                EMVInfo.CardSeqNum = Convert.ToString(cardSeqNum[0]).Trim('\0');
                EMVInfo.CardExpDate = Encoding.Default.GetString(expData).Trim('\0');
                if (String.IsNullOrEmpty(EMVInfo.CardNum))
                {
                    Log.Warn("IC:读卡号失败");
                    return -1;
                }
                else
                    EMVInfo.CardNum = EMVInfo.CardNum.Replace('\0', ' ').Trim();
                #endregion

                #region 获取55域

                //byte[] field55 = new byte[512];
                //int field55Len = 0;
                ////6 App_EMVGetField55
                //pboc.App_EMVGetField55(field55, ref field55Len);
                //EMVInfo.SendField55 = new byte[field55Len];
                //Array.Copy(field55, EMVInfo.SendField55, field55Len);
                EMVInfo.SendField55 = GetField55(pboc, 0);
                #endregion

            }
            catch (Exception ex)
            {
                Log.Error("IC:初始化异常", ex);
                return -1;
            }
            return 0;
        }

        public bool CheckIsNoPin(NoPinAndSignParamData gNoPinAndSignParamData)
        {
            Log.Info("Enter CheckIsNoPin");
            int iValueLen = 0;
            string strValue9F6C = "";
            string strValue9F51 = "";
            string strValueDF71 = "";
            pboc.App_EMVGetData("9F51", ref iValueLen, ref strValue9F51);
            pboc.App_EMVGetData("DF71", ref iValueLen, ref strValueDF71);
            Log.Info("9F51:" + strValue9F51 + "DF71:" + strValueDF71);
            bool cFlag = string.IsNullOrEmpty(strValue9F51) && string.IsNullOrEmpty(strValueDF71);//取币别代码

            if (gNoPinAndSignParamData.CdcvmIndentity == "1")// 启用CDCVM
            {
                pboc.App_EMVGetData("9F6C", ref iValueLen, ref strValue9F6C);
                Log.Info("9F6C:" + strValue9F6C);
                byte[] value9F6C = Encoding.Default.GetBytes(strValue9F6C);
                if (((value9F6C[0] & 0x80) != 0x80) && ((value9F6C[1] & 0x80) == 0x80))	//卡片不请求PIN，并且CDCVM已执行。
                {
                    return true;
                }
            }

            if (gNoPinAndSignParamData.QpsIndentity == "1")
            {
                //取货币代码失败,视为内卡；第一或者第二币种其中之一为人民币，即为内卡
                if (cFlag || strValueDF71 == "0156" || strValue9F51 == "0156")
                {
                    if (gNoPinAndSignParamData.BinAIndentity == "1") //启用卡BIN表A 
                    {
                        AppLog.Write("CardNo:" + CommonData.BankCardNum, AppLog.LogMessageType.Info);
                        if (gNoPinAndSignParamData.IsContainInCardBinA(CommonData.BankCardNum)) //BIN表A范围内	
                        {
                            if (CommonData.Amount <=
                                double.Parse(gNoPinAndSignParamData.QpsLimitAmt) / 100d)
                            {
                                return true;
                            }
                        }
                    }
                    else if (gNoPinAndSignParamData.BinBIndentity == "1") //启用卡BIN表B
                    {
                        AppLog.Write("gICAid:" + CommonData.BankCardNum, AppLog.LogMessageType.Info);
                        if (gICAid.StartsWith("A000000333010102") || gICAid.StartsWith("A000000333010103"))
                        //非借记卡AID，即为贷记卡
                        {
                            if (CommonData.Amount <=
                                double.Parse(gNoPinAndSignParamData.QpsLimitAmt) / 100d)
                            {
                                return true;
                            }
                        }
                        else if (gNoPinAndSignParamData.IsContainInCardBinB(CommonData.BankCardNum)) //BIN表B范围内	
                        {
                            if (CommonData.Amount <=
                                double.Parse(gNoPinAndSignParamData.QpsLimitAmt) / 100d)
                            {
                                return true;
                            }
                        }
                    }
                    //else if (gNoPinAndSignParamData.BinCIndentity == "1")
                    //{
                    //    AppLog.Write("gICAid:" + CommonData.BankCardNum, AppLog.LogMessageType.Info);
                    //    if (gNoPinAndSignParamData.IsContainInCardBinC(CommonData.BankCardNum)) //BIN表C范围内	
                    //    {
                    //        if (CommonData.Amount <=
                    //            double.Parse(gNoPinAndSignParamData.QpsLimitAmt) / 100d)
                    //        {
                    //            return false;
                    //        }
                    //        else
                    //        {
                    //            return true;
                    //        }
                    //    }
                    //    else
                    //        return true;
                    //}
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (gICAid.StartsWith("A000000333010102") || gICAid.StartsWith("A000000333010103"))
                    //非借记卡AID，即为贷记卡
                    {
                        if (CommonData.Amount <=
                              double.Parse(gNoPinAndSignParamData.QpsLimitAmt) / 100d)
                        {
                            return true;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(strValue9F6C))
                            {
                                byte[] value9F6C = Encoding.Default.GetBytes(strValue9F6C);
                                if ((value9F6C[0] & 0x80) != 0x80) //卡片不请求Pin
                                {
                                    return true;
                                }
                            }
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(strValue9F6C))
                        {
                            byte[] value9F6C = Encoding.Default.GetBytes(strValue9F6C);
                            if ((value9F6C[0] & 0x80) != 0x80) //卡片不请求Pin
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }



    }
}
