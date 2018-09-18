using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace CQYBYPayment.Business.Management
{
    class ManageCommonErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Message").InnerText = "重打印出错，无数据！";
            GetElementById("Back").Click += new HtmlElementEventHandler(Return_Click);

        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理主界面");
        }

    }
}
