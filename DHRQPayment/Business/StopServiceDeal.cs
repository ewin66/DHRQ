using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using Landi.FrameWorks.HardWare;

namespace DHRQPayment.Business
{
    class StopServiceDeal : FrameActivity, ITimeTick
    {
        private const int timeout = 10;
        public static string Message;

        protected override void OnEnter()
        {
            //GetElementById("ComComponnents").Style = "display: none";
            setComponnents("ComComponnents", false, "btnHome", true, "btnReturn", true);

            string msg = (string)MyIntent.GetExtra("Message");
            if (string.IsNullOrEmpty(msg))
                msg = Message;
            GetElementById("Message1").InnerText = msg;
            GetElementById("Message2").InnerText = "请联系工作人员！";
            //GetElementById("Message1").Style = "font-size:25px; visibility:visible; color: #000000; font-weight: normal;";
        }

        public override bool CanQuit()
        {
            return true;
        }

        public void OnTimeTick(int count)
        {
            if (count % timeout == 0)
            {
                if (!InitializeDeal.Initialized)
                    StartActivity("初始化");
                else
                {
                    HardwareManager.CheckAll();
                    if (GPRS.ExistError())
                        return;
                    if (!CardReader.ExistError() && !Esam.ExistError())
                        GotoMain();
                }
            }
        }
    }
}
