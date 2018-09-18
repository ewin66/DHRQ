using DHRQPayment.Entity;
using DHRQPayment.Package;
using Landi.FrameWorks;
using Landi.FrameWorks.Frameworks;
using Landi.FrameWorks.HardWare;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Business.DHRQPay
{
    class BeingWriteGasCardDeal : FrameActivity
    {
        private DHRQPaymentEntity entity;

        protected override void OnEnter()
        {
            base.OnEnter();
            entity = (GetBusinessEntity() as DHRQPaymentEntity);
            //GetElementById("btnReturn").Style = "display: none";
            //GetElementById("btnHome").Style = "display: none";
            setComponnents("ComComponnents", true, false, false);

            GetElementById("Message1").InnerText = "正在写燃气卡，请稍后... ...";
            int res = WriteCardDeal();
            if (res == 0)
            {
                //写卡成功，打印凭条
                if (ReceiptPrinter.ExistError())
                    StartActivity("德化燃气成功界面");
                    //StartActivity("德化燃气正在写燃气卡");
                else
                    StartActivity("德化燃气正在打印");

                //StartActivity("正在打印");
            }
            else
            {
                //写卡出错，发起冲正
                //if (SyncTransaction(new CReverser_DHRQPaymentPay()) == TransResult.E_RECV_FAIL)
                //{
                //    ShowMessageAndGotoMain("交易失败！|交易超时，请重试");
                //    return;
                //}
                if (res == -1)
                {
                    Log.Error("写卡出错|程序出错");
                }
                else
                {
                    string strRes = entity.GetGasCardReaderRes(res);
                    Log.Error("写卡出错|" + strRes);
                }
                ShowMessageAndGotoMain("写卡出错|请联系燃气公司客服人员");
            }
        }

        private int WriteCardDeal()
        {
            int res = -1;
#if DEBUG
            return res;
#else
            try
            {
                //string icId = text_w_icid.Text;
                //string icMark = text_w_icmark.Text.PadLeft(2, '0');
                //string icNum = text_w_icnum.Text.PadLeft(2, '0');
                //string icGas = text_w_gas.Text;

                //string datetime = text_w_datetime.Text;
                //string temp = String.Format("{0}{1}{2}{3}{4}{5}{6}", icId.PadRight(20, 'F'), icMark, icNum,
                //                            icGas.PadLeft(8, '0'), dateB, timeB, datetime);

                //byte[] sPassWord = Encoding.ASCII.GetBytes(text_w_password.Text);
                //int iGasCount = int.Parse(text_w_gascount.Text);
                //int iIcErroy = -1;
                //string sErrMsg = null;
                int icGas = int.Parse(entity.cardinfo.gasCount) + 1;
                entity.cardinfo.gasCount = icGas.ToString();

                DateTime dt = DateTime.Now;
                string dateB = String.Format("{0}{1}{2}", dt.Year, dt.Month.ToString().PadLeft(2, '0'), dt.Day.ToString().PadLeft(2, '0'));
                string timeB = String.Format("00{0}{1}{2}", dt.Hour.ToString().PadLeft(2, '0'), dt.Minute.ToString().PadLeft(2, '0'), dt.Second.ToString().PadLeft(2, '0'));
                if (entity.cardinfo.cardType == "0")
                {
                    //天信 充钱

                    //输入金额格式待讨论 目前30.1格式为00030100

                    string temp = String.Format("{0}{1}{2}{3}{4}{5}{6}", entity.cardinfo.cardNo.PadRight(20, 'F'), entity.cardinfo.icMark.PadLeft(2, '0'), entity.cardinfo.icNum.PadLeft(2, '0'), (entity.Amount).ToString().PadLeft(8, '0'), dateB, timeB, entity.deenCryptmsg.readCardTime);
                    entity.cardinfo.strDeCrypt = temp;
                }
                else
                {
                    //苍南 购气
                    //14位IC卡号(不足14位后加F补齐)+1位平台代码+1位读卡器类型+1位动态库类型+3位动态库版本+2位卡备注信息（不足2位补零）+2位发卡次数信息（不足2位前补零）+ 8位卡内余量(不足8位前补零)+8位表内余量(不足8位前补零)+8位总用气量(不足8位前补零)+8位当前日期A(YYYYMMDD)+8位当前时间A(00HHMMSS)

                    Log.Info("entity.deenCryptmsg.platformCode : " + entity.deenCryptmsg.platformCode);

                    string temp = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", entity.cardinfo.cardNo.PadRight(14, 'F'), entity.deenCryptmsg.platformCode, entity.deenCryptmsg.readCardType, entity.deenCryptmsg.dllType, entity.deenCryptmsg.dllVersion, entity.deenCryptmsg.remainMsg.PadLeft(2, '0'), entity.deenCryptmsg.getCardNums.PadLeft(2, '0'), entity.buyNums.ToString().PadLeft(8, '0'), "".PadLeft(16, '0'), dateB, timeB);
                    entity.cardinfo.strDeCrypt = temp;

                }

                Log.Info("write DeCrypt: " + entity.cardinfo.strDeCrypt);
                res = GasCardReader.WriteCard(entity.cardinfo, int.Parse(entity.cardinfo.cardType));
                if (res != 0)
                {
                    icGas = int.Parse(entity.cardinfo.gasCount) - 1;
                    entity.cardinfo.gasCount = icGas.ToString();
                    throw new Exception("GasCardReader.WriteCard err res : " + res);
                }
            }
            catch(Exception ex)
            {
                Log.Error("[BeingWriteGasCardDeal][WriteCardDeal]err", ex);
            }
            return res;
#endif

        }

    }
}
