using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Landi.FrameWorks.Package
{
    public class NoPinAndSignParamData
    {
        /// <summary>
        /// 非接交易通道开关
        /// </summary>
        public string RfChannel { get; private set; }
        /// <summary>
        /// 闪卡单笔重刷处理时间
        /// </summary>
        public string QpRetryTime { get; private set; }
        /// <summary>
        /// 闪卡记录可处理时间
        /// </summary>
        public string QpOptionTime { get; private set; }
        /// <summary>
        /// 非接快速业务（QPS）免密限额
        /// </summary>
        public string QpsLimitAmt { get; private set; }
        /// <summary>
        /// 非接快速业务标识
        /// </summary>
        public string QpsIndentity { get; private set; }
        /// <summary>
        /// BIN表A标识
        /// </summary>
        public string BinAIndentity { get; private set; }
        /// <summary>
        /// BIN表B标识
        /// </summary>
        public string BinBIndentity { get; private set; }
        /// <summary>
        /// BIN表C标识
        /// </summary>
        public string BinCIndentity { get; private set; }
        /// <summary>
        /// CDCVM标识
        /// </summary>
        public string CdcvmIndentity { get; private set; }
        /// <summary>
        /// 免签限额
        /// </summary>
        public string NoSignLimitAMt { get; private set; }
        /// <summary>
        /// 免签标识
        /// </summary>
        public string NoSignIndentity { get; private set; }


        public void InstanseNoPinAndSignParam()
        {
            string temp = ReadFromFile("NoPinParam.txt");
            UnPacket(temp);
        }

        public void Add2CardbinB(string msg)
        {
            InstanceCardbin();
            add2cardbin(msg, _cardBinB);
        }
        
        public void Add2CardbinC(string msg)
        {
            InstanceCardbin();
            add2cardbin(msg, _cardBinC);
        }

        private string ReadFromFile(string filename)
        {
            InstanceCardbin();
            if (string.IsNullOrEmpty(filename))
                return "";
            try
            {
                StreamReader sr =
                    new StreamReader(
                        Path.Combine(Environment.CurrentDirectory,
                            filename), Encoding.Default);
                return sr.ReadToEnd();

            }
            catch (Exception ex)
            {
                Log.Error("read from file err", ex);
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ininfo"></param>
        /// <param name="data"></param>
        private void UnPacket(string ininfo)
        {
            int offset = 0;
            int len = ininfo.Length;
            while (offset < len - 6)
            {
                string tag = ininfo.Substring(offset, 6);
                try
                {
                    switch (tag)
                    {
                        case "FF805D":
                            offset += 6;
                            offset += 3;
                            RfChannel = ininfo.Substring(offset, 1);
                            offset += 1;
                            break;
                        case "FF803A":
                            offset += 6;
                            offset += 3;
                            QpRetryTime = ininfo.Substring(offset, 3);
                            offset += 3;
                            break;
                        case "FF803C":
                            offset += 6;
                            offset += 3;
                            QpOptionTime = ininfo.Substring(offset, 3);
                            offset += 3;
                            break;
                        case "FF8058":
                            offset += 6;
                            offset += 3;
                            QpsLimitAmt = ininfo.Substring(offset, 12);
                            offset += 12;
                            break;
                        case "FF8054":
                            offset += 6;
                            offset += 3;
                            QpsIndentity = ininfo.Substring(offset, 1);
                            offset += 1;
                            break;
                        case "FF8055":
                            offset += 6;
                            offset += 3;
                            BinAIndentity = ininfo.Substring(offset, 1);
                            offset += 1;
                            break;
                        case "FF8056":
                            offset += 6;
                            offset += 3;
                            BinBIndentity = ininfo.Substring(offset, 1);
                            offset += 1;
                            break;
                        case "FF8057":
                            offset += 6;
                            offset += 3;
                            CdcvmIndentity = ininfo.Substring(offset, 1);
                            offset += 1;
                            break;
                        case "FF8059":
                            offset += 6;
                            offset += 3;
                            NoSignLimitAMt = ininfo.Substring(offset, 12);
                            offset += 12;
                            break;
                        case "FF805A":
                            offset += 6;
                            offset += 3;
                            NoSignIndentity = ininfo.Substring(offset, 1);
                            offset += 1;
                            break;
                        default:
                            offset++;
                            break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Tag:" + tag + " Unpack Err," + e.Message);
                    throw;
                }
            }
        }

        private static Dictionary<string, List<int>> _cardBinA = new Dictionary<string, List<int>>();
        private static Dictionary<string, List<int>> _cardBinB = new Dictionary<string, List<int>>();
        private static Dictionary<string, List<int>> _cardBinC = new Dictionary<string, List<int>>();

        private static void InstanceCardbin()
        {
            #region CarbinA

            _cardBinA.Add("620060", new List<int> { 19 });
            _cardBinA.Add("620136", new List<int> { 16, 17, 18, 19 });
            _cardBinA.Add("620514", new List<int> { 16 });
            _cardBinA.Add("620525", new List<int> { 19 });
            _cardBinA.Add("620550", new List<int> { 19 });
            _cardBinA.Add("621080", new List<int> { 16 });
            _cardBinA.Add("621081", new List<int> { 19 });
            _cardBinA.Add("621082", new List<int> { 16 });
            _cardBinA.Add("621284", new List<int> { 19 });
            _cardBinA.Add("621286", new List<int> { 16 });
            _cardBinA.Add("621351", new List<int> { 16 });
            _cardBinA.Add("621352", new List<int> { 16 });
            _cardBinA.Add("621390", new List<int> { 16 });
            _cardBinA.Add("621420", new List<int> { 16 });
            _cardBinA.Add("621466", new List<int> { 16 });
            _cardBinA.Add("621467", new List<int> { 19 });
            _cardBinA.Add("621468", new List<int> { 16 });
            _cardBinA.Add("621483", new List<int> { 16 });
            _cardBinA.Add("621485", new List<int> { 16 });
            _cardBinA.Add("621486", new List<int> { 16 });
            _cardBinA.Add("621488", new List<int> { 16 });
            _cardBinA.Add("621499", new List<int> { 16 });
            _cardBinA.Add("621598", new List<int> { 19 });
            _cardBinA.Add("621621", new List<int> { 19 });
            _cardBinA.Add("621673", new List<int> { 19 });
            _cardBinA.Add("621700", new List<int> { 19 });
            _cardBinA.Add("621791", new List<int> { 16 });
            _cardBinA.Add("621792", new List<int> { 16 });
            _cardBinA.Add("621793", new List<int> { 16 });
            _cardBinA.Add("621795", new List<int> { 16 });
            _cardBinA.Add("621796", new List<int> { 16 });
            _cardBinA.Add("621797", new List<int> { 19 });
            _cardBinA.Add("621798", new List<int> { 19 });
            _cardBinA.Add("621799", new List<int> { 19 });
            _cardBinA.Add("622161", new List<int> { 16 });
            _cardBinA.Add("622168", new List<int> { 16 });
            _cardBinA.Add("622206", new List<int> { 16 });
            _cardBinA.Add("622210", new List<int> { 16 });
            _cardBinA.Add("622211", new List<int> { 16 });
            _cardBinA.Add("622212", new List<int> { 16 });
            _cardBinA.Add("622213", new List<int> { 16 });
            _cardBinA.Add("622214", new List<int> { 16 });
            _cardBinA.Add("622215", new List<int> { 16 });
            _cardBinA.Add("622220", new List<int> { 16 });
            _cardBinA.Add("622223", new List<int> { 16 });
            _cardBinA.Add("622224", new List<int> { 16 });
            _cardBinA.Add("622225", new List<int> { 16 });
            _cardBinA.Add("622229", new List<int> { 16 });
            _cardBinA.Add("622230", new List<int> { 16 });
            _cardBinA.Add("622231", new List<int> { 16 });
            _cardBinA.Add("622232", new List<int> { 16 });
            _cardBinA.Add("622233", new List<int> { 16 });
            _cardBinA.Add("622234", new List<int> { 16 });
            _cardBinA.Add("622235", new List<int> { 16 });
            _cardBinA.Add("622236", new List<int> { 16 });
            _cardBinA.Add("622237", new List<int> { 16 });
            _cardBinA.Add("622238", new List<int> { 16 });
            _cardBinA.Add("622239", new List<int> { 16 });
            _cardBinA.Add("622240", new List<int> { 16 });
            _cardBinA.Add("622245", new List<int> { 16 });
            _cardBinA.Add("622246", new List<int> { 16 });
            _cardBinA.Add("622252", new List<int> { 16 });
            _cardBinA.Add("622253", new List<int> { 16 });
            _cardBinA.Add("622260", new List<int> { 19 });
            _cardBinA.Add("622262", new List<int> { 19 });
            _cardBinA.Add("622280", new List<int> { 19 });
            _cardBinA.Add("622285", new List<int> { 16 });
            _cardBinA.Add("622516", new List<int> { 16 });
            _cardBinA.Add("622518", new List<int> { 16 });
            _cardBinA.Add("622521", new List<int> { 16 });
            _cardBinA.Add("622522", new List<int> { 16 });
            _cardBinA.Add("622523", new List<int> { 16 });
            _cardBinA.Add("622570", new List<int> { 16 });
            _cardBinA.Add("622575", new List<int> { 16 });
            _cardBinA.Add("622576", new List<int> { 16 });
            _cardBinA.Add("622577", new List<int> { 16 });
            _cardBinA.Add("622578", new List<int> { 16 });
            _cardBinA.Add("622581", new List<int> { 16 });
            _cardBinA.Add("622582", new List<int> { 16 });
            _cardBinA.Add("622588", new List<int> { 16 });
            _cardBinA.Add("622597", new List<int> { 16 });
            _cardBinA.Add("622599", new List<int> { 16 });
            _cardBinA.Add("622600", new List<int> { 16 });
            _cardBinA.Add("622601", new List<int> { 16 });
            _cardBinA.Add("622602", new List<int> { 16 });
            _cardBinA.Add("622609", new List<int> { 16 });
            _cardBinA.Add("622623", new List<int> { 16 });
            _cardBinA.Add("622650", new List<int> { 16 });
            _cardBinA.Add("622655", new List<int> { 16 });
            _cardBinA.Add("622656", new List<int> { 16 });
            _cardBinA.Add("622657", new List<int> { 16 });
            _cardBinA.Add("622658", new List<int> { 16 });
            _cardBinA.Add("622659", new List<int> { 16 });
            _cardBinA.Add("622680", new List<int> { 16 });
            _cardBinA.Add("622685", new List<int> { 16 });
            _cardBinA.Add("622687", new List<int> { 16 });
            _cardBinA.Add("622688", new List<int> { 16 });
            _cardBinA.Add("622689", new List<int> { 16 });
            _cardBinA.Add("622700", new List<int> { 19 });
            _cardBinA.Add("622752", new List<int> { 16 });
            _cardBinA.Add("622753", new List<int> { 16 });
            _cardBinA.Add("622759", new List<int> { 16 });
            _cardBinA.Add("622760", new List<int> { 16 });
            _cardBinA.Add("622761", new List<int> { 16 });
            _cardBinA.Add("622788", new List<int> { 16 });
            _cardBinA.Add("622801", new List<int> { 16 });
            _cardBinA.Add("622810", new List<int> { 16 });
            _cardBinA.Add("622811", new List<int> { 16 });
            _cardBinA.Add("622812", new List<int> { 16 });
            _cardBinA.Add("622889", new List<int> { 16 });
            _cardBinA.Add("622910", new List<int> { 16 });
            _cardBinA.Add("622916", new List<int> { 16 });
            _cardBinA.Add("622918", new List<int> { 16 });
            _cardBinA.Add("622919", new List<int> { 16 });
            _cardBinA.Add("622966", new List<int> { 16 });
            _cardBinA.Add("622988", new List<int> { 16 });
            _cardBinA.Add("623074", new List<int> { 16, 17, 18, 19 });
            _cardBinA.Add("623094", new List<int> { 19 });
            _cardBinA.Add("623111", new List<int> { 16 });
            _cardBinA.Add("623126", new List<int> { 16 });
            _cardBinA.Add("623136", new List<int> { 16 });
            _cardBinA.Add("623211", new List<int> { 19 });
            _cardBinA.Add("623251", new List<int> { 16 });
            _cardBinA.Add("623668", new List<int> { 19 });
            _cardBinA.Add("623698", new List<int> { 19 });
            _cardBinA.Add("623699", new List<int> { 19 });
            _cardBinA.Add("625017", new List<int> { 16 });
            _cardBinA.Add("625018", new List<int> { 16 });
            _cardBinA.Add("625019", new List<int> { 16 });
            _cardBinA.Add("625040", new List<int> { 16 });
            _cardBinA.Add("625042", new List<int> { 16 });
            _cardBinA.Add("625136", new List<int> { 16 });
            _cardBinA.Add("625160", new List<int> { 16 });
            _cardBinA.Add("625161", new List<int> { 16 });
            _cardBinA.Add("625162", new List<int> { 16 });
            _cardBinA.Add("625188", new List<int> { 16 });
            _cardBinA.Add("625330", new List<int> { 16 });
            _cardBinA.Add("625331", new List<int> { 16 });
            _cardBinA.Add("625332", new List<int> { 16 });
            _cardBinA.Add("625333", new List<int> { 16 });
            _cardBinA.Add("625337", new List<int> { 16 });
            _cardBinA.Add("625338", new List<int> { 16 });
            _cardBinA.Add("625339", new List<int> { 16 });
            _cardBinA.Add("625367", new List<int> { 16 });
            _cardBinA.Add("625368", new List<int> { 16 });
            _cardBinA.Add("625568", new List<int> { 16 });
            _cardBinA.Add("625650", new List<int> { 16 });
            _cardBinA.Add("625708", new List<int> { 16 });
            _cardBinA.Add("625709", new List<int> { 16 });
            _cardBinA.Add("625801", new List<int> { 16 });
            _cardBinA.Add("625802", new List<int> { 16 });
            _cardBinA.Add("625803", new List<int> { 16 });
            _cardBinA.Add("625824", new List<int> { 16, 17, 18, 19 });
            _cardBinA.Add("625834", new List<int> { 16 });
            _cardBinA.Add("625858", new List<int> { 16 });
            _cardBinA.Add("625859", new List<int> { 16 });
            _cardBinA.Add("625860", new List<int> { 16 });
            _cardBinA.Add("625865", new List<int> { 16 });
            _cardBinA.Add("625866", new List<int> { 16 });
            _cardBinA.Add("625899", new List<int> { 16 });
            _cardBinA.Add("625900", new List<int> { 16 });
            _cardBinA.Add("625905", new List<int> { 16 });
            _cardBinA.Add("625906", new List<int> { 16 });
            _cardBinA.Add("625907", new List<int> { 16 });
            _cardBinA.Add("625908", new List<int> { 16 });
            _cardBinA.Add("625909", new List<int> { 16 });
            _cardBinA.Add("625910", new List<int> { 16 });
            _cardBinA.Add("625911", new List<int> { 16 });
            _cardBinA.Add("625912", new List<int> { 16 });
            _cardBinA.Add("625913", new List<int> { 16 });
            _cardBinA.Add("625919", new List<int> { 16 });
            _cardBinA.Add("625941", new List<int> { 16 });
            _cardBinA.Add("625955", new List<int> { 16 });
            _cardBinA.Add("625956", new List<int> { 16 });
            _cardBinA.Add("625975", new List<int> { 16 });
            _cardBinA.Add("625976", new List<int> { 16 });
            _cardBinA.Add("625978", new List<int> { 16 });
            _cardBinA.Add("625979", new List<int> { 16 });
            _cardBinA.Add("625981", new List<int> { 16 });
            _cardBinA.Add("628201", new List<int> { 16 });
            _cardBinA.Add("628202", new List<int> { 16 });
            _cardBinA.Add("628206", new List<int> { 16 });
            _cardBinA.Add("628208", new List<int> { 16 });
            _cardBinA.Add("628209", new List<int> { 16 });
            _cardBinA.Add("628216", new List<int> { 16 });
            _cardBinA.Add("628218", new List<int> { 16 });
            _cardBinA.Add("628286", new List<int> { 16 });
            _cardBinA.Add("628288", new List<int> { 16 });
            _cardBinA.Add("628290", new List<int> { 16 });
            _cardBinA.Add("628310", new List<int> { 16 });
            _cardBinA.Add("628312", new List<int> { 16 });
            _cardBinA.Add("628313", new List<int> { 16 });
            _cardBinA.Add("628362", new List<int> { 16 });
            _cardBinA.Add("628370", new List<int> { 16 });
            _cardBinA.Add("628371", new List<int> { 16 });
            _cardBinA.Add("628372", new List<int> { 16 });
            _cardBinA.Add("628388", new List<int> { 16 });

            #endregion

            #region CardbinB

            _cardBinB.Add("601382", new List<int> { 19 });
            _cardBinB.Add("620060", new List<int> { 19 });
            _cardBinB.Add("620061", new List<int> { 19 });
            _cardBinB.Add("620136", new List<int> { 16, 17, 18, 19 });
            _cardBinB.Add("620525", new List<int> { 16, 17, 18, 19 });
            _cardBinB.Add("620550", new List<int> { 19 });
            _cardBinB.Add("621080", new List<int> { 16 });
            _cardBinB.Add("621081", new List<int> { 19 });
            _cardBinB.Add("621082", new List<int> { 16 });
            _cardBinB.Add("621283", new List<int> { 19 });
            _cardBinB.Add("621284", new List<int> { 19 });
            _cardBinB.Add("621286", new List<int> { 16 });
            _cardBinB.Add("621330", new List<int> { 19 });
            _cardBinB.Add("621331", new List<int> { 19 });
            _cardBinB.Add("621332", new List<int> { 19 });
            _cardBinB.Add("621333", new List<int> { 19 });
            _cardBinB.Add("621351", new List<int> { 16 });
            _cardBinB.Add("621352", new List<int> { 16 });
            _cardBinB.Add("621390", new List<int> { 16 });
            _cardBinB.Add("621420", new List<int> { 16 });
            _cardBinB.Add("621462", new List<int> { 19 });
            _cardBinB.Add("621466", new List<int> { 16 });
            _cardBinB.Add("621467", new List<int> { 19 });
            _cardBinB.Add("621468", new List<int> { 16 });
            _cardBinB.Add("621483", new List<int> { 16 });
            _cardBinB.Add("621485", new List<int> { 16 });
            _cardBinB.Add("621486", new List<int> { 16 });
            _cardBinB.Add("621488", new List<int> { 16 });
            _cardBinB.Add("621499", new List<int> { 16 });
            _cardBinB.Add("621568", new List<int> { 19 });
            _cardBinB.Add("621569", new List<int> { 19 });
            _cardBinB.Add("621598", new List<int> { 19 });
            _cardBinB.Add("621620", new List<int> { 19 });
            _cardBinB.Add("621621", new List<int> { 19 });
            _cardBinB.Add("621660", new List<int> { 19 });
            _cardBinB.Add("621661", new List<int> { 19 });
            _cardBinB.Add("621663", new List<int> { 19 });
            _cardBinB.Add("621665", new List<int> { 19 });
            _cardBinB.Add("621666", new List<int> { 19 });
            _cardBinB.Add("621668", new List<int> { 19 });
            _cardBinB.Add("621669", new List<int> { 19 });
            _cardBinB.Add("621672", new List<int> { 19 });
            _cardBinB.Add("621673", new List<int> { 19 });
            _cardBinB.Add("621700", new List<int> { 19 });
            _cardBinB.Add("621725", new List<int> { 19 });
            _cardBinB.Add("621756", new List<int> { 19 });
            _cardBinB.Add("621757", new List<int> { 19 });
            _cardBinB.Add("621758", new List<int> { 19 });
            _cardBinB.Add("621759", new List<int> { 19 });
            _cardBinB.Add("621785", new List<int> { 19 });
            _cardBinB.Add("621786", new List<int> { 19 });
            _cardBinB.Add("621787", new List<int> { 19 });
            _cardBinB.Add("621788", new List<int> { 19 });
            _cardBinB.Add("621789", new List<int> { 19 });
            _cardBinB.Add("621790", new List<int> { 19 });
            _cardBinB.Add("621791", new List<int> { 16 });
            _cardBinB.Add("621792", new List<int> { 16 });
            _cardBinB.Add("621793", new List<int> { 16 });
            _cardBinB.Add("621795", new List<int> { 16 });
            _cardBinB.Add("621796", new List<int> { 16 });
            _cardBinB.Add("621797", new List<int> { 19 });
            _cardBinB.Add("621798", new List<int> { 19 });
            _cardBinB.Add("621799", new List<int> { 19 });
            _cardBinB.Add("622260", new List<int> { 19 });
            _cardBinB.Add("622262", new List<int> { 19 });
            _cardBinB.Add("622280", new List<int> { 19 });
            _cardBinB.Add("622516", new List<int> { 16 });
            _cardBinB.Add("622518", new List<int> { 16 });
            _cardBinB.Add("622521", new List<int> { 16 });
            _cardBinB.Add("622522", new List<int> { 16 });
            _cardBinB.Add("622523", new List<int> { 16 });
            _cardBinB.Add("622568", new List<int> { 19 });
            _cardBinB.Add("622588", new List<int> { 16 });
            _cardBinB.Add("622609", new List<int> { 16 });
            _cardBinB.Add("622700", new List<int> { 19 });
            _cardBinB.Add("622966", new List<int> { 16 });
            _cardBinB.Add("622988", new List<int> { 16 });
            _cardBinB.Add("623074", new List<int> { 16, 17, 18, 19 });
            _cardBinB.Add("623094", new List<int> { 19 });
            _cardBinB.Add("623111", new List<int> { 16 });
            _cardBinB.Add("623126", new List<int> { 16 });
            _cardBinB.Add("623136", new List<int> { 16 });
            _cardBinB.Add("623184", new List<int> { 19 });
            _cardBinB.Add("623208", new List<int> { 19 });
            _cardBinB.Add("623211", new List<int> { 19 });
            _cardBinB.Add("623251", new List<int> { 16 });
            _cardBinB.Add("623506", new List<int> { 19 });
            _cardBinB.Add("623569", new List<int> { 19 });
            _cardBinB.Add("623571", new List<int> { 19 });
            _cardBinB.Add("623572", new List<int> { 19 });
            _cardBinB.Add("623573", new List<int> { 19 });
            _cardBinB.Add("623575", new List<int> { 19 });
            _cardBinB.Add("623586", new List<int> { 19 });
            _cardBinB.Add("623668", new List<int> { 19 });
            _cardBinB.Add("623698", new List<int> { 19 });
            _cardBinB.Add("623699", new List<int> { 19 });
            _cardBinB.Add("625955", new List<int> { 16 });
            _cardBinB.Add("625956", new List<int> { 16 });
            _cardBinB.Add("627000", new List<int> { 16 });
            _cardBinB.Add("627001", new List<int> { 16 });
            _cardBinB.Add("627002", new List<int> { 16 });
            _cardBinB.Add("627003", new List<int> { 16 });
            _cardBinB.Add("627004", new List<int> { 16 });
            _cardBinB.Add("627005", new List<int> { 16 });
            _cardBinB.Add("627006", new List<int> { 16 });
            _cardBinB.Add("627007", new List<int> { 16 });
            _cardBinB.Add("627008", new List<int> { 16 });
            _cardBinB.Add("627009", new List<int> { 16 });
            _cardBinB.Add("627010", new List<int> { 16 });
            _cardBinB.Add("627011", new List<int> { 16 });
            _cardBinB.Add("627012", new List<int> { 16 });
            _cardBinB.Add("627013", new List<int> { 16 });
            _cardBinB.Add("627014", new List<int> { 16 });
            _cardBinB.Add("627015", new List<int> { 16 });
            _cardBinB.Add("627016", new List<int> { 16 });
            _cardBinB.Add("627017", new List<int> { 16 });
            _cardBinB.Add("627018", new List<int> { 16 });
            _cardBinB.Add("627019", new List<int> { 16 });
            _cardBinB.Add("627020", new List<int> { 19 });
            _cardBinB.Add("627021", new List<int> { 19 });
            _cardBinB.Add("627022", new List<int> { 19 });
            _cardBinB.Add("627023", new List<int> { 19 });
            _cardBinB.Add("627024", new List<int> { 19 });
            _cardBinB.Add("627025", new List<int> { 19 });
            _cardBinB.Add("627026", new List<int> { 19 });
            _cardBinB.Add("627027", new List<int> { 19 });
            _cardBinB.Add("627028", new List<int> { 19 });
            _cardBinB.Add("627029", new List<int> { 19 });
            _cardBinB.Add("627030", new List<int> { 19 });
            _cardBinB.Add("627031", new List<int> { 19 });
            _cardBinB.Add("627032", new List<int> { 19 });
            _cardBinB.Add("627033", new List<int> { 19 });
            _cardBinB.Add("627034", new List<int> { 19 });
            _cardBinB.Add("627035", new List<int> { 19 });
            _cardBinB.Add("627036", new List<int> { 19 });
            _cardBinB.Add("627037", new List<int> { 19 });
            _cardBinB.Add("627038", new List<int> { 19 });
            _cardBinB.Add("627039", new List<int> { 19 });
            _cardBinB.Add("627040", new List<int> { 19 });
            _cardBinB.Add("627041", new List<int> { 19 });
            _cardBinB.Add("627042", new List<int> { 19 });
            _cardBinB.Add("627043", new List<int> { 19 });
            _cardBinB.Add("627044", new List<int> { 19 });
            _cardBinB.Add("627045", new List<int> { 19 });
            _cardBinB.Add("627046", new List<int> { 19 });
            _cardBinB.Add("627047", new List<int> { 19 });
            _cardBinB.Add("627048", new List<int> { 19 });
            _cardBinB.Add("627049", new List<int> { 19 });
            _cardBinB.Add("627050", new List<int> { 19 });
            _cardBinB.Add("627051", new List<int> { 19 });
            _cardBinB.Add("627052", new List<int> { 19 });
            _cardBinB.Add("627053", new List<int> { 19 });
            _cardBinB.Add("627054", new List<int> { 19 });
            _cardBinB.Add("627055", new List<int> { 19 });
            _cardBinB.Add("627056", new List<int> { 19 });
            _cardBinB.Add("627057", new List<int> { 19 });
            _cardBinB.Add("627058", new List<int> { 19 });
            _cardBinB.Add("627059", new List<int> { 19 });

            #endregion
        }

        private static void add2cardbin(string inMsg, Dictionary<string, List<int>> cardbin)
        {

            if (string.IsNullOrEmpty(inMsg))
                return;
            List<int> cardnolenlist = new List<int>();
            try
            {
                string cardno = inMsg.Substring(0, 6);
                int pos = 6;
                int cardnolen;
                while (pos < inMsg.Length - 1)
                {
                    cardnolen = int.Parse(inMsg.Substring(pos, 2));
                    pos += 2;
                    cardnolenlist.Add(cardnolen);
                }
                cardbin.Add(cardno, cardnolenlist);

            }
            catch (Exception ex)
            {
                Log.Error("add2cardbin err", ex);
            }
        }





        public bool IsContainInCardBinA(string bin)
        {
            string tempBin = bin.Substring(0, 6);
            bool result = _cardBinA.ContainsKey(tempBin) && _cardBinA[tempBin].Contains(bin.Length);
            return result;
        }

        public bool IsContainInCardBinB(string bin)
        {
            string tempBin = bin.Substring(0, 6);
            bool result = _cardBinB.ContainsKey(tempBin) && _cardBinB[tempBin].Contains(bin.Length);
            return result;
        }

        public bool IsContainInCardBinC(string bin)
        {
            string tempBin = bin.Substring(0, 6);
            bool result = _cardBinC.ContainsKey(tempBin) && _cardBinC[tempBin].Contains(bin.Length);
            return result;
        }

    }

}
