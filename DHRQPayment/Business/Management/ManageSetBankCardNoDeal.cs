using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace CQYBYPayment.Business.Management
{
    class ManageSetBankCardNoDeal : Activity
    {
        private ParamInfo info = Global.ParamInfo;

        protected override void OnEnter()
        {
            if (string.IsNullOrEmpty(info.BankCardNo1))
            {
                GetElementById("message").InnerText = "转账卡参数下载无效，请重新下载！";
                GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
                GetElementById("OK").Enabled = false;
                return;
            }
            HtmlElement ele = GetElementById("bankcardNo");
            string select = "<option value=\"{0}\">{0}</option>";
            string temp = string.Format(select, info.BankCardNo1);
            if (!string.IsNullOrEmpty(info.BankCardNo2))
                temp += string.Format(select, info.BankCardNo2);
            if (!string.IsNullOrEmpty(info.BankCardNo3))
                temp += string.Format(select, info.BankCardNo3);
            ele.OuterHtml = "<select id=\"bankcardNo\" style=\"width: 406px\">" + temp + "</select>";

            GetElementById("OK").Click += new HtmlElementEventHandler(OK_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理主界面");
        }

        private void OK_Click(object sender, HtmlElementEventArgs e)
        {
            string pass = GetElementById("password").GetAttribute("value");
            string bankCardNo = GetElementById("bankcardNo").GetAttribute("value");

            if (pass.Length != 6)
            {
                GetElementById("message").InnerText = "密码长度太短，请确认！";
                return;
            }
            if (string.IsNullOrEmpty(bankCardNo))
            {
                GetElementById("message").InnerText = "财务卡帐号不能为空！";
                return;
            }
            try
            {
                //TransAccessFactory factory = new TransAccessFactory();
                //factory.InsertPassword(bankCardNo, pass);

                //if (bankCardNo == info.BankCardNo1)
                //    info.Password1 = pass;
                //else if (bankCardNo == info.BankCardNo2)
                //    info.Password2 = pass;
                //else if (bankCardNo == info.BankCardNo3)
                //{
                //    info.Password3 = pass;
                //}
            }
            catch (Exception)
            {
                Log.Info("SetBankCardPassword Err！");
                throw;
            }
            GetElementById("message").InnerText = "设置成功！";
            GetElementById("password").InnerText = "";
        }
    }
}
