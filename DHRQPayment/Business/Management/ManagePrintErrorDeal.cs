using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace CQYBYPayment.Business.PFWLPay
{
    class ManagePrintErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Message").InnerText = "重打印失败";
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
            GetElementById("Retry").Click += new HtmlElementEventHandler(Retry_Click);
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理主界面");
        }
        void Retry_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("浦发物流转账正在打印");
        }
    }
}
