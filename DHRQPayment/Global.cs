using Landi.FrameWorks.Package;
using System;
using System.Collections.Generic;
using System.Text;

namespace DHRQPayment
{
    public class Global
    {
        //DHRQ
        public static int ReadCardTypeFlag; //0----接触试读卡  1----非接读卡

        //private static NoPinAndSignParamData _noPinData;
        ///// <summary>
        ///// 免密免签参数值
        ///// </summary>
        //public static NoPinAndSignParamData gNoPinAndSignParamData
        //{
        //    get
        //    {
        //        if (_noPinData == null)
        //        {
        //            _noPinData = new NoPinAndSignParamData();
        //        }
        //        return _noPinData;
        //    }
        //}

        public static string TicketOrderNo;

    }
}
