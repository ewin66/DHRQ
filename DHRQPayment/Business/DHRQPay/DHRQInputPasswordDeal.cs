using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using DHRQPayment.Entity;

namespace DHRQPayment.Business.DHRQPay
{
    class DHRQInputPasswordDeal : EsamActivity
    {
        protected override void OnErrorLength()
        {
            InvokeScript("showBankPassLenError");
        }

        protected override string InputId
        {
            get { return "pin"; }
        }

        protected override void OnClearNotice()
        {
            InvokeScript("hideBankPassLenError");
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success)
            {
                if (string.IsNullOrEmpty(Password))
                {
                    CommonData.IsNoPin = true;
                }
                else
                {
                    CommonData.IsNoPin = false;
                }

                CommonData.BankPassWord = Password;
                StartActivity("正在交易");

            }
            else if (result == Result.Cancel || result == Result.TimeOut)
                GotoMain();
            else if (result == Result.HardwareError)
                ShowMessageAndGotoMain("系统错误|密码键盘故障");
        }

        protected override string SectionName
        {
            get { return GetBusinessEntity().SectionName; }
        }

        protected override void OnKeyDown(System.Windows.Forms.Keys keyCode)
        {

            base.OnKeyDown(keyCode);
        }

    }
}
