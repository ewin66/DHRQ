using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Landi.FrameWorks;
using CQYBYPayment.Package;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using CQYBYPayment.Entity;

namespace CQYBYPayment
{
    public class TransAccessFactory : DataAccess
    {
        private BaseEntity _entity = new PFWLPaymentEntity();

        public string PayTraceNo = "";

        /// <summary>
        //  保存密钥
        /// </summary>
        /// <param name="responseInfo"></param>
        public void InsertKeyParam()
        {
            DeleteKeyParam();
            //string strSql = String.Format(@"INSERT INTO Param ([Tel1],[Tel2],[Tel3],[MerchantName],[TerminalNo],[MerchantNameE],[MerchantNo],[BankCardNO1],[BankCardNO2],[BankCardNO3],[TransLimit],[PayLimit],[SupportType]) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", info.Tel1, info.Tel2, info.Tel3, info.MerchantName, info.TerminalNo, info.MerchantNameE, info.MerchantNo, info.BankCardNo1, info.BankCardNo2, info.BankCardNo3, info.TransLimit, info.PayLimit, info.SupportType);
            string strSql = "";
            string fristKey = Global.FirstKey;
            string mainKey = EncryptString(Global.MainKey, Skey);
            strSql = String.Format(@"INSERT INTO KeyParam ([MainKey],[FirstKey]) VALUES('{0}','{1}')", mainKey, fristKey);

            Log.Debug("SQL:" + strSql);
            ExecuteCommand(strSql);
        }
        /// <summary>
        /// 删除密钥记录记录
        /// </summary>
        public void DeleteKeyParam()
        {
            string strSql = @"DELETE FROM KeyParam";
            int iRet = ExecuteCommand(strSql);
        }

        public string GetKeyParam(string KeyMode)
        {
            string key = "";
            string mainKey = "";
            string firstKey = "";

            string strSql = string.Format(@"SELECT MainKey,FirstKey FROM KeyParam");
            DataTable table = GetDataTable(strSql);
            if (table == null || table.Rows.Count == 0)
                return null;
            DataRow dr = table.Rows[0];
            mainKey = GetString(dr["MainKey"]);
            firstKey = GetString(dr["FirstKey"]);
            if (string.Compare(KeyMode, "mainKey") == 0)
            {
                key = DecryptString(mainKey, Skey);
            }
            else if (string.Compare(KeyMode, "firstKey") == 0)
            {
                key = firstKey;
            }
            
            return key;
        }



        /// <summary>
        //  保存交易参数下载（仅有一笔数据，插入前删除旧资料）
        /// </summary>
        /// <param name="responseInfo"></param>
        public void InsertParam(ParamInfo info)
        {
            DeleteParam();
            //string strSql = String.Format(@"INSERT INTO Param ([Tel1],[Tel2],[Tel3],[MerchantName],[TerminalNo],[MerchantNameE],[MerchantNo],[BankCardNO1],[BankCardNO2],[BankCardNO3],[TransLimit],[PayLimit],[SupportType]) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", info.Tel1, info.Tel2, info.Tel3, info.MerchantName, info.TerminalNo, info.MerchantNameE, info.MerchantNo, info.BankCardNo1, info.BankCardNo2, info.BankCardNo3, info.TransLimit, info.PayLimit, info.SupportType);
            string strSql = String.Format(@"INSERT INTO Param ([BankCardNO1]) VALUES('{0}')", info.BankCardNo1);
            Log.Debug("SQL:" + strSql);
            ExecuteCommand(strSql);
        }

        /// <summary>
        /// 删除交易记录
        /// </summary>
        public void DeleteParam()
        {
            string strSql = @"DELETE FROM Param";
            int iRet = ExecuteCommand(strSql);
        }

