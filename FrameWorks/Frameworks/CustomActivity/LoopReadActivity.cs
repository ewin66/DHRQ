using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks;
using System.Threading;
using Landi.FrameWorks.Package;
using Landi.FrameWorks.Frameworks;

namespace Landi.FrameWorks
{
    public abstract class LoopReadActivity : FrameActivity, IHandleMessage
    {
        protected enum Result
        {
            Again,  //������
            Success,    //�����ɹ�
            HardwareError,  //Ӳ������
            Fail,   //����ʧ��
            Cancel, //�û�ȡ��
            TimeOut,    //���볬ʱ
            RfSuccess,  //�ǽӶ����ɹ�
        }

        /// <summary>
        /// �Ƿ�����IC��
        /// </summary>
        private bool mIsUseICCard
        {
            get { return GetBusinessEntity().UseICCard; }
        }

        private string mBackId;
        protected string Track1;
        protected string Track2;
        protected string Track3;
        protected string BankCardNum;
        protected string ExpDate;
        protected string CardSeqNum;
        protected byte[] Field55;

        protected UserBankCardType BankCardType;
        private const int READ_CARD = 1;
        private R80.ActivateResult activeres;
        //public NoPinAndSignParamData nopinParams;

        protected LoopReadActivity()
        {
            mBackId = ReturnId;
            if (mBackId == null || GetElementById(mBackId) == null)
                throw new Exception("����ķ��ؼ�ID");
        }

        protected override void OnEnter()
        {
            Track1 = "";
            Track2 = "";
            Track3 = "";
            BankCardNum = "";
            ExpDate = "";
            CardSeqNum = "";
            BankCardType = UserBankCardType.None;
            CommonData.BIsCardIn = false;
            //����AID��CA�ļ�ǰ׺
            //EMVTransProcess.EMVSetAidAndCAFileName(GetBusinessEntity().SectionName);
            GetElementById(mBackId).Click += new System.Windows.Forms.HtmlElementEventHandler(Back_Click);
            SendMessage(READ_CARD);
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            UserToQuit = true;
        }

        private static void DoRead(LoopReadActivity ac)
        {
            Result ret = Result.Again;
            while (ret == Result.Again)
            {
                if (UserToQuit)
                {
                    ret = Result.Cancel;
                    break;
                }
                else if (TimeIsOut)
                {
                    ret = Result.TimeOut;
                    break;
                }
                ret = ac.ReadOnce();
            }
            if (ret == Result.Success || ret == Result.Cancel || ret == Result.TimeOut)
                Log.Debug("ReadCard : " + ret.ToString());
            else if (ret == Result.Fail)
                Log.Warn("ReadCard : " + ret.ToString());
            else
                Log.Error("ReadCard : " + ret.ToString());
            ac.HandleResult(ret);
        }

        /// <summary>
        /// ���ذ�ť��Id
        /// </summary>
        protected abstract string ReturnId
        {
            get;
        }

        /// <summary>
        /// ���ݲ���ʵ�ֽ�����ת
        /// </summary>
        /// <param name="result"></param>
        protected abstract void HandleResult(Result result);

        protected abstract Result ReadOnce();

        protected Result DefaultRead()
        {
            string trk1 = "", trk2 = "", trk3 = "";
            CardReader.Status ret = CardReader.InsertCard(ref trk1, ref trk2, ref trk3);
            if (ret == CardReader.Status.CARD_SUCC)
            {
                if(CardReader.IsUse)
                    CommonData.BIsCardIn = true;//�п�����
                if (trk1.Trim() == "")
                    Log.Debug("Track1 : NULL");
                else
                    Log.Debug("Track1 : IN");
                if (trk2.Trim() == "")
                    Log.Debug("Track2 : NULL");
                else
                    Log.Debug("Track2 : IN");
                if (trk3.Trim() == "")
                    Log.Debug("Track3 : NULL");
                else
                    Log.Debug("Track3 : IN");

                Track1 = trk1;
                Track2 = trk2;
                Track3 = trk3;
                string CardNumber = Utility.GetCardNumber(trk2, trk3);
                BankCardNum = CardNumber;
                if (CardNumber.Trim().Length > 0)
                {
                    return Result.Success;
                }
                else
                {
                    return Result.Fail;
                }
            }
            else if (ret == CardReader.Status.CARD_WAIT)
            {
                return Result.Again;
            }
            else
            {
                Log.Error("ReaderOnce:" + ret.ToString());
                return Result.HardwareError;
            }
        }

