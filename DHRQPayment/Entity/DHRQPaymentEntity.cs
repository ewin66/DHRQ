using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment.Entity
{
    class DHRQPaymentEntity : BaseEntity
    {
        #region 常量

        public const string SECTION_NAME = "DHRQPayment";

        #endregion

        public DHRQPaymentEntity(string businessName)
        {
            BusinessName = businessName;
        }

        public override string SectionName
        {
            get { return SECTION_NAME; }
        }

        private string OrderNo
        {
            get { return ReadIniFile("orderNo"); }
            set { WriteIniFile("orderNo", value); }
        }

        public string BusinessName = "";
        public cardInfo cardinfo = new cardInfo();
        public DeEnCryptMsg deenCryptmsg = new DeEnCryptMsg();
        public List<DetailInfo> detailinfolist = new List<DetailInfo>();

        public string GetGasCardReaderRes(int res)
        {
            string strRes = "";
            INIClass gasCardReaderIni = new INIClass(System.AppDomain.CurrentDomain.BaseDirectory + "GasCardReaderRes.ini");
            strRes = gasCardReaderIni.IniReadValue("GasCardReader", res.ToString());
            return strRes;
        }

        public string unitNo     //单位编号 初始为0000
        {
            get { return ReadIniFile("UntisNo"); }
        }
        public string unitMsg;
        public string Version
        {
            set { ConfigFile.WriteConfig("AppData", "Version", value); }
        }

        public string signBankCardNo;

        public double buyNums;      //购气量

        public string Amount;      //购买金额
        public bool isSign;         //是否签约标志 true---已签约 false---未签约
        public string returnMsg;   //返回44域信息
        public string returnCode;   //返回码39域信息
        public ReturnCardInfo returnCardInfo = new ReturnCardInfo();

//IC卡圈存申请返回信息（ 缴费通返回："ICDATA" + IC卡圈存申请返回信息）：公司代码|客户姓名|客户地址|购气单价|本次最大购气量|帐户余额|折扣类型（折扣类型：1全额折扣；2比例折扣；3赠送气量）|折扣率|折扣金额|赠送气量|
        public class ReturnCardInfo
        {
            public string companyCode;
            public string userName;
            public string userAddr;
            public string unitPrice;
            public string maxNums;
            public string accountRemind;
            public string discountType;
            public string discount;
            public string discountAmount;
            public string presentQue;
        }

        public class DeEnCryptMsg
        {
            public string platformCode;     //1位平台代码
            public string readCardType;     //1位读卡器类型
            public string dllType;          //1位动态库类型
            public string dllVersion;       //3位动态库版本
            public string remainMsg;        //2位卡备注信息（不足2位前补零）
            public string getCardNums;      //2位发卡次数信息（不足2位前补零）
            public string remainCardNums;   //8位购气量(不足8位前补零)
            public string readCardTime;     //天信读卡时间
        }

        public class DetailInfo
        {
            public string TransCode;          //购气编号
            public string CardNo;             //卡号
            public string CardRemark;         //卡备注
            public string IssuingCardTimes;   //发卡次数
            public string TransType;          //业务类型
            public string TransDate;          //购气日期
            public string TransTime;          //购气时间
            public string TransGasVolume;     //购气总量
            public string TransAmount;        //实交金额
            public string GasVolume1;         //一阶气量
            public string GasPrices1;         //一阶单价
            public string GasVolume2;         //二阶气量
            public string GasPrices2;         //二阶单价
            public string GasVolume3;         //三阶气量
            public string GasPrices3;         //三阶单价
            public string GasVolume4;         //四阶气量
            public string GasPrices4;         //四阶气价
        }
    }
}
