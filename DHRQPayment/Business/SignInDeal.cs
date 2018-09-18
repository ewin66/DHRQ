using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using DHRQPayment.Package;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks.Frameworks;

namespace DHRQPayment.Business
{
    class SignInDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            bool succ = false;
            string businessName = GetBusinessName();
            if (businessName == "PFWLPaymentPay")
            {
                SyncTransaction(new CSignIn_DHRQPaymentPay());
                succ = DHRQPaymentPay.HasSignIn;
            }
            if (!succ)
            {
                ShowMessageAndGotoMain("签到失败|该业务暂时不能使用");
            }
            else
            {
                switch (GetBusinessName())
                {
                    //case "CreditCard":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //        StartActivity("信用卡还款温馨提示");
                    //    else
                    //        StartActivity("信用卡打印机故障继续");
                    //    break;
                    //case "Mobile":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //        StartActivity("手机充值主界面");
                    //    else
                    //        StartActivity("手机充值打印机故障继续");
                    //    break;
                    //case "YAPublishPay":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //    {
                    //        //StartActivity("雅安支付菜单");
                    //        StartActivity("雅安支付输入用户号");
                    //    }
                    //    else
                    //        StartActivity("雅安支付打印机故障继续");
                    //    break;
                    //case "PowerPay":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //    {
                    //        StartActivity("电力支付菜单");
                    //    }
                    //    else
                    //        StartActivity("电力支付打印机故障继续");
                    //    break;

                    //case "YATrafficPolice":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //    {
                    //        StartActivity("雅安交警罚没菜单");
                    //    }
                    //    else
                    //        StartActivity("雅安交警罚没打印机故障继续");
                    //    break;

                    //case "PFWLPaymentPay":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //    {
                    //        StartActivity("雅安交警罚没菜单");
                    //    }
                    //    else
                    //        StartActivity("雅安交警罚没打印机故障继续");
                    //    break;

                }
            }
        }
    }
}