        public ParamInfo GetParamData()
        {
            ParamInfo info = new ParamInfo();
            //string strSql = string.Format(@"SELECT Tel1,Tel2,Tel3,MerchantName,TerminalNo,MerchantNameE,MerchantNo,BankCardNO1,BankCardNO2,BankCardNO3,TransLimit,PayLimit,(select Password from Pass where BankCardNo=BankCardNO1) as pass1,(select Password from Pass where BankCardNo=BankCardNO2) as pass2,(select Password from Pass where BankCardNo=BankCardNO3) as pass3 FROM Param");
            string strSql = string.Format(@"SELECT Tel1,Tel2,Tel3,MerchantName,TerminalNo,MerchantNameE,MerchantNo,BankCardNO1,BankCardNO2,BankCardNO3,TransLimit,PayLimit FROM Param");
            DataTable table= GetDataTable(strSql);
            if (table == null || table.Rows.Count == 0)
                return null;
            DataRow dr = table.Rows[0];
            info.Tel1 = GetString(dr["Tel1"]);
            info.Tel2 = GetString(dr["Tel2"]);
            info.Tel3 = GetString(dr["Tel3"]);
            info.MerchantName = GetString(dr["MerchantName"]);
            info.TerminalNo = GetString(dr["TerminalNo"]);
            info.MerchantNameE = GetString(dr["MerchantNameE"]);
            info.MerchantNo = GetString(dr["MerchantNo"]);
            info.BankCardNo1 = GetString(dr["BankCardNO1"]);
            info.BankCardNo2 = GetString(dr["BankCardNO2"]);
            info.BankCardNo3 = GetString(dr["BankCardNO3"]);
            info.TransLimit = GetString(dr["TransLimit"]);
            info.PayLimit = GetString(dr["PayLimit"]);
            //info.Password1 = DecryptString(GetString(dr["pass1"]),Skey);
            //info.Password2 = DecryptString(GetString(dr["pass2"]),Skey);
            //info.Password3 = DecryptString(GetString(dr["pass3"]),Skey);
            return info;
        }

        public void InsertPassword(string bankCardNo, string password)
        {
            string strSql = string.Format(@"Delete from Pass where BankCardNo='{0}'", bankCardNo);
            ExecuteCommand(strSql);

            password = EncryptString(password, Skey);
            strSql = string.Format(@"Insert into Pass ([BankCardNo],[Password]) VALUES ('{0}','{1}')",bankCardNo,password);
            ExecuteCommand(strSql);
        }

        private string GetString(object obj)
        {
            if (obj == null)
            {
                return "";
            }
            else
            {
               return obj.ToString();
            }
        }

        private string Skey
        {
            get
            {
                string skey = _entity.AccessPin;
                if (skey.Length > 8)
                    skey = skey.Substring(0, 8);
                else
                    skey = skey.PadRight(8,'L');
                return skey;
            }
        }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="ConfigString"></param>
        /// <returns></returns>
        public override string GetConnectionString(string ConfigString)
        {
            string dbPath = Path.Combine(Application.StartupPath, _entity.AccessFile);
            string dbPsd = _entity.AccessPin;
            string strConn = String.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Database Password={1}", dbPath, dbPsd);
            return strConn;
        }
        /// <summary>
        /// 获取数据库驱动
        /// </summary>
        /// <param name="ConfigString"></param>
        /// <returns></returns>
        public override string GetProviderName(string ConfigString)
        {
            string ProviderName = _entity.AccessProviderName;
            return ProviderName;
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string EncryptString(string input, string sKey)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            byte[] data = Encoding.UTF8.GetBytes(input);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                ICryptoTransform desencrypt = des.CreateEncryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return BitConverter.ToString(result);
            }
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string DecryptString(string input, string sKey)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            string[] sInput = input.Split("-".ToCharArray());
            byte[] data = new byte[sInput.Length];
            for (int i = 0; i < sInput.Length; i++)
            {
                data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
            }
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                ICryptoTransform desencrypt = des.CreateDecryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(result);
            }
        }
    }
}
