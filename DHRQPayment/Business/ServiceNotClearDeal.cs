using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;

namespace DHRQPayment.Business
{
    class ServiceNotClearDeal : FrameActivity
    {
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("主界面");
        }

        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
        }
    }
}
