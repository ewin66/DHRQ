using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks.Iso8583;
using Newtonsoft.Json;

namespace Landi.FrameWorks.Package.PFBank
{
    public abstract class PFBankPay : PackageBase
    {
        public enum TransCode
        {
            T_TRANS = 0, // 转账
            T_TRANS_4_QUERY = 1, // 转账查询
            T_TRANS_4_HISTORY = 2, // 转账历史查询
            T_TRANS_VERIFY = 3, // 转账确认
            T_NONE = 4 // 无转账交易
        }

        protected bool _8583flag = true;

        protected int headLen;

        private int headLen_8583;
        private int recvLen;
        public CommunicateInfo RecvPackageJson;
        //protected List<WlInfo> ListPFWL;
        protected object SendObject;   //发送的数据
        private byte[] sendstr_all;
        protected TransCode t_TransCode;

        protected PFBankPay()
        {
            RecvPackageJson = new CommunicateInfo();
            //ListPFWL = new List<WlInfo>();
        }

        protected PFBankPay(PackageBase pb)
            : base(pb)
        {
        }

        protected virtual object PackSendObject()
        {
            return "";
        }

        protected override void PackFix()
        {
            SendPackage.SetString(41, GetTerminalNo());
            SendPackage.SetString(42, GetMerchantNo());
        }

        protected override bool UnPackFix()
        {
            var returnCode = RecvPackage.GetString(39);
            string msgMean = "", msgShow = "";
            ParseRespMessage(returnCode, ref msgMean, ref msgShow);

            SetRespInfo(returnCode, msgMean, msgShow);
            if (returnCode == "00")
                return true;
            return false;
        }