        protected Result DefaultRead2()
        {
            string trk1 = "", trk2 = "", trk3 = "";
            CardReader2.Status ret = CardReader2.InsertCard(ref trk1, ref trk2, ref trk3);
            if (ret == CardReader2.Status.CARD_SUCC)
            {
                if (trk1.Trim() == "")
                    Log.Debug("Track1 : NULL");
                else
                    Log.Debug("Track1 : " + trk1);
                if (trk2.Trim() == "")
                    Log.Debug("Track2 : NULL");
                else
                    Log.Debug("Track2 : " + trk2);
                if (trk3.Trim() == "")
                    Log.Debug("Track3 : NULL");
                else
                    Log.Debug("Track3 : " + trk3);

                Track1 = trk1;
                Track2 = trk2;
                Track3 = trk3;
                string CardNumber = Utility.GetCardNumber(trk2, trk3);
                BankCardNum = CardNumber;
                if (CardNumber.Trim().Length > 0)
                {
                    return Result.Success;
                }
                else
                {
                    return Result.Fail;
                }
            }
            else if (ret == CardReader2.Status.CARD_WAIT)
            {
                return Result.Again;
            }
            else
            {
                Log.Error("Reader2Once:" + ret.ToString());
                return Result.HardwareError;
            }
        }

        /// <summary>
        /// ���������ȣ�����IC��
        /// </summary>
        /// <returns></returns>
        protected Result DefaultRead3()
        {
            Result ret = DefaultRead();
            if ((ret == Result.Fail) && mIsUseICCard)
            {
                //����ʧ�ܣ����������IC�������IC������ģʽ
                ReportSync("none");
                EMVTransProcess emv = new EMVTransProcess();
                int state = emv.EMVTransInit(0, EMVTransProcess.PbocTransType.PURCHASE);
                CardReader.CardPowerDown();
                if (state == 0)
                {
                    BankCardNum = emv.EMVInfo.CardNum;
                    Track2 = emv.EMVInfo.Track2;
                    Log.Debug("IC Card In");
                    return Result.Success;
                }
            }

            return ret;
        }


