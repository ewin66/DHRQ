using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using Landi.FrameWorks.HardWare;

namespace DHRQPayment.Business
{
    class EjectCardDeal : FrameActivity, ITimeTick
    {
        private bool mUserTakeCard;
        protected override void OnEnter()
        {
            setComponnents("ComComponnents", true, "btnHome", false, "btnReturn", false);
            mUserTakeCard = false;
            CardReader.CardOut();
            //PostAsync(OnResult);
        }

        private void OnResult()
        {
            int i = 0;
            while (!mUserTakeCard)
            {
                Log.Info("i = "+ i++);
                if (TimeIsOut)
                {
                    CardReader.CardCapture();
                    Log.Warn("TakeCard TimeOut Capture Card.");
                    break;
                }
                Sleep(200);
            }
            StartActivity("主界面");
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
                StartActivity("主界面");

            }
            Log.Info("CardReader.Status s : " + s);
            Log.Info("CardReader.Status cs : " + cs);
        }

        #endregion
    }
}