        /// <summary>
        /// 冲正使用的55域
        /// </summary>
        protected byte[] GetICAutoField55(byte[] _field55, int fieldLen)
        {
            //95 9F1E 9F10 9F36 DF31
            var field55 = new byte[fieldLen];
            Array.Copy(_field55, field55, fieldLen);
            var tlv = new TLVHandler();
            var handler = new TLVHandler();
            handler.ParseTLV(field55);
            var value = new byte[0];

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

        protected void PackReverse(string reason)
        {
            SendPackage.SetString(0, "0400");
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

        protected override byte[] PackBytesAtFront(int dataLen)
        {
            var sendLen_all = dataLen + 13;
            // byte[] sendstr_all = new byte[sendLen_all];

            var before = new byte[13];

            //长度位 2字节
            before[0] = (byte) ((sendLen_all - 2)/256);
            before[1] = (byte) ((sendLen_all - 2)%256);

            //TPDU 5字节
            var TPDU = new byte[5];
            TPDU = Utility.str2Bcd(GetTPDU());
            Array.Copy(TPDU, 0, before, 2, 5);

            //包头 6字节
            var head = new byte[12];
            head = Utility.str2Bcd(GetHead());
            Array.Copy(head, 0, before, 7, 6);

            return before;
        }

        private string GetCustomerTPDU()
        {
            string TPDU = ReadIniFile("CustomerTPDU");
            if (TPDU == "")
            {
                throw new Exception("尚未配置CustomerTPDU");
            }
            return TPDU;
        }

        protected byte[] PackHead(int len)
        {
            var sendlen = len + 8;
            int alllen = sendlen + 10;
            var head = new byte[20];

            var str = alllen.ToString();
            str = str.PadLeft(4, '0');
            var all_len = new byte[2];
            all_len = Utility.str2Bcd2(str);
            //all_len = Encoding.ASCII.GetBytes(str);
            Array.Copy(all_len, 0, head, 0, 2);

            //TPDU 5字节
            var CustomerTPDU = new byte[5];
            CustomerTPDU = Utility.str2Bcd(GetCustomerTPDU());
            Array.Copy(CustomerTPDU, 0, head, 2, 5);

            str = sendlen.ToString();
            str = str.PadLeft(5, '0');

            var send_len = new byte[5];
            send_len = Encoding.ASCII.GetBytes(str);
            Array.Copy(send_len, 0, head, 7, 5);

            var transcode = new byte[4];
            str = ((int) t_TransCode).ToString();
            str = str.PadLeft(4, '0');
            transcode = Encoding.ASCII.GetBytes(str);
            Array.Copy(transcode, 0, head, 12, 4);

            var code = new byte[4];
            code = Encoding.ASCII.GetBytes("0000");
            Array.Copy(code, 0, head, 16, 4);

            return head;
        }

        protected byte[] UnpackHead()
        {
            var RecvData = new byte[recvLen - 20];
            var head = new byte[20];
            var datalen = 0;
            string str;

            Array.Copy(sRecvBuffer, head, 20);

            var data_len = new byte[5];
            Array.Copy(head, 7, data_len, 0, 5);
            str = Encoding.ASCII.GetString(data_len);
            datalen = int.Parse(str) - 8;
            Array.Copy(sRecvBuffer, 20, RecvData, 0, datalen);

            return RecvData;
        }

        //protected 

        protected override bool NeedCalcMac()
        {
            if (SendPackage.GetString(0) == "0800")
                return false;
            return true;
        }

        protected void DoSignInSucc()
        {
            var time = RecvPackage.GetString(12); //时间
            var date = RecvPackage.GetString(13); //日期
            SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //记录批次号

            var bField62 = new byte[0];
            bField62 = RecvPackage.GetArrayData(62); // Utility.str2Bcd(RecvPackage.GetString(62));

            var EPinkey = new byte[KeyLength];
            var EMackey = new byte[KeyLength];
            var ETrakey = new byte[KeyLength];
            switch (DType)
            {
                case DesType.Des: //目前不支持des单倍长
                    Array.Copy(bField62, 0, EPinkey, 0, KeyLength);
                    Array.Copy(bField62, 12, EMackey, 0, KeyLength);
                    break;
                case DesType.TripleDes:
                    Array.Copy(bField62, 1, EPinkey, 0, 16);
                    Array.Copy(bField62, 21, EMackey, 0, 16);
                    Array.Copy(bField62, 41, ETrakey, 0, 16);
                    break;
            }
            KeyManager.SetEnMacKey(SectionName, EMackey);
            KeyManager.SetEnPinKey(SectionName, EPinkey);
            KeyManager.SetEnTraKey(SectionName, ETrakey);
            //Log.Debug("MackeyEn:" + Utility.bcd2str(EMackey, EMackey.Length));
            //Log.Debug("PinkeyEn:" + Utility.bcd2str(EPinkey, EPinkey.Length));

            byte[] PinKey = null, WorkKey = null, TraKey = null;
            if (EnType == EncryptType.Soft)
            {
                var MasterKey = GetSoftMasterKey();
                if (DType == DesType.Des)
                {
                    PinKey = Encrypt.DESDecrypt(EPinkey, MasterKey);
                    WorkKey = Encrypt.DESDecrypt(EMackey, MasterKey);
                    TraKey = Encrypt.DESDecrypt(ETrakey, MasterKey);
                }
                else if (DType == DesType.TripleDes)
                {
                    PinKey = Encrypt.DES3Decrypt(EPinkey, MasterKey);
                    WorkKey = Encrypt.DES3Decrypt(EMackey, MasterKey);
                    TraKey = Encrypt.DES3Decrypt(ETrakey, MasterKey);
                }
            }
            else
            {
                PinKey = new byte[KeyLength];
                WorkKey = new byte[KeyLength];
                TraKey = new byte[KeyLength];
                Esam.SetWorkmode(Esam.WorkMode.Encrypt);

                Esam.UserDecrypt(GetKeyIndex(), EPinkey, KeyLength, PinKey);
                Esam.UserDecrypt(GetKeyIndex(), EMackey, KeyLength, WorkKey);
                Esam.UserDecrypt(GetKeyIndex(), ETrakey, KeyLength, TraKey);
                Esam.SetWorkmode(Esam.WorkMode.Default);
            }
            KeyManager.SetDeMacKey(SectionName, WorkKey);
            KeyManager.SetDePinKey(SectionName, PinKey);
            KeyManager.SetDeTraKey(SectionName, TraKey);

            if (!CheckKeyValue())
            {
                SetResult(TransResult.E_KEYVERIFY_FAIL);
            }
            else
            {
                ////更新当前机器时间
                //var year = DateTime.Now.Year;
                //var month = Convert.ToInt32(date.Substring(0, 2));
                //var day = Convert.ToInt32(date.Substring(2, 2));
                //var hour = Convert.ToInt32(time.Substring(0, 2));
                //var mi = Convert.ToInt32(time.Substring(2, 2));
                //var ss = Convert.ToInt32(time.Substring(4, 2));
            }
        }

        private bool CheckKeyValue()
        {
            try
            {
                var bField62 = new byte[0];
                bField62 = RecvPackage.GetArrayData(62); // Utility.str2Bcd(RecvPackage.GetString(62));

                var pinKeyValue = new byte[8];
                var macKeyValue = new byte[8];
                var traKeyValue = new byte[8];
                var calcData = new byte[8];
                for (var iPer = 0; iPer < 8; iPer++)
                {
                    calcData[iPer] = 0x00;
                }

                var pRet = CalcMacByPinkey(calcData, pinKeyValue);
                var mRet = CalcMacByMackey(calcData, macKeyValue);
                var tRet = CalcMacByTrakey(calcData, traKeyValue);
                if (pRet && mRet && tRet)
                {
                    var pinCheckValue = new byte[4];
                    var macCheckValue = new byte[4];
                    var traCheckValue = new byte[4];
                    var pinCheckValue_calc = new byte[4];
                    var macCheckValue_calc = new byte[4];
                    var traCheckValue_calc = new byte[4];
                    switch (DType)
                    {
                        case DesType.Des: //暂时不支持
                            Array.Copy(bField62, 8, pinCheckValue, 0, 4);
                            Array.Copy(bField62, 20, macCheckValue, 0, 4);
                            break;
                        case DesType.TripleDes:
                            Array.Copy(bField62, 17, pinCheckValue, 0, 4);
                            Array.Copy(bField62, 37, macCheckValue, 0, 4); //28
                            Array.Copy(bField62, 57, traCheckValue, 0, 4);
                            break;
                    }
                    Array.Copy(pinKeyValue, pinCheckValue_calc, 4);
                    Array.Copy(macKeyValue, macCheckValue_calc, 4);
                    Array.Copy(traKeyValue, traCheckValue_calc, 4);
                    var checkPinRet = Utility.ByteEquals(pinCheckValue, pinCheckValue_calc);
                    var checkMacRet = Utility.ByteEquals(macCheckValue, macCheckValue_calc);
                    var checkTraRet = Utility.ByteEquals(traCheckValue, traCheckValue_calc);
                    if (!checkPinRet)
                        Log.Warn("[CheckKeyValue]PIN Key Check Failed!");
                    if (!checkMacRet)
                        Log.Warn("[CheckKeyValue]MAC Key Check Failed!");
                    if (!checkTraRet)
                        Log.Warn("[CheckKeyValue]Tra Key Check Failed!");
                    if (checkPinRet && checkMacRet && checkTraRet)
                        return true;
                    return false;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error("[CheckKeyValue]Error!", e);
                return false;
            }
        }

        internal override TransResult transact()
        {
            var ret = TransResult.E_SEND_FAIL;
            try
            {
                ret = _8583flag ? Pack8583() : PackJson();
                if (ret != TransResult.E_SUCC)
                {
                    return ret;
                }

                ret = CommSendRecv();
                if (ret != TransResult.E_SUCC)
                {
                    return ret;
                }
                ret = _8583flag ? Unpack8583() : UnpackJson();
                if (ret != TransResult.E_SUCC)
                {
                    return ret;
                }
            }
            catch (Exception ex)
            {
                Log.Error(GetType().Name, ex);
            }

            return ret;
        }



        private TransResult Pack8583()
        {
            var ret = TransResult.E_PACKET_FAIL;

            try
            {
                var SendBytes = new byte[0];
                PackFix();
                Packet();
                if (SendPackage.IsNull())
                    return TransResult.E_INVALID;
                var MAC = new byte[8];

                //计算mac并将mac打包进去
                if (NeedCalcMac())
                {
                    byte[] macKey = null;
                    if (EnType == EncryptType.Hardware)
                        macKey = KeyManager.GetEnMacKey(mSectionName);
                    else
                        macKey = KeyManager.GetDeMacKey(mSectionName);
                    if (macKey == null)
                        throw new Exception("尚未设置MAC密钥");
                    if ((DType == DesType.Des && macKey.Length == 16) ||
                        (DType == DesType.TripleDes && macKey.Length == 8))
                    {
                        throw new Exception("MAC密钥长度不符合DES算法");
                    }
                    var macField = 64;
                    if (SendPackage.ExistBit(1))
                        macField = 128;
                    SendPackage.SetArrayData(macField, new byte[8]);
                    var tmp = SendPackage.GetSendBuffer();
                    var macBytes = new byte[tmp.Length - 8];
                    Array.Copy(tmp, macBytes, macBytes.Length);

                    if (CalcMacByMackey(macBytes, MAC))
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
                var head_8583 = PackBytesAtFront(SendBytes.Length);
                headLen_8583 = head_8583.Length;

                #region Json复合8583包打包（子类中重写PackSendObject())方法
                SendObject = PackSendObject();
                string strJson = "";
                BaseCommunicateSendJson send = SendObject as BaseCommunicateSendJson;
                if (send != null) //对象必须是继承基类型的才采用json发送
                {
                    send.rowCount = send.DS.Length;
                    strJson = JsonConvert.SerializeObject(SendObject);
                }
                #endregion

                
                //if (!string.IsNullOrEmpty(strJson))
                //{
                    byte[] json = Encoding.Default.GetBytes(strJson);
                //}
                var sendLen_all = SendBytes.Length + head_8583.Length + json.Length;

                var t_head = PackHead(sendLen_all);//自定义报文头
                headLen = t_head.Length;
                sendLen_all += headLen;
                sendstr_all = new byte[sendLen_all];

                Array.Copy(t_head, sendstr_all, t_head.Length);
                Array.Copy(head_8583, 0, sendstr_all, headLen, head_8583.Length);
                Array.Copy(SendBytes, 0, sendstr_all, head_8583.Length + headLen, SendBytes.Length);
                if (!string.IsNullOrEmpty(strJson))
                {
                    Array.Copy(json, 0, sendstr_all, head_8583.Length + headLen + SendBytes.Length, json.Length);
                }

                ret = TransResult.E_SUCC;
            }
            catch (Exception ex)
            {
                Log.Error(GetType().Name, ex);
            }
            return ret;
        }

        private TransResult CommSendRecv()
        {
            var ret = TransResult.E_SEND_FAIL;
            Socket socket = null;

            if (RealEnv)
            {
                if (!GPRSConnect()) return ret;
                var ip = IPAddress.Parse(serverIP);
                var ipe = new IPEndPoint(ip, serverPort); //把ip和端口转化为IPEndPoint实例
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.SendTimeout = sendTimeOut*1000;
                    socket.ReceiveTimeout = recvTimeOut*1000;
                    socket.Connect(ipe);
                }
                catch (Exception err)
                {
                    Log.Error(GetType().Name, err);
                    return ret;
                }

                try
                {
                    //发送信息给服务器
                    ret = TransResult.E_SEND_FAIL;
                    var sendLen = 0;
                    sendLen = socket.Send(sendstr_all, sendstr_all.Length, 0);
                    if (sendLen <= 0)
                    {
                        socket.Close();
                        return ret;
                    }
                    //记录原始发送日志
                    //CLog.LogPackage(sendstr_all, SendPackage, CLog.LogType.Send);
                    CLog.Info(CLog.GetLog(sendstr_all, SendPackage, this, CLog.LogType.Send));

                    //从服务器端接受返回信息
                    ret = TransResult.E_RECV_FAIL;
                    recvLen = 0;
                    sRecvBuffer.Initialize();
                    recvLen = socket.Receive(sRecvBuffer, sRecvBuffer.Length, 0);

                    if (recvLen <= 0)
                    {
                        socket.Close();
                        return ret;
                    }


                    //记录原始接收日志
                    var logRecv = new byte[recvLen];
                    Array.Copy(sRecvBuffer, logRecv, recvLen);
                    //CLog.LogPackage(logRecv, RecvPackage, CLog.LogType.Recv);
                    CLog.Info(CLog.GetLog(logRecv, RecvPackage, this, CLog.LogType.Recv));
                    //记录原始发送日志
                    //CLog.LogPackage(sendstr_all, SendPackage, CLog.LogType.Send);
                    //CLog.Info(CLog.GetLog(sRecvBuffer, RecvPackage, this, CLog.LogType.Recv));
                }
                catch (Exception ex)
                {
                    Log.Error(GetType().Name, ex);
                }
                finally
                {
                    if (socket != null && socket.Connected)
                        socket.Close();
                    ret = TransResult.E_SUCC;
                }
            }
            return ret;
        }

        private TransResult Unpack8583()
        {
            var ret = TransResult.E_RECV_FAIL;
            try
            {
                var RecvData = new byte[recvLen - headLen];
                RecvData = UnpackHead();

                byte[] len8583 = new byte[2];
                Array.Copy(RecvData, len8583, 2);
                int lenthOf8583 = len8583[0]*256 + len8583[1];
                if (lenthOf8583 != 0)
                {
                    var RecvBytes = new byte[RecvData.Length - headLen_8583]; //包体数据
                    Array.Copy(RecvData, headLen_8583, RecvBytes, 0, RecvData.Length - headLen_8583);
                    var headBytes = new byte[headLen_8583];
                    Array.Copy(RecvData, headBytes, headLen_8583); //包头数据
                    //解包
                    ret = TransResult.E_UNPACKET_FAIL;
                    FrontBytes = headBytes;
                    HandleFrontBytes(headBytes); //根据报文头来判断是否要下载密钥
                    RecvPackage.ParseBuffer(RecvBytes, SendPackage.ExistValue(0));

                    
                    var nRet = UnPackFix();
                    if (!mInvokeSetResult)
                        throw new Exception("should invoke SetRespInfo() in UnPackFix()");
                    mInvokeSetResult = false;
                    ret = nRet ? TransResult.E_SUCC : TransResult.E_HOST_FAIL;
                    CLog.Info(RecvPackage.GetLogText());
                }
                else
                {
                    ret=TransResult.E_HOST_FAIL;
                }
                if (recvLen > headLen + lenthOf8583+2)
                {
                    byte[] jsonByte = new byte[recvLen - headLen - lenthOf8583 - 2];
                    Array.Copy(RecvData,  lenthOf8583 + 2, jsonByte, 0, jsonByte.Length);
                    var strJson = Encoding.Default.GetString(jsonByte);
                    RecvPackageJson = JsonConvert.DeserializeObject<CommunicateInfo>(strJson);
                }
            }
            catch (Exception ex)
            {
                Log.Error(GetType().Name, ex);
            }
            return ret;
        }

        private TransResult PackJson()
        {
            var ret = TransResult.E_PACKET_FAIL;
            SendObject = PackSendObject();
            if (SendObject == null)
            {
                return ret;
            }
            try
            {
                string strJson;
                BaseCommunicateSendJson send = SendObject as BaseCommunicateSendJson;
                if (send!=null) //对象必须是继承基类型的才采用json发送
                {
                    send.rowCount = send.DS.Length;
                    strJson = JsonConvert.SerializeObject(SendObject);
                }
                else
                {
                    strJson = SendObject.ToString();
                }
                var b_SendJson = Encoding.Default.GetBytes(strJson);
                var head = PackHead(strJson.Length);
                headLen = head.Length;
                sendstr_all = new byte[b_SendJson.Length + headLen];

                Array.Copy(head, sendstr_all, head.Length);
                Array.Copy(b_SendJson, 0, sendstr_all, head.Length, b_SendJson.Length);
                ret = TransResult.E_SUCC;
                Log.Debug("PackJson Succ!");
            }
            catch (Exception ex)
            {
                Log.Error("Pack Json Error!", ex);
            }
            return ret;
        }

        private TransResult UnpackJson()
        {
            var ret = TransResult.E_UNPACKET_FAIL;
            try
            {
                var RecvData = new byte[recvLen - headLen];
                RecvData = UnpackHead();
                var strJson = Encoding.Default.GetString(RecvData);
                //string str = "{\"returnMessage\": \"Query Succeed/ QueryErrStr:数据查询错误…..\",\"rowCount\": \"10\",\"DS\": [{\"寄件日期\": \"2015-01-02\",\"目的地\": \" xxxxx \", \"收件人\": \"xxxxx\",\"运单号\": \" xxxxx xxxxxx\",\"运单号校验码\": \" xxxxx xxxxxx \",\"货号\": \"xxx-xx\",\"代收金额\" : \"20\",\"代扣运费\": \"130\", \"手续费\": \"10\",\"实际应付\": \"150\",\"实际应付校验码\": \"xxxxxxxx\",\"银行\" : \"xxxxx\",\"行卡号\": \"xxxxxxxx\",\"银行卡号校验码\": \"xxxxxx\",\"账户姓名\": \"xxxxxxx\"}]}";

                CommunicateInfo recv = RecvPackageJson as CommunicateInfo;
                if (recv!=null)
                {
                    RecvPackageJson = JsonConvert.DeserializeObject<CommunicateInfo>(strJson);
                    ret = TransResult.E_SUCC;
                }
                else
                {
                    if (string.Compare(strJson, "00") == 0)
                    {
                        //确认返回成功
                        ret = TransResult.E_SUCC;
                    }
                    else
                    {
                        Log.Error("TransVerify Error msg:" + strJson);
                        return ret;
                    }
                }

                Log.Debug("Unpack Json Succ!");
            }
            catch (Exception ex)
            {
                Log.Error("Unpack Json Error!", ex);
            }

            return ret;
        }
    }

}