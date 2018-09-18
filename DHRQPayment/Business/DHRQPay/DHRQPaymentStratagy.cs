using DHRQPayment.Entity;
using Landi.FrameWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Business.DHRQPay
{
    class DHRQPaymentStratagy : BusinessStratagy
    {
        public override string BusinessName
        {
            get { return OptionName; }
        }

        public override string MessageActivity
        {
            get { return "德化燃气通用错误提示"; }
        }
        private string OptionName;

        public DHRQPaymentStratagy(string businessName)
        {
            OptionName = businessName;
        }

        public override BaseEntity BusinessEntity
        {
            get { return new DHRQPaymentEntity(BusinessName); }
        }

    }
}
