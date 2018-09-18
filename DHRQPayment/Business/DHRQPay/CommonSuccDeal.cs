using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using Landi.FrameWorks.HardWare;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class CommonSuccDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                //GetElementById("ComComponnents").Style = "display:block";
                //GetElementById("btnReturn").Style = "display: block";
                //GetElementById("btnHome").Style = "display: block";
                setComponnents("ComComponnents", true, true, true);
                CardReader.CardOut();
                Log.Info("cardRead out succ in succ");

            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        protected override void FrameReturnClick()
        {
            GotoMain();
        }

        //private void HomeClick(object sender, HtmlElementEventArgs e)
        //{
        //    //StartActivity("退卡");
        //    GotoMain();
        //}

        //private void ReturnClick(object sender, HtmlElementEventArgs e)
        //{
        //    //StartActivity("退卡");

        //    GotoMain();
        //}
    }
}