        /// <summary>
        /// IC�����ȣ����ݴ�����������������
        /// </summary>
        /// <returns></returns>
        protected Result DefaultRead4()
        {
            bool isIcCard = true;
            int nCardType = 0;
            string trk1 = "", trk2 = "", trk3 = "";
            CardReader.Status ret = CardReader.InsertCard(ref trk1, ref trk2, ref trk3);
            if (ret == CardReader.Status.CARD_SUCC)
            {
                if (CardReader.IsUse)
                    CommonData.BIsCardIn = true;//�п�����
                //����ģʽ���ذ�ť
                ReportSync("none");
                //Log.Debug("Track1:" + trk1);
                //Log.Debug("Track2 : " + trk2);
                //Log.Debug("Track3 : " + trk3);
                string CardNumber = Utility.GetCardNumber(trk2, trk3);
                if (CardNumber.Trim().Length > 0)
                {
                    Log.Info("MS Card Deal");
                    Track1 = trk1;
                    Track2 = trk2;
                    Track3 = trk3;
                    BankCardNum = CardNumber;
                    ExpDate = Utility.GetExpDate(trk2, trk3);
                    isIcCard = Utility.CheckIcCardFlag(trk2);
                    Log.Info("isIcCard=" + isIcCard.ToString());
                    //isIcCard = true;//���ܴ��ڶ��ŵ� ic���жϴ������� ����IC������Ҫ��֤�Է� 45����
                    nCardType = 1;
                }

                if (!CardReader.IsUse)
                {
                    isIcCard = false;
                }

                if (isIcCard && mIsUseICCard)
                {
                    //CardReader.CardType(0, 0);//��ֹ������������������
                    EMVTransProcess emv = new EMVTransProcess();
                    int state = emv.EMVTransInit(0, EMVTransProcess.PbocTransType.PURCHASE);
                    CardReader.CardPowerDown();
                    if (state == 0)
                    {
                        Log.Info("IC Card Deal");
                        BankCardNum = emv.EMVInfo.CardNum;
                        Track2 = emv.EMVInfo.Track2;
                        ExpDate = emv.EMVInfo.CardExpDate;
                        CardSeqNum = emv.EMVInfo.CardSeqNum;
                        nCardType += 2;
                    }
                    //else
                    //    isIcCard = false;//��ֹ���������޷�����
                }

                BankCardType = (UserBankCardType)nCardType;

                if (mIsUseICCard && isIcCard && nCardType < 2)//����IC��������������
                    return Result.Fail;

                if (BankCardType == UserBankCardType.None)
                    return Result.Fail;
            }
            else if (ret == CardReader.Status.CARD_WAIT)
            {
                return Result.Again;
            }
            else
            {
                Log.Error("ReaderOnce:" + ret.ToString());
                return Result.HardwareError;
            }

            return Result.Success;
        }

        /// <summary>
        /// IC�����ȣ����ݴ�����������������
        /// </summary>
        /// <returns></returns>
        protected Result DefaultRead5()
        {
            bool isIcCard = true;
            int nCardType = 0;
            string trk1 = "", trk2 = "", trk3 = "";
            CardReader.Status ret = CardReader.InsertCard(ref trk1, ref trk2, ref trk3);
            if (ret == CardReader.Status.CARD_SUCC)
            {
                if (CardReader.IsUse)
                    CommonData.BIsCardIn = true;//�п�����
                //����ģʽ���ذ�ť
                ReportSync("none");
                //Log.Debug("Track1:" + trk1);
                //Log.Debug("Track2 : " + trk2);
                //Log.Debug("Track3 : " + trk3);
                string CardNumber = Utility.GetCardNumber(trk2, trk3);
                if (CardNumber.Trim().Length > 0)
                {
                    Log.Info("MS Card Deal");
                    Track1 = trk1;
                    Track2 = trk2;
                    Track3 = trk3;
                    BankCardNum = CardNumber;
                    ExpDate = Utility.GetExpDate(trk2, trk3);
                    isIcCard = Utility.CheckIcCardFlag(trk2);
                    Log.Info("isIcCard=" + isIcCard.ToString());
                    //isIcCard = true;//���ܴ��ڶ��ŵ� ic���жϴ������� ����IC������Ҫ��֤�Է� 45����
                    nCardType = 1;
                }

                if (!CardReader.IsUse)
                {
                    isIcCard = false;
                }

                if (isIcCard && mIsUseICCard)
                {
                    //CardReader.CardType(0, 0);//��ֹ������������������
                    EMVTransProcess emv = new EMVTransProcess();
                    int state = emv.EMVTransInit(0, EMVTransProcess.PbocTransType.PURCHASE);
                    CardReader.CardPowerDown();
                    if (state == 0)
                    {
                        Log.Info("IC Card Deal");
                        BankCardNum = emv.EMVInfo.CardNum;
                        Track2 = emv.EMVInfo.Track2;
                        ExpDate = emv.EMVInfo.CardExpDate;
                        CardSeqNum = emv.EMVInfo.CardSeqNum;
                        nCardType += 2;
                    }
                    else
                        isIcCard = false;//��ֹ���������޷�����
                }

                BankCardType = (UserBankCardType)nCardType;

                //if (mIsUseICCard && isIcCard && nCardType < 2)//����IC��������������
                //    return Result.Fail;

                if (BankCardType == UserBankCardType.None)
                    return Result.Fail;
            }
            else if (ret == CardReader.Status.CARD_WAIT)
            {
                return Result.Again;
            }
            else
            {
                Log.Error("ReaderOnce:" + ret.ToString());
                return Result.HardwareError;
            }

            return Result.Success;
        }
        /// <summary>
        /// �����оƬ��
        /// </summary>
        /// <returns></returns>
        protected Result InsertICCard()
        {
            CardReader.Status ret = CardReader.Insert_ICcard();
            if (ret != CardReader.Status.CARD_WAIT)
                CommonData.BIsCardIn = true;

            switch (ret)
            {
                case CardReader.Status.CARD_WAIT:
                    {
                        return Result.Again;
                    }
                case CardReader.Status.CARD_SUCC:
                    {
                        ReportSync("none");
                        return Result.Success;
                    }
                default:
                    return Result.HardwareError;
            }
        }

