using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using Landi.FrameWorks.HardWare;

namespace DHRQPayment.Business.PFWLPay
{
    class CommonErrorDeal : MessageActivity
    {
        protected override void DoMessage(object message)
        {
            //GetElementById("ComComponnents").Style = "display:block";
            //GetElementById("btnReturn").Style = "display: block";
            //GetElementById("btnHome").Style = "display: block";
            setComponnents("ComComponnents", true, false, true);

            //GetElementById("btnReturn").Click += new HtmlElementEventHandler(ReturnClick);
            //GetElementById("btnHome").Click += new HtmlElementEventHandler(HomeClick);
            //CardReader.CardOut();

            Log.Info("cardRead out succ in err");
            string msgstr = (string)message;
            int index = msgstr.IndexOf('|');
            if (index > -1)
            {
                string[] messagestr = msgstr.Split(new char[] { '|' });

                if (messagestr.Length == 2)
                {
                    GetElementById("Message1").InnerText = messagestr[0];
                    GetElementById("Message2").InnerText = messagestr[1];
                }
            }
            else
            {
                GetElementById("Message1").InnerText = msgstr;
                GetElementById("Message2").InnerText = " ";
            }
            //GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        protected override void FrameReturnClick()
        {
            //CardReader.CardOut();

            GotoMain();
        }
    }
}
