using DHRQPayment.Entity;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using Landi.FrameWorks.HardWare;
using System;
using System.Text;
using System.Windows.Forms;

namespace DHRQPayment.Business.DHRQPay
{
    class InputPasswordDeal : FrameActivity
    {
        private string pin;
        private string mId;
        private int mCount;
        private int mPinLength = 6;
        private bool mAllowBlank = true;
        private string mSectionName;
        private int mKeyIndex;
        private int mKeyLength;
        protected string Password;

        private const int INPUT_PASS = 1;
        private byte[] mPinKey;
        private DHRQPaymentEntity entity;

        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                entity = (GetBusinessEntity() as DHRQPaymentEntity);
                mSectionName = entity.SectionName;
                mId = InputId;
                GetElementById(mId).Focus();
                //if (Esam.IsUse)
                //    mPinKey = KeyManager.GetEnPinKey(mSectionName);
                //else
                setComponnents("ComComponnents", true, false, false);

                mPinKey = KeyManager.GetDePinKey(mSectionName);
                Log.Info("InputPass mPinkey:" + Utility.bcd2str(mPinKey, mPinKey.Length));
                //if (Esam.IsUse)
                //{
                //    Esam.SetWorkmode(Esam.WorkMode.Default);
                //    Esam.SetKeyLen(mKeyLength);
                //    Esam.SetMasterkeyNo(mKeyIndex);
                //    SendMessage(INPUT_PASS);
                //}
                //pin = GetElementById("pin").GetAttribute("value");

                //GetElementById("ok").Click += new HtmlElementEventHandler(OKClick);
            }
            catch (Exception ex)
            {
                Log.Error("[InputPasswordDeal][OnEnter] err", ex);
            }
        }

        private void OKClick(object sender, HtmlElementEventArgs e)
        {
            if (pin.Length < 6 && pin.Length != 0)
            {
                InvokeScript("showBankPassLenError");
            }
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    ReportSync("LengthEnough");
                    KeyPressed((char)keyCode);
                    break;
                case Keys.Enter:
                    if ((mCount > 0) && (mCount < mPinLength))
                    {
                        Password = "";
                        mCount = 0;
                        ReportSync("LengthNotEnough");
                    }
                    else if (mCount == 0)
                    {
                        if (mAllowBlank)
                        {
                            Finish();
                        }
                        else
                        {
                            Password = "";
                            ReportSync("LengthNotEnough");
                        }
                    }
                    else
                    {
                        Finish();
                    }
                    break;
                case Keys.Back:
                    BackSpace();
                    break;
                case Keys.Escape:
                    StartActivity("退卡");
                    //HandleResultInner(this, Result.Cancel);
                    break;
            }
        }

        private void BackSpace()
        {
            if (mCount > 0)
            {
                mCount--;
                ReportSync(null);
            }
        }

        protected sealed override void OnReport(object progress)
        {
            if ((string)progress == "LengthNotEnough")
            {
                GetElementById(mId).SetAttribute("value", "");
                OnErrorLength();
            }
            else if ((string)progress == "LengthEnough")
                OnClearNotice();
            else if (progress == null)
                GetElementById(mId).SetAttribute("value", GetElementById(mId).GetAttribute("value").Substring(0, mCount));
            else
                GetElementById(mId).SetAttribute("value", GetElementById(mId).GetAttribute("value") + (string)progress);
        }

        protected void OnErrorLength()
        {
            InvokeScript("showBankPassLenError");
        }

        protected string InputId
        {
            get { return "pin"; }
        }

        protected void OnClearNotice()
        {
            InvokeScript("hideBankPassLenError");
        }

        private void KeyPressed(char key)
        {
            if (mCount < mPinLength)
            {
                mCount++;
                //ReportSync("" + key);
            }
        }

        private void Finish()
        {
            pin = GetElementById(mId).GetAttribute("value").PadRight(8, '0');
            //pin = "84383500";
            byte[] EncrPin = Encrypt.DESEncrypt(Encoding.Default.GetBytes(pin), mPinKey);
            CommonData.BankPassWord = Utility.bcd2str(EncrPin, EncrPin.Length);
            //StartActivity("德化燃气正在交易");
            GotoNext();

        }

    }
}
