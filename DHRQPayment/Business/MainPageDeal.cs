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
            //StartActivity("输入密码");
            //StartActivity("德化燃气购气选择");
#if DEBUG

            StartActivity("德化燃气插入燃气卡提示");
            //GotoNext();
            
#else
            StartActivity("德化燃气插入燃气卡提示");
#endif
        }

        private void Query_Click(object sender, HtmlElementEventArgs e)
        {
            //GetElementById("ComComponnents").Style = "display: block";
            EnterBusiness(new DHRQPaymentStratagy("QueryOption"));
            StartActivity("德化燃气正在明细查询");
        }

        private DateTime ConvertToTime(string value)
        {
            string[] time = value.Split(new char[] {':'}, StringSplitOptions.None);
            DateTime result = DateTime.Today;
            int hour = int.Parse(time[0]);
            if (hour >= 24)
            {
                Log.Error("小时数不能大于24！");
                throw new Exception("小时数不能大于24");
            }
            result = result.AddHours(hour);
            int min = int.Parse(time[1]);
            if (min >= 60)
            {
                Log.Error("分钟数不能大于60！");
                throw new Exception("分钟数不能大于60");
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
                StartActivity("该业务暂未开通");
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

        


        #region ITimeTick 成员

        public void OnTimeTick(int count)
        {
            //string sDate = DateTime.Now.ToString("yyyy年MM月dd日");
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

        #region 中间广告
        private List<string> mPictures = new List<string>();
        int m_currpicture = 0;
        int m_AdSwitchInterval = 10;
        /// <summary>
        /// 读取所有图片
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
            StartActivity("该业务暂未开通");
            return;
            EnterBusiness(new CreditcardStratagy());
            if (QMPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                    StartActivity("信用卡还款温馨提示");
                else
                    StartActivity("信用卡打印机故障继续");
            }
            else
            {
                StartActivity("正在签到");
            }
        }

        private void Mobile_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("该业务暂未开通");
            return;
            EnterBusiness(new MobileStratagy());
            if (QMPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                    StartActivity("手机充值主界面");
                else
                    StartActivity("手机充值打印机故障继续");
            }
            else
            {
                StartActivity("正在签到");
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
                    StartActivity("中石油支付打印机故障继续");
            }
            else
            {
                StartActivity("正在签到");
            }
        }
 */
#endregion

