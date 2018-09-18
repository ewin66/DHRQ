using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using CQYBYPayment.Entity;
using System.Windows.Forms;

namespace CQYBYPayment.Business.Management
{
    class ManageRePrintInfo : Activity
    {
        private PFWLPaymentEntity _entity;
        private int _pageCount;
        private int _pageIndex;
        private int MaxitemNum;

        protected override void OnEnter()
        {
            _entity = GetBusinessEntity() as PFWLPaymentEntity;
            MaxitemNum = int.Parse(_entity.QueryMaxRequestNum);
            GetElementById("prevpage").Click += new HtmlElementEventHandler(PrevPage_Click);
            GetElementById("nextpage").Click += new HtmlElementEventHandler(NextPage_Click);
            GetElementById("Ok").Click += new HtmlElementEventHandler(ok_Click);
            GetElementById("Back").Click += new HtmlElementEventHandler(back_Click);
            try
            {
                _entity.ReadFile4RePrint();
                _pageCount = (_entity.Recvlist.Count + 7) / MaxitemNum;
                _pageIndex = 1;

                displayMsg();
            }
            catch (Exception ex)
            {
                Log.Error("Manage Reprint Error", ex);
            }
        }

        private void displayMsg()
        {
            if (_entity.Recvlist.Count > 0)
            {
                string UserInfo = "<table width=\"984\" class=\" gridtable\" border=\"0px\" cellpadding=\"0\" cellspacing=\"1\" style=\"background-color:#ccc;\" ><tr><th style=\"width:12%;\" id=\"pic1.1\">寄件日期</th><th style=\"width:10%;\" id=\"pic1.2\">目的地</th><th style=\"width:10%;\" id=\"pic1.3\">收件人</th><th style=\"width:17%;\" id=\"pic1.4\">运单号</th><th style=\"width:11%;\" id=\"pic1.5\">货号</th><th style=\"width:10%;\" id=\"pic1.6\">代收金额</th><th style=\"width:10%;\" id=\"pic1.7\">代扣运费</th><th style=\"width:10%;\" id=\"pic1.8\">手续费</th><th style=\"width:10%;\" id=\"pic1.9\">实际应付</th></tr>";

                for (int i = (_pageIndex - 1) * MaxitemNum; i < _entity.Recvlist.Count && i < MaxitemNum; i++)
                {
                    InfoBody body = _entity.Recvlist[i];
                    string backgroundcolor;
                    string temp = "<tr id=\"{0}\" name=\"info\" style=\"background-color: {1};\" align=\"center\" valign=\"middle\"><td style=\"height: 40px\">{2}</td><td style=\"height: 40px\">{3}</td><td style=\"height: 40px\">{4}</td><td style=\" color:#279b2c; height: 40px\">{5}</td><td style=\"height: 40px\">{6}</td><td style=\"height: 40px\" >{7}</td><td style=\" color:#2f41af; height: 40px\">{8}</td><td style=\" color:#2f41af; height: 40px\">{9}</td><td style=\" color:#f00; height: 40px\">{10}</td></tr>";

                    if (i % 2 == 0)
                    {
                        backgroundcolor = "white";
                    }
                    else
                    {
                        backgroundcolor = "#eee";
                    }

                    temp = string.Format(temp, i, backgroundcolor, DealDate(body.SendDate), body.Destination, body.Recipient, body.WayBill, body.ArticleNum, DealCash(body.CollectAmt), DealCash(body.ShipWithholding), DealCash(body.Fee), DealCash(body.ActualCope));

                    UserInfo += temp;
                }
                UserInfo += "</table>";

                GetElementById("userinfo").InnerHtml = UserInfo;
                GetElementById("pageIndex").InnerText = _pageIndex.ToString();//设置页码
                GetElementById("pageCount").InnerText = _pageCount.ToString();
            }
            else
            {
                StartActivity("管理设置通用错误提示");
            }
        }

        private string DealCash(string inputStr)
        {
            if (!string.IsNullOrEmpty(inputStr))
            {
                string[] str = inputStr.Split('.');
                string ret = string.Format("{0}.{1}", str[0], str[1].Substring(0, 2));
                return ret;
            }
            return null;
        }

        private string DealDate(string inputDate)
        {
            if (!string.IsNullOrEmpty(inputDate))
            {
                string str = inputDate.Substring(2);
                return str;
            }
            return null;
        }

        private void PrevPage_Click(object sender, HtmlElementEventArgs e)
        {
            if (_pageIndex == 1)
                return;
            _pageIndex--;
            displayMsg();
        }

        private void NextPage_Click(object sender, HtmlElementEventArgs e)
        {
            if (_pageIndex == _pageCount)
                return;
            _pageIndex++;
            displayMsg();
        }

        private void ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理设置正在重打印");
        }

        private void back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("管理主界面");
        }

    }
}
