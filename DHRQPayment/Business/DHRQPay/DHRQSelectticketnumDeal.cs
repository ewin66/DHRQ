using DHRQPayment.Entity;
using DHRQPayment.Package;
using DHRQPayment.Package.DHRQPay;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class DHRQSelectticketnumDeal : Activity
    {
        private DHRQPaymentEntity _entity;
        private HtmlElement btnPay;
        private HtmlElement btnplus;
        private HtmlElement btncut;
        private HtmlElement btnReturn;
        private HtmlElement inputnumbg;
        private HtmlElement fare;
        private HtmlElement totalAmount;
        private HtmlElement ticketnum;

        private int ticketnums;
        private double ticketfare;
        private double totalamount;


        protected override void OnEnter()
        {
            _entity = (DHRQPaymentEntity)GetBusinessEntity();
            _entity.initorderno();
            try
            {

                btncut = GetElementById("btncut");
                btnPay = GetElementById("Pay");
                btnplus = GetElementById("btnplus");
                btnReturn = GetElementById("Return");
                inputnumbg = GetElementById("input");
                fare = GetElementById("fare");
                totalAmount = GetElementById("totalAmount");
                ticketnum = GetElementById("ticketnum");

                btnPay.Click += new HtmlElementEventHandler(Pay_Click);
                btnReturn.Click += new HtmlElementEventHandler(Return_Click);
                btnplus.Click += new HtmlElementEventHandler(btnplus_Click);
                btncut.Click += new HtmlElementEventHandler(btncut_Click);

                inputnumbg.Click += new HtmlElementEventHandler(InputClick);
                ticketnum.LostFocus += new HtmlElementEventHandler(ticketlostfocus);
                ticketnum.GotFocus += new HtmlElementEventHandler(ticketgotfocus);
                ticketnum.KeyUp += new HtmlElementEventHandler(ticketnumKeyUp);
                fare.InnerText = _entity.TicketFare;

                ticketfare = double.Parse(_entity.TicketFare);
                if (_entity.TicketNums > 0)
                {
                    inputnumbg.Style = "position: absolute; left: 39px; top: 246px; visibility:hidden";
                    ticketnum.InnerText = _entity.TicketNums.ToString();
                    ticketnums = _entity.TicketNums;
                }
                else
                {
                    totalamount = 00.0;
                    ticketnums = 0;
                }
                totalamount = ticketfare * ticketnums;
                totalAmount.InnerText = totalamount.ToString();
                //StartActivity("重庆园博园正在打印");

                //PayProcess();
            }
            catch (Exception ex)
            {
                Log.Error("select ticket num err", ex);
            }
        }

        private void PayProcess()
        {
            DHRQTranspay pay = new DHRQTranspay();
            TransResult result = SyncTransaction(pay);
            CReverser_DHRQPaymentPay rev = new CReverser_DHRQPaymentPay(pay);
            //ReportSync("BeingPay");
            if (result == TransResult.E_SUCC)
            {
                //if (bisICCard)
                //{
                //    int state = emv.EMVTransEnd(entity.RecvField55, entity.RecvField38);
                //    if (state != 0)
                //    {
                //        rev.Reason = "06";
                //        SyncTransaction(rev);
                //        ShowMessageAndGotoMain("交易失败！|IC确认错误，交易失败，请重试");
                //        return;
                //    }
                //}

                if (ReceiptPrinter.ExistError())
                    StartActivity("重庆园博园正在打印");
                else
                    StartActivity("重庆园博园成功界面");
            }
            else if (result == TransResult.E_HOST_FAIL)
            {
                if (pay.ReturnCode == "51")
                    ShowMessageAndGotoMain("交易失败！|您卡内余额不足！");
                else if (pay.ReturnCode == "55")
                    ShowMessageAndGotoMain("交易失败！|密码错误！");
                else
                    ShowMessageAndGotoMain(pay.ReturnCode + "|" +
                        pay.ReturnMessage);
            }
            else if (result == TransResult.E_RECV_FAIL)
            {
                rev.Reason = "98";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("交易失败！|交易超时，请重试");
                return;
            }
            else if (result == TransResult.E_CHECK_FAIL)
            {
                rev.Reason = "96";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("交易失败！|系统异常，请稍后再试");
                return;
            }
            else
            {
                ShowMessageAndGotoMain("交易失败|请重试");
            }

            rev.ClearReverseFile();//在不发冲正文件的情况下，才清除冲正文件
        }


        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);
            inputnumbg.Style = "position: absolute; left: 39px; top: 246px; visibility:hidden";
            ticketnum.Focus();

            switch (keyCode)
            {
                case Keys.Enter:
                    HtmlElement btnOK = GetElementById("Pay");
                    if (btnOK != null)
                        btnOK.InvokeMember("Click");
                    break;
            }
        }

        private void Pay_Click(object sender, HtmlElementEventArgs e)
        {
            if (totalamount == 0)
            {
                return;
            }
            CommonData.Amount = totalamount;
            _entity.TicketNums = ticketnums;
            //StartActivity("重庆园博园正在交易");
            StartActivity("重庆园博园读卡界面");
            //StartActivity("重庆园博园输入密码");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        private void btnplus_Click(object sender, HtmlElementEventArgs e)
        {
            string sticketnum = ticketnum.GetAttribute("value");
            int iticketnum;

            if (string.IsNullOrEmpty(sticketnum))
            {
                inputnumbg.Style = "position: absolute; left: 39px; top: 246px; visibility:hidden";
                ticketnum.Focus();
                iticketnum = 1;
            }
            else
            {
                if (!int.TryParse(sticketnum, out iticketnum))
                    return;
                iticketnum++;
            }
            //int iticketnum = int.Parse(sticketnum);
            ticketnum.InnerText = iticketnum.ToString();
            inputchange();
        }

        private void btncut_Click(object sender, HtmlElementEventArgs e)
        {
            string sticketnum = ticketnum.GetAttribute("value");
            int iticketnum; 
            if (string.IsNullOrEmpty(sticketnum))
                return;
            if (!int.TryParse(sticketnum, out iticketnum))
                return;
            //= int.Parse(sticketnum);
            iticketnum--;
            if (iticketnum <= 0)
                return;
            ticketnum.InnerText = iticketnum.ToString();
            inputchange();
        }


        private void inputchange()
        {
            string strnums = ticketnum.GetAttribute("value");
            if (string.IsNullOrEmpty(strnums))
            {
                totalamount = 0;
            }
            else
            {
                ticketnums = int.Parse(strnums);
                totalamount = ticketfare * ticketnums;
            }
            totalAmount.InnerText = totalamount.ToString();
        }

        private void ticketnumKeyUp(object sender, HtmlElementEventArgs e)
        {
            inputchange();
        }

        private void InputClick(object sender, HtmlElementEventArgs e)
        {
            inputnumbg.Style = "position: absolute; left: 39px; top: 246px; visibility:hidden";
            ticketnum.Focus();
        }

        private void ticketlostfocus(object sender, HtmlElementEventArgs e)
        {
            string sticketnum = ticketnum.GetAttribute("value");
            if (string.IsNullOrEmpty(sticketnum))
            {
                inputnumbg.Style = "position: absolute; left: 39px; top: 246px; visibility:visible";
            }
        }

        private void ticketgotfocus(object sender, HtmlElementEventArgs e)
        {
            inputnumbg.Style = "position: absolute; left: 39px; top: 246px; visibility:hidden";
        }

        

    }
}
