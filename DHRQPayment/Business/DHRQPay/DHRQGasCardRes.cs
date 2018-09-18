using DHRQPayment.Entity;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class DHRQGasCardRes : FrameActivity
    {
        private DHRQPaymentEntity entity;

        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                entity = (GetBusinessEntity() as DHRQPaymentEntity);
                //GetElementById("btnReturn").Style = "display: block";
                //GetElementById("btnHome").Style = "display: block";
                setComponnents("ComComponnents", true, true, true);

                GetElementById("btnPay").Click += new HtmlElementEventHandler(PayClick);

                GetElementById("Username").InnerHtml = entity.returnCardInfo.userName.Trim();
                GetElementById("gasCardNo").InnerHtml = entity.cardinfo.cardNo.Trim();
                if (entity.cardinfo.cardType == "0")
                    GetElementById("gasCardType").InnerHtml = "工业卡";
                else if (entity.cardinfo.cardType == "1")
                    GetElementById("gasCardType").InnerHtml = "民用卡";
                else if (entity.cardinfo.cardType == "2")
                    GetElementById("gasCardType").InnerHtml = "商业卡";
                else
                    GetElementById("gasCardType").InnerHtml = "";

                if (entity.cardinfo.cardType == "1")
                {
                    //充气
                    GetElementById("gasCardNumsOrAmountTitle").InnerText = "购买气量：";
                    GetElementById("gasCardNumsOrAmount").InnerText = entity.buyNums.ToString() + "m³";
                }
                else if (entity.cardinfo.cardType == "0")
                {
                    GetElementById("gasCardNumsOrAmountTitle").InnerText = "购买金额：";
                    GetElementById("gasCardNumsOrAmount").InnerText = CommonData.Amount + "元";
                }
                else
                {
                    GetElementById("gasCardNumsOrAmountTitle").InnerText = "";
                }
                GetElementById("totalAmount").InnerHtml = CommonData.Amount + "元";
                //CommonData.BankCardNum = entity.signBankCardNo;


            }
            catch (Exception ex)
            {
                Log.Error("[BeingReadGasCardDeal][OnEnter] error ", ex);
            }
        }

        private void PayClick(object sender, HtmlElementEventArgs e)
        {
            //entity.isSign = false;
            //CommonData.Amount = 1;
            Log.Debug("sign card NO:" + entity.signBankCardNo);
            if (entity.isSign)
            {
                //已签约，直接扣款

                CommonData.BankCardNum = entity.signBankCardNo;

                //StartActivity("输入密码");
                //byte[] mpinkey = KeyManager.GetDePinKey(entity.SectionName);
                //string pin = "12345600";
                //byte[] EncrPin = Encrypt.DESEncrypt(Encoding.Default.GetBytes(pin), mpinkey);
                //CommonData.BankPassWord = Utility.bcd2str(EncrPin, EncrPin.Length);
                StartActivity("德化燃气正在交易");
            }
            else
            {
                //未签约，银行卡交易
                StartActivity("德化燃气插入银行卡");
            }
        }

        private void displayInfo()
        {
            GetElementById("gasCardNo").InnerHtml = entity.cardinfo.cardNo;
            GetElementById("gasCount").InnerHtml = entity.cardinfo.gasCount + "次";
            GetElementById("icNum").InnerHtml = entity.cardinfo.icNum + "次";
        }

        protected override void FrameReturnClick()
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

        //private void ReturnClick(object sender, HtmlElementEventArgs e)
        //{
        //    if (entity.cardinfo.cardType == "1")
        //    {
        //        //购气
        //        StartActivity("德化燃气购气选择");
        //    }
        //    else if (entity.cardinfo.cardType == "0")
        //    {
        //        StartActivity("德化燃气充值选择");
        //    }
        //    else
        //    {
        //        return;
        //    }
        //}

    }
}
