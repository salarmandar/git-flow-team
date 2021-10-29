using Bgt.Ocean.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

namespace Bgt.Ocean.Repository.EntityFramework
{
    public abstract class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext>
      where TDbContext : DbContext
    {
        private readonly IDbFactory<TDbContext> _dbFactory;
        protected UnitOfWork(IDbFactory<TDbContext> dbFactory)
        {
            _dbFactory = dbFactory;

#if DEBUG
            DbContext.Database.Log = msg => Debug.WriteLine(msg);
#endif
        }

        public TDbContext DbContext
        {
            get
            {
                return _dbFactory.GetCurrentDbContext;
            }
        }

        public void Commit()
        {
#if DEBUG
            DbContext.Database.Log = msg => Debug.WriteLine(msg);
#endif
            try
            {
                DbContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                string innerError = "";
                foreach (var eve in ex.EntityValidationErrors)
                {
                    innerError = $"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:";
                    foreach (var ve in eve.ValidationErrors)
                    {
                        innerError += $"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"";
                    }
                }

                using (var db = new Bgt.Ocean.Models.OceanDbEntities())
                {
                    string clientName = Infrastructure.Helpers.SystemHelper.ClientHostName;
                    Bgt.Ocean.Models.TblSystemLog_HistoryError newError = new Bgt.Ocean.Models.TblSystemLog_HistoryError();
                    newError.Guid = Guid.NewGuid();
                    newError.ErrorDescription = ex.Message;
                    newError.FunctionName = ex.TargetSite == null ? "" : ex.TargetSite.Name;
                    newError.PageName = Bgt.Ocean.Infrastructure.Helpers.SystemHelper.CurrentPageUri;
                    newError.InnerError = innerError;
                    newError.ClientIP = Bgt.Ocean.Infrastructure.Helpers.SystemHelper.CurrentIpAddress;
                    newError.ClientName = clientName;
                    newError.DatetimeCreated = DateTime.UtcNow;
                    newError.FlagSendEmail = false;
                    db.TblSystemLog_HistoryError.Add(newError);
                    db.SaveChanges();
                }
                throw;
            }
        }

        public async Task CommitAsync()
        {
            await DbContext.SaveChangesAsync();
        }

        public TransactionScope BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            var option = new TransactionOptions();
            option.IsolationLevel = isolationLevel;
            option.Timeout = TimeSpan.FromMinutes(5);

            return new TransactionScope(TransactionScopeOption.Suppress, option);
        }

        /// <summary>
        /// For more speed when Insert many data set to Enable = false before.
        /// </summary>
        /// <param name="isEnable"></param>
        public void ConfigAutoDetectChanges(bool isEnable)
        {
            DbContext.Configuration.AutoDetectChangesEnabled = isEnable;
        }

        public string GetConnectionString()
        {
            return DbContext.Database.Connection.ConnectionString;
        }

