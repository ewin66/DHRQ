using Landi.FrameWorks.Iso8583;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Landi.FrameWorks.Package.Other
{
    public abstract class STRSocket : PackageBase
    {
        public enum TransCode
        {
            T_TRANS = 0, // 转账
            T_TRANS_4_QUERY = 1, // 转账查询
            T_TRANS_4_HISTORY = 2, // 转账历史查询
            T_TRANS_VERIFY = 3, // 转账确认
            T_NONE = 5 // 无转账交易
        }

        protected bool _8583flag = true;
        protected bool Packetflag = true;

        protected int headLen;

        private int headLen_8583;
        private int recvLen;
        public string SendPackageStr; //发送的数据
        public string RecvPackageStr; //接收的数据
        protected string mainKey;
        private byte[] sendstr_all;
        protected TransCode t_TransCode;

        protected STRSocket(PackageBase pb)
            : base(pb)
        {
        }

        protected STRSocket()
        {

        }

        protected virtual string PackSendBag()
        {
            return "";
        }


        protected override byte[] PackBytesAtFront(int dataLen)
        {
            var sendLen_all = dataLen + 13;
            // byte[] sendstr_all = new byte[sendLen_all];

            var before = new byte[13];

            //长度位 2字节
            before[0] = (byte)((sendLen_all - 2) / 256);
            before[1] = (byte)((sendLen_all - 2) % 256);

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

        private string GetCompanyId()
        {
            string CI = ReadIniFile("CompanyId");
            if (CI == "")
            {
                throw new Exception("尚未配置CompanyId");
            }
            return CI;
        }

        private string GetCustomerNO()
        {
            string NO = ReadIniFile("CustomerNo");
            if (NO == "")
            {
                throw new Exception("尚未配置CustomerNo");
            }
            return NO;
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
            str = ((int)t_TransCode).ToString();
            str = str.PadLeft(4, '0');
            transcode = Encoding.ASCII.GetBytes(str);
            Array.Copy(transcode, 0, head, 12, 4);

            var code = new byte[4];
            code = Encoding.ASCII.GetBytes(GetCompanyId());
            //code = Encoding.ASCII.GetBytes("0000");
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

        internal override TransResult transact()
        {
            var ret = TransResult.E_SEND_FAIL;
            try
            {
                ret = PackStr();
                if (ret != TransResult.E_SUCC)
                {
                    return ret;
                }

                ret = CommSendRecv();
                if (ret != TransResult.E_SUCC)
                {
                    return ret;
                }
                ret = UnpackStr();
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
                    socket.SendTimeout = sendTimeOut * 1000;
                    socket.ReceiveTimeout = recvTimeOut * 1000;
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

        private TransResult PackStr()
        {
            var ret = TransResult.E_PACKET_FAIL;
            SendPackageStr = PackSendBag();
            if (SendPackageStr == null)
            {
                return ret;
            }
            try
            {
                string send = SendPackageStr;
                byte[] b_send = Encoding.Default.GetBytes(send);
                sendstr_all = new byte[send.Length];

                Array.Copy(b_send, 0, sendstr_all, 0, b_send.Length);
                ret = TransResult.E_SUCC;
                Log.Debug("PackStr Succ!");
            }
            catch (Exception ex)
            {
                Log.Error("Pack Str Error!", ex);
            }
            return ret;
        }

        private TransResult UnpackStr()
        {
            var ret = TransResult.E_UNPACKET_FAIL;
            try
            {
                var recvStr = Encoding.Default.GetString(sRecvBuffer);

                if (!string.IsNullOrEmpty(recvStr))
                {
                    ret = TransResult.E_SUCC;
                }
                else
                {
                    Log.Error("Unpack Str Error! recvStr is null or empty");
                    return ret;
                }

                Log.Debug("Unpack Str Succ!");
            }
            catch (Exception ex)
            {
                Log.Error("Unpack Str Error!", ex);
            }

            return ret;
        }


    }
}
