using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using System.Net.Sockets;
using System.Net;
using Landi.FrameWorks.Iso8583;

namespace Landi.FrameWorks.CCBPay
{
    public abstract class CCBPay : PackageBase
    {
        protected CCBPay()
        {

        }

        protected CCBPay(PackageBase pb)
            : base(pb)
        {

        }

        protected override bool NeedCalcMac()
        {
            if (SendPackage.GetString(3) == "349000")
                return false;
            else
                return true;
        }

        protected override byte[] PackBytesAtFront(int dataLen)
        {
            int sendLen_all = dataLen + 7;
            byte[] sendstr_all = new byte[sendLen_all];

            byte[] before = new byte[7];

            //长度位
            //before[0] = (byte)((sendLen_all - 2) / 256);
            //before[1] = (byte)((sendLen_all - 2) % 256);
            byte[] len = new byte[2];
            len = Utility.str2Bcd((sendLen_all - 2).ToString().PadLeft(4, '0'));
            Array.Copy(len, 0, before, 0, 2);
            //TPDU
            byte[] TPDU = new byte[5];
            TPDU = Utility.str2Bcd(GetTPDU());
            Array.Copy(TPDU, 0, before, 2, 5);

            ////包头
            //byte[] head = new byte[12];
            //head = Utility.str2Bcd(GetHead());
            //Array.Copy(head, 0, before, 7, 6);

            return before;
        }

        protected override bool UnPackFix()
        {
            string returnCode = RecvPackage.GetString(39);
            string msgMean = "", msgShow = "";
            ParseRespMessage(returnCode, ref msgMean, ref msgShow);

            SetRespInfo(returnCode, msgMean, msgShow);
            if (returnCode == "0000" || returnCode == "0740")
                return true;
            else
                return false;
        }

        protected override void PackFix()
        {
            SendPackage.SetArrayData(41, Encoding.ASCII.GetBytes(GetMerchantNo()));
            SendPackage.SetArrayData(42, Encoding.ASCII.GetBytes(GetTerminalNo()));
        }

        protected void PackReverse(string reason)
        {
            SendPackage.SetString(0, "0100");
            if (String.IsNullOrEmpty(reason))
            {
                reason = "06";
            }
            SendPackage.SetString(39, reason); //冲正原因
            SendPackage.ClearBitAndValue(26);
            SendPackage.ClearBitAndValue(52);
            SendPackage.ClearBitAndValue(53);
            SendPackage.ClearBitAndValue(64);
        }

        /// <summary>
        /// 冲正使用的55域
        /// </summary>
        protected byte[] GetICAutoField55(byte[] _field55, int fieldLen)
        {
            //95 9F1E 9F10 9F36 DF31
            byte[] field55 = new byte[fieldLen];
            Array.Copy(_field55, field55, fieldLen);
            TLVHandler tlv = new TLVHandler();
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(field55);
            byte[] value = new byte[0];

            #region 打包Field55

            if ((value = handler.GetBytesValue("95")) != null)
            {
                tlv.AddTag("95", value);
            }
            if ((value = handler.GetBytesValue("9F1E")) != null)
            {
                tlv.AddTag("9F1E", value);
            }
            if ((value = handler.GetBytesValue("9F10")) != null)
            {
                tlv.AddTag("9F10", value);
            }
            if ((value = handler.GetBytesValue("9F36")) != null)
            {
                tlv.AddTag("9F36", value);
            }
            if ((value = handler.GetBytesValue("DF31")) != null)
            {
                tlv.AddTag("DF31", value);
            }
            #endregion

            return tlv.GetTLV();
        }

