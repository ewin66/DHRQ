using DHRQPayment.Entity;
using DHRQPayment.Package;
using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Business.DHRQPay
{
    class DHRQBeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            setComponnents("ComComponnents", true,  false, false);

            PrintReceipt(new DHRQPaymentPay().GetTransferReceipt());
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success || result == Result.PaperFew)
            {
                StartActivity("德化燃气成功界面");
            }
            else
            {
                ShowMessageAndGotoMain("失败|打印错误！");
            }
        }

    }
}
