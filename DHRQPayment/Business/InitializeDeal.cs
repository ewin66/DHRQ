using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using DHRQPayment.Entity;
using DHRQPayment.Package;
using DHRQPayment.Package.EMV.DHRQPaymentEMV;
using System;
using DHRQPayment.Package.DHRQPay;
using Landi.FrameWorks.Frameworks;

namespace DHRQPayment.Business
{
    internal class InitializeDeal : FrameActivity, ITimeTick
    {
        public static bool Initialized;
        private bool checkHardWareRet;
        private bool checkNetRet;
        private bool downLoadParamsRet;

        private int readyTime = 1;
        private int step;
        private int restartTime;
        private int initTime;

        public void OnTimeTick(int count)
        {
            if (step == 3)
            {
                if (readyTime == 0)
                {
                    Initialized = true;
                    StartActivity("主界面");
                }
                else
                    GetElementById("procNum").InnerText = (readyTime--).ToString();
            }
            if (step == 1)
            {
                if (initTime++ == restartTime)
                {
                    WindowsController.ExitWindows(RestartOptions.Reboot, true);
                    System.Threading.Thread.Sleep(5000);

                }
            }
        }

        private void processing(int index)
        {
            GetElementById("img" + (index + 1)).SetAttribute("src", "images/ing.gif");
        }

        private void success(int index)
        {
            GetElementById("img" + (index + 1)).SetAttribute("src", "images/csh_success.png");
        }

        private void checkHardWare()
        {
            if (!HardwareManager.OpenAll())
                checkHardWareRet = false;
            else
            {
                HardwareManager.CheckAll();
                if (!HardwareManager.HardWareError())
                    checkHardWareRet = true;
                else
                    checkHardWareRet = false;
            }

            checkNetRet = !GPRS.ExistError();
        }

        private void downLoadParams()
        {
            try
            {
                downLoadParamsRet = true;
                bool nopinret = true;
                DHRQPaymentEntity ya = new DHRQPaymentEntity("DownLoadKey");
                if (ya.DownLoadAidAndCA)
                {
                    CDHRQDownAID yaAID = new CDHRQDownAID();
                    CDHRQDownCA yaCA = new CDHRQDownCA();
                    downLoadParamsRet = yaCA.DownPublishCA() ? yaAID.DownAID() : false;//YA下载AID和CA
                    ya.DownLoadAidAndCA = !downLoadParamsRet;
                    if (!downLoadParamsRet)
                        return;
                }

                downLoadParamsRet = nopinret & downLoadParamsRet ? true : false;
            }
            catch (Exception ex)
            {
                Log.Error("init downLoadParams err", ex);
            }

        }

        private void initdata()
        {
            step = 0;
            Initialized = false;
            readyTime = 3;
            initTime = 0;
            restartTime = 120;
            checkHardWareRet = false;
            checkNetRet = false;
            downLoadParamsRet = false;
            SetManageEntryInfo("ManageEntry");
        }

        protected override void OnEnter()
        {
            initdata();
            for (var i = 0; i < step; i++)
            {
                success(i);
            }
            switch (step)
            {
                case 0:
                    processing(step);
                    PostSync(checkHardWare);
                    if (checkHardWareRet)
                    {
                        success(step);
                        goto case 1;
                    }
                if (!checkNetRet)
                    StopServiceDeal.Message = "网络故障";
                else
                    StopServiceDeal.Message = "硬件故障";
                goto default;
                case 1:
                    step = 1;
                    processing(step);
                    if (!DHRQPaymentPay.HasSignIn)
                    {
                        SyncTransaction(new ApplicationKeyPay());
                    }
                    if (!DHRQPaymentPay.HasSignIn)
                    {
                        StopServiceDeal.Message = "签到失败";
                        goto default;
                    }
                    success(step);
                    goto case 2;
                case 2:
                    step = 2;
                    processing(step);
                    //PostSync(downLoadParams);
                    //if (downLoadParamsRet)
                    {
                        success(step);
                        goto case 3;
                    }
                    //else
                    //{
                    //    StopServiceDeal.Message = "下载参数失败";
                    //    goto default;
                    //}
                case 3:
                    //GetElementById("procSys").Style = "visibility:block;";
                    readyTime = 3;
                    step = 3;
                    break;
                default:
                    StartActivity("暂停服务");
                    break;
            }
        }

        protected override void OnCreate()
        {
            var config = TimerConfig.Default();
            config.Left = 868;
            config.Top = 64;
            config.Font_Size = 22;
            SetTimerConfig(config);

            //if (ConfigFile.ReadConfigAndCreate("AppData", "AutoRun", "1").Trim() == "1")
            //{
            //    if (SetAutoRunCtrlRegInfo(true))
            //        Log.Info("设置开机自启动成功");
            //}

            #region 此功能在配置参数时已实现
            //if (ConfigFile.ReadConfigAndCreate("AppData", "AutoRun", "1").Trim() == "1")
            //{
            //    if (SetAutoRunCtrlRegInfo(true))
            //        Log.Info("设置开机自启动成功");
            //}
            //else
            //{
            //    if (SetAutoRunCtrlRegInfo(false))
            //        Log.Info("取消开机自启动成功");
            //}
            #endregion

            //初装机注册文件
            if (GlobalAppData.GetInstance().AppFirst && RegsvrStarTrans())
            {
                Log.Info("注册成功");
                GlobalAppData.GetInstance().AppFirst = false;
            }

            //GPRS.AddedToManager();
            CardReader.AddedToManager();
            Esam.AddedToManager();
            ReceiptPrinter.AddedToManager();
            //R80.AddedToManager();
            GasCardReader.AddedToManager();
        }
    }
}