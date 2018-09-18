using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;

namespace CQYBYPayment.Business.Management
{
    class ManageLoginDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            setComponnents("ComComponnents", true, "btnHome", false, "btnReturn", false);
            GetElementById("Password").Focus();
            GetElementById("ok").Click += Ok_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string passWord = GetElementById("Password").GetAttribute("value").Trim();
            if (passWord.Length < 6)
            {
                GetElementById("Password").SetAttribute("value", "");
                return;
            }

            //校验密码
            if (Encrypt.AESEncrypt(passWord, GlobalAppData.EncryptKey) == GlobalAppData.GetInstance().EntryPwd)
            {
                StartActivity("管理主界面");
            }
            else
            {
                GetElementById("Password").SetAttribute("value", "");
            }
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);
            InputNumber("Password", keyCode);
        }

        protected override void OnTimeOut()
        {
            StartActivity("主界面");
        }
    }
}
