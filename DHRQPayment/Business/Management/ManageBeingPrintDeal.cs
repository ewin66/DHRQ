using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using CQYBYPayment.Package;

namespace CQYBYPayment.Business.PFWLPay
{
    class ManageBeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            PrintReceipt(new PFWLPaymentPay().GetRePrintReceipt());
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success || result == Result.PaperFew)
            {
                StartActivity("管理设置重打印成功");
            }
            else
            {
                StartActivity("管理设置重打印失败");
            }
        }
    }
}