        protected void DoSignInSucc()
        {
            string time = RecvPackage.GetString(12); //时间
            string date = RecvPackage.GetString(13); //日期
            //SetBatchNo(RecvPackage.GetString(37).Substring(2, 6));//记录批次号

            byte[] bField60 = new byte[0];
            bField60 = Utility.str2Bcd(RecvPackage.GetString(60));
            //bField60 = RecvPackage.GetArrayData(60);
            byte[] EPinkey = new byte[KeyLength];
            byte[] EMackey = new byte[KeyLength];
            Array.Copy(bField60, 0, EPinkey, 0, KeyLength);
            Array.Copy(bField60, 8, EMackey, 0, KeyLength);
            KeyManager.SetEnMacKey(SectionName, EMackey);
            KeyManager.SetEnPinKey(SectionName, EPinkey);
            byte[] bTerminalNo = Encoding.Default.GetBytes(GetTerminalNo());

            byte[] PinKey = null, WorkKey = null;
            if (EnType == EncryptType.Soft)
            {
                byte[] MasterKey = GetSoftMasterKey();
                if (DType == DesType.Des)
                {
                    byte[] EPinkeytmp = EPinkey;
                    byte[] EMackeytmp = EMackey;

                    byte[] tmp = Encrypt.DESEncrypt(EPinkeytmp, MasterKey);
                    PinKey = Encrypt.DESDecrypt(tmp, bTerminalNo);
                    
                    tmp = Encrypt.DESEncrypt(EMackeytmp, MasterKey);
                    WorkKey = Encrypt.DESDecrypt(tmp, bTerminalNo);
                }
                else if (DType == DesType.TripleDes)
                {
                    PinKey = Encrypt.DES3Decrypt(EPinkey, MasterKey);
                    WorkKey = Encrypt.DES3Decrypt(EMackey, MasterKey);
                }
            }
            else
            {
                PinKey = new byte[KeyLength];
                WorkKey = new byte[KeyLength];
                Esam.SetWorkmode(Esam.WorkMode.Encrypt);

                byte[] tempkey = new byte[8];

                Esam.UserEncrypt(GetKeyIndex(), EPinkey, KeyLength, tempkey);
                //Esam.Encrypt(EPinkey, 8, tempkey);
                PinKey = Encrypt.DESDecrypt(tempkey, bTerminalNo);

                Esam.UserEncrypt(GetKeyIndex(), EMackey, KeyLength, tempkey);
                //Esam.Encrypt(EMackey, 8, tempkey);
                WorkKey = Encrypt.DESDecrypt(tempkey, bTerminalNo);

                Esam.SetWorkmode(Esam.WorkMode.Default);
            }
            KeyManager.SetDeMacKey(SectionName, WorkKey);
            KeyManager.SetDePinKey(SectionName, PinKey);
            Log.Info("download pinKey:" + Utility.bcd2str(PinKey, PinKey.Length));
            Log.Info("download macKey:" + Utility.bcd2str(WorkKey, WorkKey.Length));
            //更新当前机器时间
            int year = System.DateTime.Now.Year;
            int month = Convert.ToInt32(date.Substring(4, 2));
            int day = Convert.ToInt32(date.Substring(6, 2));
            int hour = Convert.ToInt32(time.Substring(0, 2));
            int mi = Convert.ToInt32(time.Substring(2, 2));
            int ss = Convert.ToInt32(time.Substring(4, 2));
            DateTime newDt = new DateTime(year, month, day, hour, mi, ss);
#if !DEBUG

                Utility.SetSysTime(newDt);
#endif
//            }
        }


        private bool CalcMac(byte[] macBytes, byte[] mackey, ref byte[] MAC)
        {
            int p_len = macBytes.Length;
            
            int i, j, p;
            byte[] szBuffer = new byte[8];
            byte[] gbszBufstr = new byte[10240];
            macBytes[9] |= 1;
            Array.Copy(macBytes, gbszBufstr, p_len);
            if (p_len % 8 != 0)
                p = 1;
            else
                p = 0;
            for (i = 0; i < (p_len / 8) + p; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    szBuffer[j] ^= gbszBufstr[i * 8 + j];
                }
            }
            //Log.Debug("calc data:" + Utility.bcd2str(szBuffer,szBuffer.Length));
            MAC = Encrypt.DESEncrypt(szBuffer, mackey);
            return true;
        }

