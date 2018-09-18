using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks.Package;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class DHRQReadGasCardDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                setComponnents("ComComponnents", true, true, true);
                GetElementById("btnReadCard").Click += new HtmlElementEventHandler(ReadCardClick);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected override void FrameReturnClick()
        {
#if DEBUG
            GotoMain();
#else
            GotoMain();
#endif
        }

        private void ReadCardClick(object sender, HtmlElementEventArgs e)
        {


#if DEBUG
            GotoNext();
#else
            StartActivity("德化燃气正在读燃气卡");
#endif
            //StartActivity("德化燃气正在查询");
        }
    }
}
