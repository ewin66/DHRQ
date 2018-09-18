using DHRQPayment.Entity;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class InputGasNumsDeal : FrameActivity
    {
        private DHRQPaymentEntity entity;

        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                entity = (GetBusinessEntity() as DHRQPaymentEntity);

                //GetElementById("btnReturn").Style = "display: block";
                //GetElementById("btnHome").Style = "display: block";
                setComponnents("ComComponnents", true, true, true);

                GetElementById("ok").Click += new HtmlElementEventHandler(OKClick);
                GetElementById("nums").Focus();

            }
            catch (Exception ex)
            {
                Log.Error("[InputGasNumsDeal][OnEnter] error" + ex);
            }
        }

        protected override void FrameReturnClick()
        {
            StartActivity("德化燃气明细信息显示");
        }

        private void OKClick(object sender, HtmlElementEventArgs e)
        {
            try
            {
                string numsres = GetElementById("numsres").InnerText;
                numsres = numsres.Replace(".", "");

                numsres = numsres.Remove(numsres.Length - 2);
                if (double.Parse(numsres) == 0)
                    return;

                entity.buyNums = double.Parse(numsres);
                //Log.Info("amount : " + numsres);

                StartActivity("德化燃气正在缴费查询");
            }
            catch (Exception ex)
            {
                Log.Error("[InputGasNumsDeal][OKClick] error" + ex);
            }
        }

        //private void HomeClick(object sender, HtmlElementEventArgs e)
        //{
        //    GotoMain();
        //}

        //private void ReturnClick(object sender, HtmlElementEventArgs e)
        //{
        //    //StartActivity("德化燃气插入燃气卡提示");
        //    GoBack();
        //}
    }
}