        /// <summary>
        /// �ǽӿ���������
        /// </summary>
        /// <returns></returns>
        //protected Result DefaultRead5()
        //{
        //    CardReader.Status CardRet;
        //    activeres = R80.ActivateResult.ET_NOSUPPORTMIF;
        //    bool isIcCard = true;
        //    int nCardType = 0;
        //    string trk1 = "", trk2 = "", trk3 = "";
        //    //if ((ret == Result.Fail) && mIsUseICCard)
        //    //{
        //    //long r = R80.GetHandle();
        //    string card = "";
        //    try
        //    {
        //        activeres = R80.ICActive(1, ref card);
        //        #region test t100
        //        //if(activeres != R80.ActivateResult.ET_SETSUCCESS)
        //        //{
        //        //    return Result.Again;
        //        //}
        //        //else if(activeres == R80.ActivateResult.ET_SETSUCCESS)
        //        //{
        //        //    ReportSync("none");
        //        //    nCardType = 4;
        //        //    QpbocTrans qpboct = new QpbocTrans();
        //        //    int status = qpboct.QPBOCReadRFCard(CommonData.Amount, EMVTransProcess.PbocTransType.PURCHASE);
        //        //    if (status == 0)
        //        //    {
        //        //        BankCardNum = qpboct.EMVInfo.CardNum;
        //        //        Track2 = qpboct.EMVInfo.Track2;
        //        //        ExpDate = qpboct.EMVInfo.CardExpDate;
        //        //        CardSeqNum = qpboct.EMVInfo.CardSeqNum.PadLeft(3, '0');
        //        //        Field55 = qpboct.EMVInfo.SendField55;
        //        //        Log.Info("qpboct.EMVInfo.CardNum = " + qpboct.EMVInfo.CardNum + " cardseqnum = " + CardSeqNum);
        //        //        BankCardType = (UserBankCardType)nCardType;

        //        //        CommonData.IsNoPin = qpboct.CheckIsNoPin(nopinParams);

        //        //        return Result.RfSuccess;
        //        //    }
        //        //    //����ʧ�ܣ����������IC�������IC������ģʽ
        //        //    return Result.Fail;

        //        //}

        //        #endregion
        //        CardRet = CardReader.InsertCard(ref trk1, ref trk2, ref trk3);
        //        if (activeres != R80.ActivateResult.ET_SETSUCCESS && CardRet == CardReader.Status.CARD_WAIT)
        //        {
        //            return Result.Again;
        //        }
        //        else if (activeres == R80.ActivateResult.ET_SETSUCCESS && CardRet != CardReader.Status.CARD_SUCC)
        //        {
        //            ReportSync("none");
        //            nCardType = 4;
        //            QpbocTrans qpboct = new QpbocTrans();
        //            int status = qpboct.QPBOCReadRFCard(CommonData.Amount, EMVTransProcess.PbocTransType.PURCHASE);
        //            if (status == 0)
        //            {
        //                BankCardNum = qpboct.EMVInfo.CardNum;
        //                Track2 = qpboct.EMVInfo.Track2;
        //                ExpDate = qpboct.EMVInfo.CardExpDate;
        //                CardSeqNum = qpboct.EMVInfo.CardSeqNum.PadLeft(3, '0');
        //                Field55 = qpboct.EMVInfo.SendField55;
        //                Log.Info("qpboct.EMVInfo.CardNum = " + qpboct.EMVInfo.CardNum + " cardseqnum = " + CardSeqNum);
        //                BankCardType = (UserBankCardType)nCardType;

