using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using CQYBYPayment.Entity;

namespace CQYBYPayment.Business.PFWLPay
{
    class ManagePrintSuccess : Activity
    {
        private PFWLPaymentEntity _entity;

        protected override void OnEnter()
        {
            _entity = (PFWLPaymentEntity)GetBusinessEntity();
            GetElementById("Back").Click += new HtmlElementEventHandler(Return_Click);
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理主界面");
        }
    }
}
