using DHRQPayment.Entity;
using DHRQPayment.Package.DHRQPay;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Business.DHRQPay
{
    class BeingQueryDetailMsgDeal : FrameActivity
    {
        private DHRQPaymentEntity entity;

        protected override void OnEnter()
        {
            try
            {
                base.OnEnter();
                setComponnents("ComComponnents", true, false, false);

                GetElementById("Message1").InnerHtml = "正在查询，请稍后... ...";
                entity = (GetBusinessEntity() as DHRQPaymentEntity);
                if (QueryMsg() == 0)
                {
                    StartActivity("德化燃气明细信息显示");
                }
                else
                {
                    ShowMessageAndGotoMain("查询明细失败！|" + entity.returnCode + entity.returnMsg);
                }
            }
            catch (Exception ex)
            {
                Log.Error("[BeingQueryDetailMsgDeal][OnEnter] error" + ex);
            }
        }

        private int QueryMsg()
        {
            int ret = 0;
            try
            {
                DetailQueryPay query = new DetailQueryPay();
                TransResult result = SyncTransaction(query);
                if (result == TransResult.E_SUCC)
                    ret = 0;
                else
                    ret = -1;

            }
            catch (Exception ex)
            {
                Log.Error("[BeingQueryDetailMsgDeal][QueryMsg] err" + ex);
            }
            return ret;
        }
    }
}