        //                CommonData.IsNoPin = qpboct.CheckIsNoPin(nopinParams);

        //                return Result.RfSuccess;
        //            }
        //            //����ʧ�ܣ����������IC�������IC������ģʽ
        //            return Result.Fail;
        //        }
        //        else if (CardRet == CardReader.Status.CARD_SUCC && activeres != R80.ActivateResult.ET_SETSUCCESS)
        //        {
        //            if (CardReader.IsUse)
        //                CommonData.BIsCardIn = true;//�п�����
        //            //����ģʽ���ذ�ť
        //            ReportSync("none");
        //            //Log.Debug("Track1:" + trk1);
        //            //Log.Debug("Track2 : " + trk2);
        //            //Log.Debug("Track3 : " + trk3);
        //            string CardNumber = Utility.GetCardNumber(trk2, trk3);
        //            if (CardNumber.Trim().Length > 0)
        //            {
        //                Log.Info("MS Card Deal");
        //                Track1 = trk1;
        //                Track2 = trk2;
        //                Track3 = trk3;
        //                BankCardNum = CardNumber;
        //                ExpDate = Utility.GetExpDate(trk2, trk3);
        //                isIcCard = Utility.CheckIcCardFlag(trk2);
        //                Log.Info("isIcCard=" + isIcCard.ToString());
        //                //isIcCard = true;//���ܴ��ڶ��ŵ� ic���жϴ������� ����IC������Ҫ��֤�Է� 45����
        //                nCardType = 1;
        //            }

        //            if (!CardReader.IsUse)
        //            {
        //                isIcCard = false;
        //            }

        //            if (isIcCard && mIsUseICCard)
        //            {
        //                //CardReader.CardType(0, 0);//��ֹ������������������
        //                EMVTransProcess emv = new EMVTransProcess();
        //                int state = emv.EMVTransInit(0, EMVTransProcess.PbocTransType.PURCHASE);
        //                CardReader.CardPowerDown();
        //                if (state == 0)
        //                {
        //                    Log.Info("IC Card Deal");
        //                    BankCardNum = emv.EMVInfo.CardNum;
        //                    Track2 = emv.EMVInfo.Track2;
        //                    ExpDate = emv.EMVInfo.CardExpDate;
        //                    CardSeqNum = emv.EMVInfo.CardSeqNum.PadLeft(3, '0');
        //                    Log.Info("CardNum = " + BankCardNum + " cardseqnum = " + CardSeqNum);

        //                    nCardType += 2;
        //                }
        //                //else
        //                //    isIcCard = false;//��ֹ���������޷�����
        //            }

        //            BankCardType = (UserBankCardType)nCardType;

        //            if (mIsUseICCard && isIcCard && nCardType < 2)//����IC��������������
        //                return Result.Fail;

        //            if (BankCardType == UserBankCardType.None)
        //                return Result.Fail;
        //            return Result.Success;
        //        }
        //        return Result.Fail;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("readCard loopReadActivity DefaultRead5 err", ex);
        //        return Result.Fail;

        //    }
        //}

        private void ActiveRFCard()
        {

        }
        

        protected sealed override void OnTimeOut()
        {

        }

        protected override void OnReport(object progress)
        {
            string msg = (string)progress;
            switch (msg)
            {
                case "none":
                    {
                        GetElementById(mBackId).Style = "display:none";
                        GetElementById("btnHome").Style = "display:none";

                        break;
                    }
            }
        }

        public void HandleMessage(Message message)
        {
            if (message.what == READ_CARD)
            {
                DoRead(this);
            }
        }
    }
}
