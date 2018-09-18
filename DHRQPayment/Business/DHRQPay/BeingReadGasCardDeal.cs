using DHRQPayment.Entity;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using Landi.FrameWorks.HardWare;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class BeingReadGasCardDeal : FrameActivity
    {
        private DHRQPaymentEntity entity;
        protected override void OnEnter()
        {
            try
            {
                base.OnEnter();
                entity = (GetBusinessEntity() as DHRQPaymentEntity);
                //GetElementById("btnReturn").Style = "display: none";
                //GetElementById("btnHome").Style = "display: none";
                setComponnents("ComComponnents", true, false, false);
                //ReportSync("BeingRead");
                GetElementById("Message1").InnerText = "正在查询，请稍后... ...";

                if (ReadCardDeal() == 0)
                {

                    if (int.Parse(entity.cardinfo.iccSpare) > 0)
                    {
                        ShowMessageAndGotoMain("卡内有气|请先插表使用");
                    }
                    else
                    {
#if DEBUG
                        GotoNext();
#else
                        StartActivity("德化燃气正在协议查询");
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("[BeingReadGasCardDeal][OnEnter] error ", ex);
            }
        }

        private int ReadCardDeal()
        {
            try
            {
#if DEBUG
                int resDebug = 0;
                entity.cardinfo.cardNo = "02063838";
                entity.cardinfo.cardType = "1";
                entity.cardinfo.gasCount = "1";
                entity.cardinfo.iccSpare = "0";
                entity.cardinfo.icErroy = "";
                entity.cardinfo.icNum = "1";
                return resDebug; 
#endif
                int res = GasCardReader.ReadGasCard(ref entity.cardinfo);
                //int res = 0;
                getDeMsg();
                if (res == 0)
                {
                    return 0;
                }
                else
                {
                    if (res == -1)
                    {
                        ShowMessageAndGoBack("读卡出错|程序出错");
                    }
                    else
                    {
                        string strRes = entity.GetGasCardReaderRes(res);
                        ShowMessageAndGoBack("读卡出错|错误码：" + res + "\r\n错误信息：" + strRes);
                    }
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Log.Error("[BeingReadGasCardDeal][ReadCardDeal] error ", ex);
                return -1;
            }
        }

        protected override void OnReport(object progress)
        {
            base.OnReport(progress);
            string msg = (string)progress;
            if (msg == "BeingRead")
            {
                GetElementById("Message1").InnerText = "正在读燃气卡，请稍后... ...";
            }
            else if (msg == "BeingQuery")
            {
                GetElementById("Message1").InnerText = "正在查询，请稍后... ...";
            }
        }

        private void getDeMsg()
        {
            string strDeCrypt = entity.cardinfo.strDeCrypt;
            Log.Info("strDeCrypt : " + strDeCrypt);
            if (entity.cardinfo.cardType == "1")
            {
                entity.deenCryptmsg.platformCode = strDeCrypt.Substring(14, 1);
                entity.deenCryptmsg.readCardType = strDeCrypt.Substring(15, 1);
                entity.deenCryptmsg.dllType = strDeCrypt.Substring(16, 1);
                entity.deenCryptmsg.dllVersion = strDeCrypt.Substring(17, 3);
                entity.deenCryptmsg.remainMsg = strDeCrypt.Substring(20, 2);
                entity.deenCryptmsg.getCardNums = strDeCrypt.Substring(22, 2);
                entity.deenCryptmsg.remainCardNums = strDeCrypt.Substring(24, 8);
                Log.Info("entity.deenCryptmsg.platformCode : " + entity.deenCryptmsg.platformCode);
            }
            else if (entity.cardinfo.cardType == "0")
            {
                entity.deenCryptmsg.readCardTime = strDeCrypt.Substring(48, 16);
            }
        }
    }
}
