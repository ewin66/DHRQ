using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using DHRQPayment.Entity;
using DHRQPayment.Package;
using DHRQPayment.Business.DHRQPay;
using System.Reflection;
using Landi.FrameWorks.Frameworks;


namespace DHRQPayment.Business
{
    class MainPageDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            try
            {
                Esam.SetWorkmode(Esam.WorkMode.Default);
                InvokeScript("gettime");
                SetManageEntryInfo("ManageEntry");
                setBtnName("btnReturn", "btnHome");
                Log.Info("Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

                INIClass gasCardReaderIni = new INIClass(System.AppDomain.CurrentDomain.BaseDirectory + "Versionfile.ini");
                gasCardReaderIni.IniWriteValue("Version", "VersionNo", Assembly.GetExecutingAssembly().GetName().Version.ToString());

                //ConfigFile.WriteConfig("AppData", "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
                //GetElementById("ComComponnents").Style = "display: none";
                setComponnents("ComComponnents", false, false, false);
                GetElementById("btnBuy").Click += new HtmlElementEventHandler(Buy_Click);
                GetElementById("btnQuery").Click += new HtmlElementEventHandler(Query_Click);
            }
            catch(Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }


        private void Buy_Click(object sender, HtmlElementEventArgs e)
        {
            //GetElementById("ComComponnents").Style = "display: block";
            EnterBusiness(new DHRQPaymentStratagy("BuyOption"));
            //StartActivity("��������");
            //StartActivity("�»�ȼ������ѡ��");
#if DEBUG

            StartActivity("�»�ȼ������ȼ������ʾ");
            //GotoNext();
            
#else
            StartActivity("�»�ȼ������ȼ������ʾ");
#endif
        }

        private void Query_Click(object sender, HtmlElementEventArgs e)
        {
            //GetElementById("ComComponnents").Style = "display: block";
            EnterBusiness(new DHRQPaymentStratagy("QueryOption"));
            StartActivity("�»�ȼ��������ϸ��ѯ");
        }

        private DateTime ConvertToTime(string value)
        {
            string[] time = value.Split(new char[] {':'}, StringSplitOptions.None);
            DateTime result = DateTime.Today;
            int hour = int.Parse(time[0]);
            if (hour >= 24)
            {
                Log.Error("Сʱ�����ܴ���24��");
                throw new Exception("Сʱ�����ܴ���24");
            }
            result = result.AddHours(hour);
            int min = int.Parse(time[1]);
            if (min >= 60)
            {
                Log.Error("���������ܴ���60��");
                throw new Exception("���������ܴ���60");
            }
            result = result.AddMinutes(min);
            return result;
        }

        protected override void OnTimeOut()
        {
            ShowAd();
        }

        public override bool CanQuit()
        {
            return true;
        }

        protected override void OnLeave()
        {
            Esam.SetWorkmode(Esam.WorkMode.Default);
        }

        bool no_service()
        {
            //Test
            //return false;

            bool bRet = false;
            TimeSpan t = DateTime.Now.TimeOfDay;
            if ((t.Hours == 22 && t.Minutes >= 30) ||
                t.Hours == 23 ||
                t.Hours == 0)
            {
                StartActivity("��ҵ����δ��ͨ");
                bRet = true;
            }

            return bRet;
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            //base.OnKeyDown(keyCode);
            string ID = "";
            switch (keyCode)
            {
                case Keys.D1:
                    {
                        Buy_Click(ID, null);
                    }
                    break;
                case Keys.D2:
                    {
                        Query_Click(ID, null);
                    }
                    break;
                default:
                    return;
            }
        }

        


        #region ITimeTick ��Ա

        public void OnTimeTick(int count)
        {
            //string sDate = DateTime.Now.ToString("yyyy��MM��dd��");
            //string sW = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            //string sTime = DateTime.Now.ToString("HH:mm:ss");

            //string temp = sDate + " " + sW + " " + sTime;
            //GetElementById("DateTime").InnerText = temp;

            //if (count % m_AdSwitchInterval == 0)
            //{
            //    m_currpicture = (m_currpicture + 1) % mPictures.Count;
            //    GetElementById("AdImage").SetAttribute("src", mPictures[m_currpicture]);
            //}
        }

        #endregion

        #region �м���
        private List<string> mPictures = new List<string>();
        int m_currpicture = 0;
        int m_AdSwitchInterval = 10;
        /// <summary>
        /// ��ȡ����ͼƬ
        /// </summary>
        /// <param name="dir"></param>
        private void getAllPicture(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        string ext = Path.GetExtension(d).ToLower();
                        if (ext == ".jpg" || ext == ".bmp")
                        {
                            mPictures.Add(d);
                        }
                    }
                    else
                        getAllPicture(d);
                }
            }
        }
        #endregion
    }
}
#region temp
/*
private void CreditCard_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("��ҵ����δ��ͨ");
            return;
            EnterBusiness(new CreditcardStratagy());
            if (QMPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                    StartActivity("���ÿ�������ܰ��ʾ");
                else
                    StartActivity("���ÿ���ӡ�����ϼ���");
            }
            else
            {
                StartActivity("����ǩ��");
            }
        }

        private void Mobile_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("��ҵ����δ��ͨ");
            return;
            EnterBusiness(new MobileStratagy());
            if (QMPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                    StartActivity("�ֻ���ֵ������");
                else
                    StartActivity("�ֻ���ֵ��ӡ�����ϼ���");
            }
            else
            {
                StartActivity("����ǩ��");
            }
        }

        private void PetroPay_Click(object sender, HtmlElementEventArgs e)
        {
            EnterBusiness(new PetroPayStratagy());
            if (PetroChinaPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                {
                    if (R80.IsUse)
                        StartActivity(typeof(PetroPayShowUserCardDeal));
                    else
                        StartActivity(typeof(PetroPayUserLoginDeal));
                }
                else
                    StartActivity("��ʯ��֧����ӡ�����ϼ���");
            }
            else
            {
                StartActivity("����ǩ��");
            }
        }
 */
#endregion

