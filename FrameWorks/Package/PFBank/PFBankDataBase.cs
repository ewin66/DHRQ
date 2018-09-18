using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Landi.FrameWorks.Package.PFBank
{
    /// <summary>
    ///     发送json类型基类
    /// </summary>
    public class BaseCommunicateSendJson
    {
        public int rowCount;
        public Info[] DS;
    }

    public class Info
    {
    }


    /// <summary>
    /// 通用json接收类型(类型和字符串必须严格匹配)
    /// </summary>
    public class CommunicateInfo
    {
        public WlInfo[] DS;
        public int rowCount;
        public string returnMessage;
        public string ReturnCode;
        public string ReturnMsg;
        public string recordcount;
        public string PageCount;
        public string PageCurrent;
    }

    public class WlInfo : Info
    {
        [JsonProperty("寄件日期")]
        public string SendDate;
        [JsonProperty("目的地")]
        public string Destination;
        [JsonProperty("收件人")]
        public string Recipient;
        [JsonProperty("运单号")]
        public string WayBill;
        [JsonProperty("运单号校验码")]
        public string WayBillCode;
        [JsonProperty("货号")]
        public string ArticleNum;
        [JsonProperty("代收金额")]
        public string CollectAmt;
        [JsonProperty("代扣运费")]
        public string ShipWithholding;
        [JsonProperty("手续费")]
        public string Fee;
        [JsonProperty("实际应付")]
        public string ActualCope;
        [JsonProperty("实际支付")]
        public string ActualCopeHis;
        [JsonProperty("实际应付校验码")]
        public string ActualCopeCode;
        [JsonProperty("账号户名")]
        public string AccountName;
        [JsonProperty("银行")]
        public string Bank;
        [JsonProperty("卡号及方式")]
        public string BankCardNoAndPayMethod;
        [JsonProperty("支付日期")]
        public string PayDate;
    }
}