        /// <summary>
        /// 重写通信函数
        /// </summary>
        /// <returns></returns>
        internal override TransResult transact()
        {

            int headLen;
            TransResult ret = TransResult.E_SEND_FAIL;
            Socket socket = null;
            if (RealEnv)
            {
                if (!GPRSConnect()) return ret;
                IPAddress ip = IPAddress.Parse(serverIP);
                IPEndPoint ipe = new IPEndPoint(ip, serverPort); //把ip和端口转化为IPEndPoint实例
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.SendTimeout = sendTimeOut * 1000;
                    socket.ReceiveTimeout = recvTimeOut * 1000;
                    socket.Connect(ipe);
                }
                catch (Exception err)
                {
                    Log.Error(this.GetType().Name, err);
                    return ret;
                }
            }

            try
            {
                ret = TransResult.E_PACKET_FAIL;
                byte[] SendBytes = new byte[0];
                PackFix();
                Packet();
                if (SendPackage.IsNull())
                    return TransResult.E_INVALID;
                byte[] MAC = new byte[8];

                //计算mac并将mac打包进去
                if (NeedCalcMac())
                {
                    byte[] macKey = null;
                    //if (EnType == EncryptType.Hardware)
                    //    macKey = KeyManager.GetEnMacKey(mSectionName);
                    //else
                         macKey = KeyManager.GetDeMacKey(mSectionName);
                    if (macKey == null)
                        throw new Exception("尚未设置MAC密钥");
                    if ((DType == DesType.Des && macKey.Length == 16) || (DType == DesType.TripleDes && macKey.Length == 8))
                    {
                        throw new Exception("MAC密钥长度不符合DES算法");
                    }
                    int macField = 64;
                    if (SendPackage.ExistBit(1))
                        macField = 128;
                    SendPackage.SetArrayData(macField, new byte[8]);
                    byte[] tmp = SendPackage.GetSendBuffer();
                    byte[] macBytes = new byte[tmp.Length - 8];
                    Array.Copy(tmp, macBytes, macBytes.Length);
                    //Log.Debug(Utility.bcd2str(macBytes, macBytes.Length));
                    if (CalcMac(macBytes, macKey, ref MAC))
                    {
                        SendPackage.SetArrayData(macField, MAC);
                    }
                    else
                    {
                        SendPackage.ClearBitAndValue(macField);
                        throw new Exception("计算MAC失败");
                    }
                }
                SendBytes = SendPackage.GetSendBuffer();
                if (SendBytes.Length <= 0)
                {
                    return ret;
                }
                byte[] head = PackBytesAtFront(SendBytes.Length);
                headLen = head.Length;
                int sendLen_all = SendBytes.Length + head.Length;

                byte[] sendstr_all = new byte[sendLen_all];
                Array.Copy(head, sendstr_all, head.Length);
                Array.Copy(SendBytes, 0, sendstr_all, head.Length, SendBytes.Length);

                //记录原始发送日志
                //CLog.LogPackage(sendstr_all, SendPackage, CLog.LogType.Send);
                CLog.Info(CLog.GetLog(sendstr_all, SendPackage, this, CLog.LogType.Send));

                ret = TransResult.E_SEND_FAIL;
                if (RealEnv)
                {
                    int sendLen = 0;
                    sendLen = socket.Send(sendstr_all, sendLen_all, 0);
                    if (sendLen <= 0)
                    {
                        socket.Close();
                        return ret;
                    }
                }

                //从服务器端接受返回信息
                ret = TransResult.E_RECV_FAIL;
                int recvLen = 0;
                if (RealEnv)
                {
                    sRecvBuffer.Initialize();
                    recvLen = socket.Receive(sRecvBuffer, sRecvBuffer.Length, 0);

                    if (recvLen <= 0)
                    {
                        socket.Close();
                        return ret;
                    }
                    byte[] RecvBytes = new byte[recvLen - headLen];
                    Array.Copy(sRecvBuffer, headLen, RecvBytes, 0, recvLen - headLen);
                    byte[] headBytes = new byte[headLen];
                    Array.Copy(sRecvBuffer, headBytes, headLen);
                    //CLog.Info(RecvPackage);

                    //解包
                    ret = TransResult.E_UNPACKET_FAIL;
                    FrontBytes = headBytes;
                    HandleFrontBytes(headBytes);//根据报文头来判断是否要下载密钥
                    RecvPackage.ParseBuffer(RecvBytes, SendPackage.ExistValue(0));
                    //CLog.Info(RecvBytes);
                    //记录原始接收日志
                    byte[] logRecv = new byte[recvLen];
                    Array.Copy(sRecvBuffer, logRecv, recvLen);
                    //CLog.LogPackage(logRecv, RecvPackage, CLog.LogType.Recv);
                    CLog.Info(CLog.GetLog(logRecv, RecvPackage, this, CLog.LogType.Recv));
                    bool nRet = UnPackFix();
                    if (!mInvokeSetResult)
                        throw new Exception("should invoke SetRespInfo() in UnPackFix()");
                    mInvokeSetResult = false;
                    if (nRet)
                    {
                        ret = TransResult.E_SUCC;
                    }
                    else
                    {
                        ret = TransResult.E_HOST_FAIL;
                    }
                }
                else
                {
                    ret = TransResult.E_SUCC;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(this.GetType().Name, ex);
            }
            finally
            {
                if (socket != null && socket.Connected)
                    socket.Close();
            }
            return ret;
        

        }

