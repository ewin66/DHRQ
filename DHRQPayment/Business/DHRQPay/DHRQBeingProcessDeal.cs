using DHRQPayment.Entity;
using DHRQPayment.Package;
using DHRQPayment.Package.DHRQPay;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using Landi.FrameWorks.HardWare;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Business.DHRQPay
{
    class DHRQBeingProcessDeal : FrameActivity
    {
        
        private EMVTransProcess emv = new EMVTransProcess();
        private bool bemvInit;
        private bool bisICCard;
        private bool bisQPBOCCard;

        private DHRQPaymentEntity entity;

        protected override void OnEnter()
        {
            base.OnEnter();
            DestroySelf();//设置成自动销毁，每次重新生成
            bemvInit = false;
            bisICCard = false;
            entity = (GetBusinessEntity() as DHRQPaymentEntity);
            //if (SyncTransaction(new CReverser_DHRQPaymentPay()) == TransResult.E_RECV_FAIL)
            //{
            //    ShowMessageAndGotoMain("交易失败！|交易超时，请重试");
            //    return;
            //}

            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bisICCard = true;

            entity.SendField55 = null;
            setComponnents("ComComponnents", true, false, false);

            if (bisICCard)//如果是IC卡，或是复合卡
            {

                if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                {
                    emv.PayType = 0;
                }
                PostSync(EMVProcess);
                if (!bemvInit)
                {
                    ShowMessageAndGotoMain("交易失败！|IC卡初始化失败，请重试");
                    return;
                }
            }
            PayProcess();
        }


        private void EMVProcess()
        {
            //传入支付金额
            int state = emv.EMVTransInit(CommonData.Amount, EMVTransProcess.PbocTransType.PURCHASE);
            if (state == 0)
            {
                if (emv.EMVTransDeal() == 0)
                {
                    CommonData.BankCardNum = emv.EMVInfo.CardNum;
                    CommonData.BankCardSeqNum = emv.EMVInfo.CardSeqNum.PadLeft(3, '0');
                    CommonData.BankCardExpDate = emv.EMVInfo.CardExpDate;
                    Log.Debug("common track2 : " + CommonData.Track2);
                    CommonData.Track2 = emv.EMVInfo.Track2;
                    Log.Debug("common track2 : " + CommonData.Track2);
                    entity.SendField55 = emv.EMVInfo.SendField55;
                    bemvInit = true;
                }
            }
        }


        private void PayProcess()
        {
            QueryBalancePay query = new QueryBalancePay();
            DHRQTranspay pay = new DHRQTranspay();
            TransResult res = TransResult.E_INVALID;
            if (!entity.isSign)
            {
                res = SyncTransaction(query);
            }
            if (res == TransResult.E_SUCC || entity.isSign)
            {
                TransResult result = SyncTransaction(pay);
                //CReverser_DHRQPaymentPay rev = new CReverser_DHRQPaymentPay(pay);
                //ReportSync("BeingPay");
                if (result == TransResult.E_SUCC)
                {
                    if (bisICCard)
                    {
                        int state = emv.EMVTransEnd(entity.RecvField55, entity.RecvField38);
                        //if (state != 0)
                        //{
                        //    //rev.Reason = "06";
                        //    //SyncTransaction(rev);
                        //    ShowMessageAndGotoMain("交易失败！|IC确认错误，交易失败，请重试");
                        //    return;
                        //}
                    }
                    StartActivity("德化燃气正在写燃气卡");
                }
                else if (result == TransResult.E_HOST_FAIL)
                {
                    if (pay.ReturnCode == "1370" || pay.ReturnCode == "0370")
                        //ShowMessageAndGotoMain("交易失败！|您卡内余额不足！");
                        StartActivity("德化燃气余额不足提示");
                    else
                    {
                        ShowMessageAndGotoMain("交易失败|" + entity.returnCode + entity.returnMsg);

                    }
                }
                else if (result == TransResult.E_RECV_FAIL)
                {
                    //rev.Reason = "98";
                    //SyncTransaction(rev);
                    ShowMessageAndGotoMain("交易失败！|交易超时，请重试");
                    return;
                }
                else if (result == TransResult.E_CHECK_FAIL)
                {
                    //rev.Reason = "96";
                    //SyncTransaction(rev);
                    ShowMessageAndGotoMain("交易失败！|系统异常，请稍后再试");
                    return;
                }
                else
                {
                    ShowMessageAndGotoMain("交易失败|请重试");
                }
            }
            else
            {
                Log.Error("[DHRQBeingProcessDeal][PayProcess] err == query fail");
                ShowMessageAndGotoMain("交易失败|查询余额失败");
            }
            //rev.ClearReverseFile();//在不发冲正文件的情况下，才清除冲正文件
        }

        protected override void OnReport(object progress)
        {
            base.OnReport(progress);
            string msg = (string)progress;
            if (msg == "BeingPay")
            {
                GetElementById("pin").SetAttribute("src", "../images/beingpay.gif");
                GetElementById("Message").InnerText = "正在扣款...";
            }
            else if (msg == "BeingPrint")
            {
                GetElementById("pin").SetAttribute("src", "../images/beingprint.gif");
                GetElementById("Message").InnerText = "正在打印...";
            }
        }

    }
}
