using DHRQPayment.Entity;
using DHRQPayment.Package.DHRQPay;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class BeingQueryInfoDeal : FrameActivity
    {
        private DHRQPaymentEntity entity;

        protected override void OnEnter()
        {
            try
            {
                base.OnEnter();
                //GetElementById("btnReturn").Style = "display: none";
                //GetElementById("btnHome").Style = "display: none";
                setComponnents("ComComponnents", true, false, false);

                GetElementById("Message1").InnerHtml = "正在查询，请稍后... ...";
                entity = (GetBusinessEntity() as DHRQPaymentEntity);

                //entity.cardinfo.cardNo = "00016016";
                //entity.cardinfo.gasCount = "3";
                //entity.cardinfo.icMark = "123";
                //entity.cardinfo.icNum = "123";
                //entity.cardinfo.strEnCrypt = "78E17879773516BE7F372369BE61E24EE2DC1576190AB6C5487E53DF2E3110F9E0639DF33E671624487E53DF2E3110F9BD612DF3FA28C3148700861A52D99C04";
                //entity.cardinfo.cardType = "0";
                //entity.buyNums = 30.2;
                if (QueryMsgDeal() == 0)
                {
                    StartActivity("德化燃气燃气卡信息显示");
                }
                else
                {
                    ShowMessageAndGotoMain("查询失败|" + entity.returnCode + entity.returnMsg);
                }
            }
            catch (Exception ex)
            {
                Log.Error("[BeingQueryInfoDeal][OnEnter] error" + ex);
            }
        }

        private int QueryMsgDeal()
        {
            QueryPay query = new QueryPay();
            TransResult result = SyncTransaction(query);
            if (result == TransResult.E_SUCC)
            {
                if (string.Compare(entity.cardinfo.cardType, "1") == 0)
                {
                    try
                    {
                        Log.Info("entity.Amount : " + entity.Amount);
#if DEBUG
                        entity.Amount = "1";               
#endif
                        CommonData.Amount = double.Parse(entity.Amount.TrimStart('0'));
                    }
                    catch (Exception ex)
                    {
                        ShowMessageAndGotoMain("查询失败|返回金额格式出错");
                        return -1;
                    }
                }
                return 0;
            }
            else
            {
                ShowMessageAndGotoMain("查询失败|");
            }

            return -1;
        }
        
    }
}