        #region SQL-Logging
        private Dictionary<string, string> dic = new Dictionary<string, string>();
        private readonly Dictionary<string, string> dicTable = new Dictionary<string, string>();
        /// <summary>
        ///  0 = no log;
        ///  1 = insert;
        ///  2 = update;
        ///  3 = store;
        /// </summary>
        private int _logtype { get; set; } = 0;
        private void insertLog(string msg, string[] txt)
        {
            if (msg.StartsWith("INSERT"))
            {
                _logtype = 1;

                dic.Add("#INSERT", txt[0]);

                var val = @"\[(.*?)\]";
                var getVal = Regex.Matches(txt[0], val);
                var v = getVal[1].Groups[1] + "";

                dic.Add("#TB", v);

                if (txt[1].StartsWith("VALUES"))
                {
                    dic.Add("#VALUES", txt[1]);
                }
            }
            if (_logtype == 1)
            {
                if (msg.StartsWith("-- @"))
                {
                    var Ntype = msg.Contains("Type = Guid") ? "N" : string.Empty;

                    var val = @"\'(.*?)\'";
                    var key = @"\@(.*?)\:";
                    var getVal = Regex.Matches(msg, val);
                    var getKey = Regex.Matches(msg, key);
                    var k = "@" + getKey[0].Groups[1];
                    var v = "" + Ntype + "'" + getVal[0].Groups[1] + "'";

                    dic.Add(k, v);

                }

                if (msg.StartsWith("-- Completed") && dic.Count > 0)
                {
                    _logtype = 0;

                    string strINSERT = dic.FirstOrDefault(d => d.Key == "#INSERT").Value;
                    string strVALUES = dic.FirstOrDefault(d => d.Key == "#VALUES").Value;
                    string strTABLE = dic.FirstOrDefault(d => d.Key == "#TB").Value;
                    string strGUID = dic.FirstOrDefault(d => d.Key == "@0").Value;

                    foreach (var entry in dic.Where(d => d.Key.StartsWith("@")))
                    {
                        string pattern = String.Format(@"(@)\b{0}\b", entry.Key.Replace("@", ""));
                        string replace = entry.Value;
                        strVALUES = Regex.Replace(strVALUES, pattern, replace);
                    }

                    string strSELECT = "SELECT * FROM " + strTABLE + " WHERE [Guid] = " + strGUID;
                    string strDELETE = "DELETE   FROM " + strTABLE + " WHERE [Guid] = " + strGUID;
                    string insert = " \r\n" + strINSERT + " \r\n" + strVALUES + "";
                    string strPRINT = "PRINT '[" + strTABLE + "]'";
                    dicTable[strTABLE + "_select"] = strSELECT;
                    dicTable[strTABLE + "_delete"] = strDELETE;
                    dicTable[strTABLE + "_insert"] = insert;
                    dicTable[strTABLE + "_print"] = strPRINT;

                    var orderDic = from entry in dicTable
                                   orderby entry.Key.EndsWith("_delete") ? 0 : 1, entry.Key.EndsWith("_select") ? 1 : 2
                                   select entry;

                    Debug.WriteLine("-- Start tables inserted log\r\n");
                    foreach (var item in orderDic)
                    {
                        Debug.WriteLine(item.Value);
                    }
                    Debug.WriteLine("-- End tables inserted log\r\n");


                    dic = new Dictionary<string, string>();
                }
            }
        }
        private void storeLog(string msg, string[] txt)
        {
            if (msg.StartsWith("[dbo]") && !txt[0].ToLower().Contains("_get"))
            {
                _logtype = 3;
                dic.Add("#STORE", msg);
            }

            if (_logtype == 3)
            {
                if (msg.StartsWith("--") && msg.Contains("Type ="))
                {
                    var Ntype = msg.Contains("Type = Guid") ? "N" : string.Empty;

                    var val = @"\'(.*?)\'";
                    var getVal = Regex.Matches(msg, val);
                    var value = getVal[0].Groups[1].ToString();
                    var v = value == "null" ? value : "" + Ntype + "'" + value + "'";
                    dic["#STORE"] = dic.FirstOrDefault(d => d.Key == "#STORE").Value + " " + v + ",";

                }
                if (msg.StartsWith("-- Completed") && dic.Count > 0)
                {
                    _logtype = 0;

                    string strSTORE = dic.FirstOrDefault(d => d.Key == "#STORE").Value.TrimEnd(',');
                    string cm = "-- EXEC ";
                    Debug.WriteLine("\r\n");
                    Debug.WriteLine("\r\n");
                    Debug.WriteLine(cm + strSTORE);
                    Debug.WriteLine("-- PRINT '" + txt[2] + "'\r\n");
                    Debug.WriteLine("\r\n");
                    dic = new Dictionary<string, string>();
                }
            }

        }
        private void updateLog(string msg, string[] txt)
        {
            if (msg.StartsWith("UPDATE"))
            {
                _logtype = 2;
                dic.Add("#UPDATE", txt[0]);

                var val = @"\[(.*?)\]";
                var getVal = Regex.Matches(txt[0], val);
                var v = getVal[1].Groups[1] + "";

                dic.Add("#TB", v);

                if (txt[1].StartsWith("SET"))
                {
                    dic.Add("#SET", txt[1]);
                }

                if (txt[2].StartsWith("WHERE"))
                {
                    dic.Add("#WHERE", txt[2]);
                }
            }



            if (_logtype == 2)
            {
                if (msg.StartsWith("-- @"))
                {
                    var Ntype = msg.Contains("Type = Guid") ? "N" : string.Empty;

                    var val = @"\'(.*?)\'";
                    var key = @"\@(.*?)\:";
                    var getVal = Regex.Matches(msg, val);
                    var getKey = Regex.Matches(msg, key);
                    var k = "@" + getKey[0].Groups[1];
                    var v = "" + Ntype + "'" + getVal[0].Groups[1] + "'";

                    dic.Add(k, v);
                }

                if (msg.StartsWith("-- Completed") && dic.Count > 0)
                {
                    _logtype = 0;

                    string strUPDATE = dic.FirstOrDefault(d => d.Key == "#UPDATE").Value;
                    string strSET = dic.FirstOrDefault(d => d.Key == "#SET").Value;
                    string strWHERE = dic.FirstOrDefault(d => d.Key == "#WHERE").Value;
                    string strTABLE = dic.FirstOrDefault(d => d.Key == "#TB").Value;
                    var param = dic.Where(d => d.Key.StartsWith("@"));

                    foreach (var entry in param)
                    {
                        string pattern = String.Format(@"(@)\b{0}\b", entry.Key.Replace("@", ""));
                        string replace = entry.Value;
                        strSET = Regex.Replace(strSET, pattern, replace);
                    }

                    foreach (var entry in param)
                    {
                        string pattern = String.Format(@"(@)\b{0}\b", entry.Key.Replace("@", ""));
                        string replace = entry.Value;
                        strWHERE = Regex.Replace(strWHERE, pattern, replace);
                    }

                    string strSELECT = "SELECT * FROM " + strTABLE + " " + strWHERE;
                    string strDELETE = "DELETE   FROM " + strTABLE + " " + strWHERE;

                    dicTable[strTABLE] = strSELECT + " \r\n" + strDELETE;

                    string cm = "-- ";
                    Debug.WriteLine("\r\n");
                    Debug.WriteLine("\r\n");
                    Debug.WriteLine(cm + strSELECT);
                    Debug.WriteLine(cm + strDELETE);
                    Debug.WriteLine(cm + strUPDATE);
                    Debug.WriteLine(cm + strSET);
                    Debug.WriteLine(cm + strWHERE);
                    Debug.WriteLine("-- PRINT '" + strTABLE + "'\r\n");
                    Debug.WriteLine("\r\n");
                    dic = new Dictionary<string, string>();
                }
            }
        }
        #endregion
    }
}
