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
    class BeingProtoQueryInfoDeal : FrameActivity
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
                if (QueryMsgDeal() == 0)
                {
                    if (entity.cardinfo.cardType == "0")
                    {
                        //天信 充值
                        if (!entity.isSign)
                        {
                            ShowMessageAndGotoMain("此卡未签约|请到前台签约");
                            return;
                        }
                    }
                    StartActivity("德化燃气正在明细查询");
                    //#region V1.5.1
                    //if (entity.cardinfo.cardType == "1")
                    //{
                    //    //购气
                    //    StartActivity("德化燃气购气选择");
                    //}
                    //else if (entity.cardinfo.cardType == "0")
                    //{
                    //    StartActivity("德化燃气充值选择");
                    //}

                    //#endregion
                }
                else
                {
                    ShowMessageAndGotoMain("协议查询失败|" + entity.returnCode + entity.returnMsg);
                }
            }
            catch (Exception ex)
            {
                Log.Error("[BeingQueryInfoDeal][OnEnter] error" + ex);
            }
        }

        private int QueryMsgDeal()
        {
            ProtoQuery query = new ProtoQuery();
            TransResult result = SyncTransaction(query);
            if (result == TransResult.E_SUCC)
            {
                return 0;
            }
            return -1;
        }

        //private bool IsRemindGas()
        //{
        //    bool res = false;
        //    entity.cardinfo.iccSpare.TrimStart('0');
        //    if()

        //    return res;
        //}
        
    }
}
