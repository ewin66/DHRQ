using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks.Package;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class DHRQReadCardDeal : LoopReadActivity
    {
        /// <summary>
        /// 插入银行卡
        /// </summary>
        protected override string ReturnId
        {
            get { return "btnReturn"; }
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            setComponnents("ComComponnents", true, false, true);
            //GetElementById("btnReturn").Click -= new HtmlElementEventHandler(DHRQReadCardDeal_Click);

            //GetElementById("btnReturn").Click += DHRQReadCardDeal_Click;
        }

        protected override void FrameReturnClick()
        {
            StartActivity("德化燃气燃气卡信息显示");
        }

        //private void DHRQReadCardDeal_Click(object sender, HtmlElementEventArgs e)
        //{
        //    StartActivity("德化燃气燃气卡信息显示");
        //}

        protected override void HandleResult(Result result)
        {
            switch (result)
            {
                case Result.Success:
                    {
#if DEBUG
                        GotoNext();
#else
                        //接触式
                        CommonData.BankCardNum = BankCardNum;
                        CommonData.BankCardSeqNum = CardSeqNum;
                        CommonData.BankCardExpDate = ExpDate;
                        CommonData.Track1 = Track1;
                        //Log.Debug("track2 : " + Track2);

                        CommonData.Track2 = Track2;
                        //Log.Debug("common track2 : " + CommonData.Track2);

                        CommonData.Track3 = Track3;
                        CommonData.UserCardType = BankCardType;
                        StartActivity("输入密码");
#endif

                    }
                    break;
                case Result.HardwareError:
                    ShowMessageAndGotoMain("读卡错误|读卡器故障");
                    break;
                case Result.Fail:
                    if (CommonData.UserCardType == UserBankCardType.IcMagCard || CommonData.UserCardType == UserBankCardType.Magcard)
                        CardReader.CardOut();
                    ShowMessageAndGotoMain("读卡错误");
                    break;
                case Result.Cancel:
                    StartActivity("主界面");
                    break;
                case Result.TimeOut:
                    StartActivity("主界面");
                    break;
            }
        }

        protected override Result ReadOnce()
        {
            return DefaultRead4();

            #region test信用卡
            //return DefaultRead5();
            //BankCardNum = "6259654001838225";
            //Track2 = "6259654001838225D41125205001020000";
            ////Track3 = "00";
            //return Result.Success;
            #endregion
            //return InsertICCard();
            //nopinParams = Global.gNoPinAndSignParamData;
            //return DefaultRead();
        }

        protected override void OnLeave()
        {
            //if()
            base.OnLeave();
            if (!CommonData.BIsCardIn)
                CardReader.CancelCommand();
        }
    }
}
