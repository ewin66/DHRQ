using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace DHRQPayment
{
    public class ParamInfo
    {
        /// <summary>
        /// 交易电话号码1
        /// </summary>
        public string Tel1 { get; set; }
        /// <summary>
        /// 交易电话号码2
        /// </summary>
        public string Tel2 { get; set; }
        /// <summary>
        /// 交易电话号码3
        /// </summary>
        public string Tel3 { get; set; }
        /// <summary>
        /// 商户名称（中文）
        /// </summary>
        public string MerchantName { get; set; }
        /// <summary>
        /// 终端号
        /// </summary>
        public string TerminalNo { get; set; }
        /// <summary>
        /// 商户名称（英文）
        /// </summary>
        public string MerchantNameE { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string MerchantNo { get; set; }
        /// <summary>
        /// 转出帐号1（默认采用）
        /// </summary>
        public string BankCardNo1 { get; set; }
        /// <summary>
        /// 转出帐号2
        /// </summary>
        public string BankCardNo2 { get; set; }
        /// <summary>
        /// 转出帐号3
        /// </summary>
        public string BankCardNo3 { get; set; }
        /// <summary>
        /// 转账交易限额
        /// </summary>
        public string TransLimit { get; set; }
        /// <summary>
        /// 消费交易限额
        /// </summary>
        public string PayLimit { get; set; }
        /// <summary>
        /// 转出卡1密码
        /// </summary>
        public byte[] Password1 { get; set; }
        /// <summary>
        /// 转出卡2密码
        /// </summary>
        public byte[] Password2 { get; set; }
        /// <summary>
        /// 转出卡3密码
        /// </summary>
        public byte[] Password3 { get; set; }
        /// <summary>
        /// 支持的类型 8字节（64位）
        /// </summary>
        public string SupportType { get; set; }

    }
}