        private bool CheckKeyValue()
        {
            try
            {
                byte[] bField62 = new byte[0];
                //bField62 = Utility.str2Bcd(RecvPackage.GetString(62));
                bField62 = RecvPackage.GetArrayData(62);
                byte[] pinKeyValue = new byte[8];
                byte[] macKeyValue = new byte[8];
                byte[] calcData = new byte[8];
                for (int iPer = 0; iPer < 8; iPer++)
                {
                    calcData[iPer] = 0x00;
                }

                bool pRet = CalcMacByPinkeySign(calcData, pinKeyValue);
                bool mRet = CalcMacByMackeySign(calcData, macKeyValue);
                if (pRet && mRet)
                {
                    byte[] pinCheckValue = new byte[4];
                    byte[] macCheckValue = new byte[4];
                    byte[] pinCheckValue_calc = new byte[4];
                    byte[] macCheckValue_calc = new byte[4];
                    switch (DType)
                    {
                        case DesType.Des:
                            Array.Copy(bField62, 8, pinCheckValue, 0, 4);
                            Array.Copy(bField62, 20, macCheckValue, 0, 4);
                            break;
                        case DesType.TripleDes:
                            Array.Copy(bField62, 16, pinCheckValue, 0, 4);
                            Array.Copy(bField62, 36, macCheckValue, 0, 4);//28
                            break;
                    }
                    Array.Copy(pinKeyValue, pinCheckValue_calc, 4);
                    Array.Copy(macKeyValue, macCheckValue_calc, 4);
                    bool checkPinRet = Utility.ByteEquals(pinCheckValue, pinCheckValue_calc);
                    bool checkMacRet = Utility.ByteEquals(macCheckValue, macCheckValue_calc);
                    if (!checkPinRet)
                        Log.Warn("[CheckKeyValue]PIN Key Check Failed!");
                    if (!checkMacRet)
                        Log.Warn("[CheckKeyValue]MAC Key Check Failed!");

                    if (checkPinRet && checkMacRet)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Log.Error("[CheckKeyValue]Error!", e);
                return false;
            }
        }
    }
}
