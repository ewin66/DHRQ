using System.Reflection;
using System.Windows.Forms;
using EudemonLink;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;

namespace CQYBYPayment.Business.Management
{
    class ManageMenuDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            setComponnents("ComComponnents", false, "btnHome", false, "btnReturn", false);

            string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            GetElementById("CloseSys").Click += CloseSys_Click;
            GetElementById("RestartSys").Click += RestartSys_Click;
            GetElementById("CloseProg").Click += CloseProg_Click;
            GetElementById("ModifyPass").Click += ModifyPass_Click;
            GetElementById("ParamConfig").Style = "display:none";
            //GetElementById("ParamConfig").Click += ParamConfig_Click;
            GetElementById("QuitManage").Click += QuitManage_Click;
            //GetElementById("RePrint").Click += RePrint_Click;

        }

        private void CloseSys_Click(object sender, HtmlElementEventArgs e)
        {
            if (MessageBox.Show("ȷ��Ҫ�رջ�����", "��ʾ", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                WindowsController.ExitWindows(RestartOptions.PowerOff, true);
                Sleep(5000);
            }
        }

        private void RestartSys_Click(object sender, HtmlElementEventArgs e)
        {
            if (MessageBox.Show("ȷ��Ҫ����������", "��ʾ", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                WindowsController.ExitWindows(RestartOptions.Reboot, true);
                Sleep(5000);
            }
        }

        private void CloseProg_Click(object sender, HtmlElementEventArgs e)
        {
            if (MessageBox.Show("ȷ��Ҫ�رճ���", "��ʾ", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (GlobalAppData.GetInstance().EudemonSwitch)
                {
                    EudemonHandler.Instance.CloseEudemon("landi123");
                }
#if !DEBUG
                SetAutoRunCtrlRegInfo(false);
#endif
                Quit();
            }
        }

        private void ModifyPass_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("�����޸�����");
        }

        private void ParamConfig_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("�������ò�������");
        }

        private void QuitManage_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("��ʼ��");
        }

        private void RePrint_Click(object sender, HtmlElementEventArgs e)
        {
            //EnterBusiness(new PFWLPaymentStratagy());

            StartActivity("���������ش�ӡ��Ϣ��ʾ");
        }

        public override bool CanQuit()
        {
            return true;
        }
    }
}
