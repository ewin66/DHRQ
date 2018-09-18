using DHRQPayment.Entity;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class DHRQDetailMsgRes : FrameActivity
    {
        private DHRQPaymentEntity entity;
        private int CurrentPage;
        private int TotalPage;
        private int MaxRow = 8;
        private int MaxColumns = 5;

        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                entity = (GetBusinessEntity() as DHRQPaymentEntity);
                setComponnents("ComComponnents", true, "btnHome", true, "btnReturn", true);

                //GetElementById("btnReturn").Click += new HtmlElementEventHandler(ReturnClick);
                //GetElementById("btnHome").Click += new HtmlElementEventHandler(HomeClick);
                //GetTestData();

                //GetElementById("btnPay").Click += new HtmlElementEventHandler(PayClick);
                CurrentPage = 1;
                TotalPage = entity.detailinfolist.Count % MaxRow != 0 ? entity.detailinfolist.Count / MaxRow + 1 : entity.detailinfolist.Count / MaxRow;
                if (TotalPage > 1)
                {
                    GetElementById("firstpage").Style = "visibility: block;";
                    GetElementById("lastpage").Style = "visibility: block;";
                    GetElementById("nextpage").Style = "visibility: block;";
                    GetElementById("previouspage").Style = "visibility: block;";
                    GetElementById("firstpage").Click += new HtmlElementEventHandler(FirstPageClick);
                    GetElementById("lastpage").Click += new HtmlElementEventHandler(LastPageClick);
                    GetElementById("nextpage").Click += new HtmlElementEventHandler(NextPageClick);
                    GetElementById("previouspage").Click += new HtmlElementEventHandler(PreviousPageClick);
                }
                else
                {
                    GetElementById("firstpage").Style = "visibility: hidden;";
                    GetElementById("lastpage").Style = "visibility: hidden;";
                    GetElementById("nextpage").Style = "visibility: hidden;";
                    GetElementById("previouspage").Style = "visibility: hidden;";
                }

                DisPlayMsg();
                if (entity.cardinfo.cardType == "1")
                {
                    //购气
                    GetElementById("btnpay").SetAttribute("value", "购气");
                }
                else if (entity.cardinfo.cardType == "0")
                {
                    GetElementById("btnpay").SetAttribute("value", "充值");
                }
                else
                {
                    GetElementById("btnpay").Style = "visibility: hidden;";
                }
                GetElementById("btnpay").Click += new HtmlElementEventHandler(PayClick);

            }
            catch (NullReferenceException e)
            {
                Log.Error("[" + MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + MethodBase.GetCurrentMethod().Name + "] err" + e);
            }

        }

        private void PayClick(object sender, HtmlElementEventArgs e)
        {
            if (entity.cardinfo.cardType == "1")
            {
                //购气
                StartActivity("德化燃气购气选择");
            }
            else if (entity.cardinfo.cardType == "0")
            {
                StartActivity("德化燃气充值选择");
            }
            else
            {
                return;
            }
        }

        private void PreviousPageClick(object sender, HtmlElementEventArgs e)
        {
            if (CurrentPage <= 1)
                return;
            CurrentPage--;
            DisPlayMsg();
        }

        private void NextPageClick(object sender, HtmlElementEventArgs e)
        {
            if (CurrentPage >= TotalPage)
                return;
            CurrentPage++;
            DisPlayMsg();
        }

        private void LastPageClick(object sender, HtmlElementEventArgs e)
        {
            CurrentPage = TotalPage;
            DisPlayMsg();
        }

        private void FirstPageClick(object sender, HtmlElementEventArgs e)
        {
            CurrentPage = 1;
            DisPlayMsg();
        }

        private void DisPlayMsg()
        {
            ClearTable();

            GetElementById("pernums").InnerHtml = CurrentPage.ToString();
            GetElementById("pagenums").InnerHtml = TotalPage.ToString();
            if (entity.detailinfolist.Count <= 0)
                return;
            for (int i = 0, j = (CurrentPage - 1) * MaxRow; i < MaxRow && j < entity.detailinfolist.Count; i++, j++)
            {
                DateTime datetime = Utility.String2Datetime(entity.detailinfolist[j].TransDate + entity.detailinfolist[j].TransTime);

                GetElementById("msg" + i + "-0").InnerHtml = (j + 1).ToString();
                GetElementById("msg" + i + "-1").InnerHtml = datetime.ToShortDateString();
                GetElementById("msg" + i + "-2").InnerHtml = datetime.ToShortTimeString();
                GetElementById("msg" + i + "-3").InnerHtml = entity.detailinfolist[j].TransGasVolume.TrimStart('0');
                GetElementById("msg" + i + "-4").InnerHtml = Utility.StringToAmount(entity.detailinfolist[j].TransAmount.TrimStart('0'));
            }
        }

        private void ClearTable()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    GetElementById("msg" + i + "-" + j).InnerHtml = "";
                }
            }
        }

        //private void HomeClick(object sender, HtmlElementEventArgs e)
        //{
        //    GotoMain();
        //}

        //private void ReturnClick(object sender, HtmlElementEventArgs e)
        //{
        //    StartActivity("德化燃气插入燃气卡提示");
        //}

        private void GetTestData()
        {
            entity.detailinfolist.Clear();
            for (int i = 0; i < 100; i++)
            {
                DHRQPaymentEntity.DetailInfo d = new DHRQPaymentEntity.DetailInfo();
                d.TransCode = "12345" + i;
                d.CardNo = "54321" + i;
                d.CardRemark = "";
                d.IssuingCardTimes = "3";
                d.TransType = "T1";
                d.TransDate = "20180528";
                d.TransTime = "155211";
                d.TransGasVolume = "30";
                d.TransAmount = "0000001837";
                d.GasPrices1 = "0000000100";
                d.GasPrices2 = "0000000200";
                d.GasPrices3 = "0000000300";
                d.GasPrices4 = "0000000400";
                d.GasVolume1 = "30";
                d.GasVolume2 = "50";
                d.GasVolume3 = "80";
                d.GasVolume4 = "100";
                entity.detailinfolist.Add(d);
            }
        }
        protected override void FrameReturnClick()
        {
            StartActivity("德化燃气插入燃气卡提示");
        }

        //protected override void OnLeave()
        //{
        //    GetElementById("btnReturn").Click -= new HtmlElementEventHandler(ReturnClick);
        //}

    }
}
