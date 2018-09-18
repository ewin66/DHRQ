using DHRQPayment.Entity;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using Landi.FrameWorks.HardWare;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class ChangeBankCardDeal : FrameActivity,ITimeTick
    {
        private DHRQPaymentEntity entity;

        protected override void OnEnter()
        {
            base.OnEnter();
            setComponnents("ComComponnents", true, false, false);
            entity = (GetBusinessEntity() as DHRQPaymentEntity);
            //CardReader.CardOut();
            entity.isSign = false;

            if (CardReader.CardOut() != CardReader.Status.CARD_SUCC)
            {
                Log.Info("吐卡失败或无卡");
            }
            GetElementById("btnRead").Click += new HtmlElementEventHandler
(ChangeBankCardDeal_Click);
            //GetElementById("btnHome").Click += new HtmlElementEventHandler(HomeClick);
        }

        private void ChangeBankCardDeal_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("德化燃气插入银行卡");
        }

        protected override void OnTimeOut()
        {
            CardReader.CardCapture();
            Log.Warn("TakeCard TimeOut Capture Card.");
            StartActivity("主界面");
        }

        #region ITimeTick 成员

        public void OnTimeTick(int count)
        {
            CardReader.CardStatus cs = CardReader.CardStatus.CARD_POS_GATE;
            CardReader.Status s = CardReader.GetStatus(ref cs);
            if (cs == CardReader.CardStatus.CARD_POS_OUT)
            {
                //mUserTakeCard = true;
                setComponnents("ComComponnents", true, true, false);

            }
            //Log.Info("CardReader.Status s : " + s);
            //Log.Info("CardReader.Status cs : " + cs);
            //Log.Info("mUserTakeCard : " + mUserTakeCard);
        }

        #endregion
        
    }
}
