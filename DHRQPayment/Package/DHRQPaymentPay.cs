using DHRQPayment.Business.DHRQPay;
using DHRQPayment.Entity;
using DHRQPayment.Package.EMV.DHRQPaymentEMV;
using Landi.FrameWorks;
using Landi.FrameWorks.CCBPay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace DHRQPayment.Package
{
    class DHRQPaymentPay : CCBPay
    {
        public DHRQPaymentPay() { }

        public DHRQPaymentPay(PackageBase pb)
            : base(pb) { }

        protected override string SectionName
        {
            get { return DHRQPaymentEntity.SECTION_NAME; }
        }

        protected override void HandleFrontBytes(byte[] headBytes)
        {
            //byte tmp = headBytes[9];
            //try
            //{
            //    switch (tmp & 0x0F)
            //    {
            //        case 0x03:
            //            {
            //                HasSignIn = false;
            //                //EnqueueWork(new CSignIn_PowerPay());
            //            }
            //            break;
            //        case 0x04://更新公钥
            //            CDHRQDownCA ca = new CDHRQDownCA();
            //            ca.DownPublishCA();
            //            break;
            //        case 0x05://下载IC卡参数
            //            CDHRQDownAID aid = new CDHRQDownAID();
            //            aid.DownAID();
            //            break;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Log.Warn("HandleFrontBytesy异常", ex);
            //}
        }

        protected override void PackFix()
        {
            //throw new NotImplementedException();
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            //如果签到，一直返回A0|99，可能存在死循环
            //不在做队列交易
            if (returnCode == "99" || returnCode == "A0")
            {
                HasSignIn = false;
                //EnqueueWork(new CSignIn_PowerPay());
            }
        }



        /// <summary>
        /// 获取网点编号
        /// </summary>
        /// <returns></returns>
        protected string GetBranchNo()
        {
            string BranchNo = ReadIniFile("BranchNo");
            if (BranchNo == "")
            {
                throw new Exception("尚未配置网点编号");
            }
            return BranchNo;
        }

        /// <summary>
        /// 获取操作员编号
        /// </summary>
        /// <returns></returns>
        protected string GetOperatorNo()
        {
            string OperatorNo = ReadIniFile("OperatorNo");
            if (OperatorNo == "")
            {
                throw new Exception("尚未配置操作员编号");
            }
            return OperatorNo;
        }

        public static bool CreateFile(string filname, byte[] szBuffer, int len)
        {
            bool bRet = true;
            try
            {
                if (File.Exists(filname))
                {
                    File.Delete(filname);
                }
                FileStream fs = new FileStream(filname, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(szBuffer, 0, len);
                bw.Close();
            }
            catch
            {
                Log.Warn("生成文件失败");
                bRet = false;
            }
            return bRet;
        }

        protected string GetFieldString(byte[] source, int startIndex, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(source, startIndex, result, 0, length);
            return Encoding.Default.GetString(result);
        }

        protected double GetFieldDouble(byte[] source, int startIndex, int length)
        {
            string value = GetFieldString(source, startIndex, length);
            double temp = Convert.ToDouble(value) / 100;
            return temp;
        }
        protected byte[] Get48TLVBytes()
        {
            byte[] tmp = RecvPackage.GetArrayData(48);
            int len = int.Parse(Encoding.Default.GetString(tmp, 72, 3));
            byte[] tmp1 = new byte[len];
            Array.Copy(tmp, 75, tmp1, 0, len);
            return tmp1;
        }

        public void SaveBCDFile(string fileName, string content)
        {
            using (FileStream fs = new FileStream(Path.Combine(Environment.CurrentDirectory, fileName), FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (BinaryWriter rw = new BinaryWriter(fs, Encoding.Default))
                {
                    rw.Write(Utility.str2Bcd(content));
                }
            }
        }

        protected DHRQPaymentEntity PayEntity
        {
            get { return BaseBusinessEntity as DHRQPaymentEntity; }
        }


        private static bool sHasSignIn;
        public static bool HasSignIn
        {
            get { return sHasSignIn; }
            protected set { sHasSignIn = value; }
        }
        public ArrayList GetTransferReceipt()
        {
            string sTitle = "***德化建行天然气自助缴费交易凭条***";
            int splitStringLen = Encoding.Default.GetByteCount("--------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add(" 交易类型 :  缴费");
            Lprint.Add(" 商户编号 : " + GetMerchantNo());
            Lprint.Add(" 支付帐号 : " + Utility.GetPrintCardNo(CommonData.BankCardNum));

            Lprint.Add(" 日期/时间: " + System.DateTime.Now.ToString("yyyy") + "/" + System.DateTime.Now.ToString("MM") + "/" + System.DateTime.Now.ToString("dd") + "  " + System.DateTime.Now.ToString("HH") + ":" + System.DateTime.Now.ToString("mm") + ":" + System.DateTime.Now.ToString("ss"));
            Lprint.Add(" 终 端 号 : " + GetTerminalNo());
            Lprint.Add(" 参 考 号 : " + PayEntity.PayReferenceNo);
            Lprint.Add(" 凭 证 号 : " + PayEntity.PayTraceNo);
            Lprint.Add(" 批 次 号 : " + GetBatchNo());
            Lprint.Add(" ----------------------------------");
            Lprint.Add(" " + sPadLeft + "缴费明细");
            Lprint.Add(" 缴费卡号 : " + PayEntity.cardinfo.cardNo);
            Lprint.Add(" 缴费金额 : " + CommonData.Amount);
            Lprint.Add("   ");
            Lprint.Add("   ");
            Lprint.Add(" " + sPadLeft + "*** 中国建设银行 ***");
            Lprint.Add(" " + sPadLeft + "***   德化支行   ***");
            Lprint.Add(" " + sPadLeft + "***  广安天然气  ***");
            //Lprint.Add(" " + sPadLeft + " 客服电话: 023-63086110");
            Lprint.Add("   ");
            Lprint.Add("   ");

            return Lprint;
        }


        public string CreatQRCodeImg()
        {
            //try
            //{
            //    //QRCodeHandler qr = new QRCodeHandler();
            //    string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"/QRCode/";    //文件目录
            //    string qrString = CreatQRCodeStr();                                      //二维码字符串
            //    Log.Info("qrstr = " + qrString);
            //    string logoFilePath = path + "myLogo.bmp";                                    //Logo路径50*50
            //    string filePath = path + "myCode.bmp";                                        //二维码文件名
            //    qr.CreateQRCode(qrString, "Byte", 6, 0, "H", filePath, false, logoFilePath);   //生成

            //    return filePath;
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("creatqrcodeimg err", ex);
            return "";
            //}

        }


    }
}
