
using Bgt.Ocean.Infrastructure.CustomAttributes;
using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework;
using Bgt.Ocean.Repository.EntityFramework.Repositories.SFO;
using Bgt.Ocean.Service.CustomJsonContractResolver;
using Bgt.Ocean.Service.ModelViews.GenericLog.AuditLog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Bgt.Ocean.Service.Implementations.AuditLog
{
    public interface IObjectComparerService
    {
        /// <summary>
        /// Compare 2 types to find which property is change and modify value to be logged
        /// </summary>
        /// <typeparam name="TOld"></typeparam>
        /// <typeparam name="TNew"></typeparam>
        /// <param name="oldObj"></param>
        /// <param name="newObj"></param>
        /// <param name="configKey"></param>
        /// <param name="fnSetValue">Method for modify its new and old value in ChangeInfo</param>
        /// <returns></returns>
        CompareResult GetCompareResult<TOld, TNew>(TOld oldObj, TNew newObj, string configKey, Action<ChangeInfo> fnSetValue = null);
    }

    public class ObjectComparerService : IObjectComparerService
    {
        private readonly ISFOSystemModelConfigRepository _sfoTblSystemModelConfigRepository;
        private readonly IDbFactory<SFOLogDbEntities> _sfoDbFactory;
        private readonly ISystemService _systemService;

        public ObjectComparerService(
                ISFOSystemModelConfigRepository sfoTblSystemModelConfigRepository,
                ISystemService systemService,
                IDbFactory<SFOLogDbEntities> sfoDbFactory
            )
        {
            _sfoTblSystemModelConfigRepository = sfoTblSystemModelConfigRepository;
            _sfoDbFactory = sfoDbFactory;
            _systemService = systemService;
        }

        #region Compare Logic

        private PropertyInfo[] _fieldInfoOldObj;
        private PropertyInfo[] _fieldInfoNewObj;
        private Action<ChangeInfo> _fnSetValue;

        private CompareResult Compare(object oldObj, object newObj, ComparableConfig config)
        {
            _fieldInfoOldObj = null;
            _fieldInfoNewObj = null;

            ValidateObjTypeWithConfig(oldObj, newObj, config);
            ValidateObjPropWithConfig(oldObj, newObj, config);

            return DoCompare(oldObj, newObj, config.ComparableConfigPropertyList);
        }

        private void ValidateObjTypeWithConfig(object oldObj, object newObj, ComparableConfig config)
        {
            var oldTypeName = oldObj.GetType().Namespace == "System.Data.Entity.DynamicProxies" ? oldObj.GetType().BaseType.Name : oldObj.GetType().Name;
            var newTypeName = newObj.GetType().Namespace == "System.Data.Entity.DynamicProxies" ? newObj.GetType().BaseType.Name : newObj.GetType().Name;

            if (oldTypeName != config.ModelNameSource || newTypeName != config.ModelNameTarget)
            {
                throw new ArgumentException("The type of source and target model are not match with the config. Please check ");
            }
        }

        private void ValidateObjPropWithConfig(object oldObj, object newObj, ComparableConfig config)
        {
            List<string> notInConfigList = new List<string>();

            _fieldInfoOldObj = oldObj.GetType().GetProperties();
            _fieldInfoNewObj = newObj.GetType().GetProperties();

            var fieldNameSource = _fieldInfoOldObj.Select(e => e.Name);
            var fieldNameTarget = _fieldInfoNewObj.Select(e => e.Name);

            var isInConfig = config.ComparableConfigPropertyList.All(e =>
            {
                var rs = fieldNameSource.Any(f => f == e.PropertyNameSource)
                    &&
                    fieldNameTarget.Any(f => f == e.PropertyNameTarget);

                if (!rs) notInConfigList.Add($"Source: {e.PropertyNameSource}, Target: {e.PropertyNameTarget}");

                return rs;
            });

            if (!isInConfig)
                throw new ArgumentException($"Please check property of Source Model '{config.ModelNameSource}' and Target Model '{config.ModelNameTarget}' type must be match with the config. \n List of incorrect are contains \n {string.Join("\n", notInConfigList)}");
        }

        private CompareResult DoCompare(object oldObj, object newObj, IEnumerable<ComparableConfigProperty> comparableConfigPropertyList)
        {
            CompareResult compareResult = new CompareResult();
            List<ChangeInfo> changeInfoList = new List<ChangeInfo>();

            foreach (var config in comparableConfigPropertyList)
            {
                var fieldInfo = _fieldInfoOldObj.FirstOrDefault(e => e.Name == config.PropertyNameSource);
                var sourceValue = _fieldInfoOldObj.FirstOrDefault(e => e.Name == config.PropertyNameSource)?.GetValue(oldObj);
                var targetValue = _fieldInfoNewObj.FirstOrDefault(e => e.Name == config.PropertyNameTarget)?.GetValue(newObj);

                string strSource = sourceValue == null ? "" : sourceValue.TryGetValue(fieldInfo.PropertyType);
                string strTarget = targetValue == null ? "" : targetValue.TryGetValue(fieldInfo.PropertyType);

                if (!strSource.Equals(strTarget) && ValidateValue(sourceValue?.ToString(), targetValue?.ToString(), fieldInfo.PropertyType))
                {
                    var changeInfo = new ChangeInfo
                    {
                        FieldName = config.PropertyNameSource,
                        NewValue = targetValue?.TryGetString(),
                        NewValueRaw = targetValue?.TryGetString(),
                        OldValue = sourceValue?.TryGetString(),
                        OldValueRaw = sourceValue?.TryGetString(),
                        LabelKey = config.LabelKey,
                        LogCategoryGuid = config.LogCategoryGuid,
                        LogProcessGuid = config.LogProcessGuid
                    };

                    _fnSetValue?.Invoke(changeInfo);

                    if (!config.QueryStrSetValue.IsEmpty())
                    {
                        changeInfo.NewValue = SetValueByQuery(config.QueryStrSetValue, changeInfo.NewValue);
                        changeInfo.OldValue = SetValueByQuery(config.QueryStrSetValue, changeInfo.OldValue);
                    }

                    changeInfoList.Add(changeInfo);
                }
            }

            compareResult.ChangeInfoList = changeInfoList;

            return compareResult;
        }

        private string SetValueByQuery(string queryStrSetValue, string value)
        {

            if (queryStrSetValue.IsEmpty() || value.IsEmpty() || value.Trim().IsEmpty()) return value;

            var dt = GetDataTable(queryStrSetValue, value);

            if (dt.Rows.Count == 0 || dt.Rows[0][0] == null) return value;

            return dt.Rows[0][0].ToString();
        }

        private bool ValidateValue(string src, string target, Type type)
        {
            if (type.IsDateTimeOrNullabelDateTime() && new string[] { src, target }.Contains(DateTime.MinValue.ToString()))
            {
                return false;
            }

            return true;
        }

        private DataTable GetDataTable(string query, string value)
        {
            DataTable dt = new DataTable();
            string connectionString = _sfoDbFactory.GetCurrentDbContext.Database.Connection.ConnectionString;

            using (var conn = new SqlConnection(connectionString))
            {
                using (var adapter = new SqlDataAdapter())
                {

                    try
                    {
                        adapter.SelectCommand = new SqlCommand();

                        adapter.SelectCommand.Connection = conn;
                        adapter.SelectCommand.Parameters.Add(new SqlParameter("@value", SqlDbType.NVarChar)).Value = value;
                        adapter.SelectCommand.CommandText = query;


                        conn.Open();

                        adapter.Fill(dt);

                        conn.Close();
                    }
                    catch (Exception err)
                    {
                        _systemService.CreateHistoryError(err);
                    }
                }
            }

            return dt;
        }

        #endregion


        public CompareResult GetCompareResult<TOld, TNew>(TOld oldObj, TNew newObj, string configKey, Action<ChangeInfo> fnSetValue = null)
        {
            var config = _sfoTblSystemModelConfigRepository.GetConfigByKey(configKey);
            _fnSetValue = fnSetValue;

            if (config == null)
            {
                return new CompareResult();
            }

            return Compare(oldObj, newObj, new ComparableConfig
            {
                ComparableConfigPropertyList = config.SFOTblSystemModelConfig_Property.Select(e => new ComparableConfigProperty
                {
                    PropertyNameSource = e.PropertyNameSource,
                    PropertyNameTarget = e.PropertyNameTarget,
                    LabelKey = e.LabelKey,
                    QueryStrSetValue = e.SFOTblSystemModelConfig_PropertyQuery?.QueryString,
                    LogCategoryGuid = e.SystemLogCategory_Guid,
                    LogProcessGuid = e.SFOTblSystemLogCategory?.SystemLogProcess_Guid,
                    Sequence = e.Sequence
                }).OrderBy(e => e.Sequence),
                ModelNameSource = config.ModelNameSource,
                ModelNameTarget = config.ModelNameTarget
            });
        }

    }

    #region Extension Method

    static class ObjectExtension
    {
        public static string TryGetValue(this object obj, Type type)
        {
            var ienumObj = obj as IEnumerable<object>;

            if (type.IsDateTimeOrNullabelDateTime())
            {
                return ((DateTime)obj).ChangeFromDateToString("yyyy-MM-dd HH:mm:ss UTC-0");
            }

            if (ienumObj != null)
            {
                return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
                {
                    ContractResolver = new MainContractResolver()
                });
            }

            return obj.ToString();
        }

        public static object GetValueByPropertyName(this object obj, string propName)
        {
            try
            {
                var property = obj.GetType().GetProperty(propName);
                var value = property?.GetValue(obj);
                return value;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public static string TryGetString(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            if (obj is IEnumerable<object>)
            {
                var objList = (IEnumerable<object>)obj;

                return objList.GetHtmlTableFromObj();
            }

            if (obj is DateTime || obj is DateTime?)
            {
                return ((DateTime)obj).ChangeFromDateToString("yyyy-MM-dd HH:mm:ss UTC-0");
            }

            if (obj is double)
            {
                return ((double)obj).ToString("N");
            }

            if (obj is int)
            {
                return ((int)obj).ToString("N0");
            }

            if (obj is decimal)
            {
                return ((decimal)obj).ToString("N");
            }

            return obj.ToString();
        }

        public static bool IsDateTimeOrNullabelDateTime(this Type type)
        {
            return type == typeof(DateTime) || type == typeof(Nullable<DateTime>);
        }
        public static bool IsTime(this Type type)
        {
            return type == typeof(TimeSpan);
        }
        private static string GetHtmlTableFromObj(this IEnumerable<object> objList)
        {
            if (!objList.Any()) return "";

            var propInfoList = objList.First().GetType().GetProperties();
            var rowList = new List<string>();

            // construct table rows
            foreach (var data in objList)
            {
                string row = "<tr>";

                row += string.Join("", propInfoList.Where(prop => prop.GetCustomAttributes(typeof(AvoidAuditLogAttribute), true).Length == 0)
                    .Select(prop => $"<td data-log-prop-name=\"{prop.Name}\">{prop.GetValue(data).TryGetString()}</td>"));

                row += "</tr>";

                rowList.Add(row);
            }

            Func<PropertyInfo, string> getCustomAttributeName = (prop) =>
            {
                if (prop.HasCustomAttributeOfType<ChangeJsonPropNameAttribute>())
                {
                    return prop.GetCustomAttribute<ChangeJsonPropNameAttribute>().Name.SeparateWordByUppercase();
                }
                return prop.Name.SeparateWordByUppercase();
            };

            // construct table header
            string thead = "<thead><tr>" + string.Join("", propInfoList.Where(prop => prop.GetCustomAttributes(typeof(AvoidAuditLogAttribute), true).Length == 0)
                .Select(h => $"<th data-log-prop-name=\"{h.Name}\">{getCustomAttributeName(h)}</th>")) + "</tr></thead>";

            // merge table rows to tbody
            string tbody = "<tbody>" + string.Join("", rowList) + "</tbody>";

            return $"<table>{thead}{tbody}</table>";
        }
    }

    #endregion
}
