using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using DHRQPayment.Package;
using Landi.FrameWorks.HardWare;
using Landi.FrameWorks.Frameworks;

namespace DHRQPayment.Business
{
    class SignInDeal : FrameActivity
    {
        protected override void OnEnter()
        {
            bool succ = false;
            string businessName = GetBusinessName();
            if (businessName == "PFWLPaymentPay")
            {
                SyncTransaction(new CSignIn_DHRQPaymentPay());
                succ = DHRQPaymentPay.HasSignIn;
            }
            if (!succ)
            {
                ShowMessageAndGotoMain("ǩ��ʧ��|��ҵ����ʱ����ʹ��");
            }
            else
            {
                switch (GetBusinessName())
                {
                    //case "CreditCard":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //        StartActivity("���ÿ�������ܰ��ʾ");
                    //    else
                    //        StartActivity("���ÿ���ӡ�����ϼ���");
                    //    break;
                    //case "Mobile":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //        StartActivity("�ֻ���ֵ������");
                    //    else
                    //        StartActivity("�ֻ���ֵ��ӡ�����ϼ���");
                    //    break;
                    //case "YAPublishPay":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //    {
                    //        //StartActivity("�Ű�֧���˵�");
                    //        StartActivity("�Ű�֧�������û���");
                    //    }
                    //    else
                    //        StartActivity("�Ű�֧����ӡ�����ϼ���");
                    //    break;
                    //case "PowerPay":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //    {
                    //        StartActivity("����֧���˵�");
                    //    }
                    //    else
                    //        StartActivity("����֧����ӡ�����ϼ���");
                    //    break;

                    //case "YATrafficPolice":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //    {
                    //        StartActivity("�Ű�������û�˵�");
                    //    }
                    //    else
                    //        StartActivity("�Ű�������û��ӡ�����ϼ���");
                    //    break;

                    //case "PFWLPaymentPay":
                    //    if (ReceiptPrinter.CheckedByManager())
                    //    {
                    //        StartActivity("�Ű�������û�˵�");
                    //    }
                    //    else
                    //        StartActivity("�Ű�������û��ӡ�����ϼ���");
                    //    break;

                }
            }
        }
    }
}
