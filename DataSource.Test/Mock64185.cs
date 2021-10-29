using Bgt.Ocean.Models;
using System.Data.Entity;

namespace DataSoruce.Test
{

    /*
     *  --query for create datasource
    ;WITH DS AS(
    SELECT * FROM TblMasterCurrency WHERE Guid = '82011826-B47D-4747-A533-A89F42DECF24'
    ),JSON_DATA AS(
    SELECT ROW_NUMBER() OVER(ORDER BY Guid ASC) AS Row# ,STRING_ESCAPE(v._ROW, 'JSON') Data# FROM DS s
    OUTER APPLY(
    SELECT (SELECT top 1 * FROM DS

    d WHERE d.Guid = s.Guid FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)

    AS _ROW) v
    )
    SELECT
    CASE WHEN Row# = 1										THEN 'result = result.AddDbSet<T,{EntityName}>("' + Data# + '"'
    WHEN Row# = (SELECT MAX(s.Row#) FROM JSON_DATA s)	THEN ',"' + Data# +'");'
    ELSE  ',"' + Data# + '"' END AS  Data#
    FROM JSON_DATA
         */
    public static class Mock64185<T> where T : class
    {

        public static DbSet<T> Data
        {
            get
            {
                DbSet<T> result = null;

                result = result.AddDbSet<T, TblMasterActualJobMCSCITDelivery>("{\"Guid\":\"2C1D251B-9843-4D74-A7E4-0C79768C3683\",\"MasterActualJobHeader_Guid\":\"D9D82D15-D00D-4558-8436-FB9F3D5D7E6A\",\"MasterActualJobServiceStopLegs_Guid\":\"036E24C4-8917-4635-8F79-A2F7C2EDA5B8\",\"MasterCurrency_Guid\":\"82011826-B47D-4747-A533-A89F42DECF24\",\"SealNo\":\"Elle002\",\"DeliveryDatetime\":\"2021-01-07T11:34:28.157\",\"STCValue\":55200.50,\"CITDeliveryStatus\":1,\"CustomerOrderNo\":201,\"UserCreated\":\"DPTest\",\"DatetimeCreated\":\"2021-01-07T11:34:28.157\",\"UniversalDatetimeCreated\":\"2021-01-07T04:34:28Z\",\"UniversalDatetimeModified\":\"2021-01-07T04:34:28Z\",\"SystemMCSCITDeliveryCommodityType_Guid\":\"984A9101-7FEB-4AE1-812D-0145D4B62975\",\"MasterReasonType_ReasonTypename\":\"\",\"CommentIfNotReceived\":\"\",\"SystemMCSCITDeliveryScanStatus_Guid\":\"FA77F00A-1096-407F-89AD-73A5EEEED4BE\"}"
, "{\"Guid\":\"E066EDBF-D22F-454B-A7AE-942C9B613019\",\"MasterActualJobHeader_Guid\":\"D9D82D15-D00D-4558-8436-FB9F3D5D7E6A\",\"MasterActualJobServiceStopLegs_Guid\":\"036E24C4-8917-4635-8F79-A2F7C2EDA5B8\",\"MasterCurrency_Guid\":\"82011826-B47D-4747-A533-A89F42DECF24\",\"SealNo\":\"Elle001\",\"DeliveryDatetime\":\"2021-01-07T11:34:15.190\",\"STCValue\":55200.50,\"CITDeliveryStatus\":1,\"CustomerOrderNo\":101,\"UserCreated\":\"DPTest\",\"DatetimeCreated\":\"2021-01-07T11:34:15.190\",\"UniversalDatetimeCreated\":\"2021-01-07T04:34:15Z\",\"UniversalDatetimeModified\":\"2021-01-07T04:34:15Z\",\"SystemMCSCITDeliveryCommodityType_Guid\":\"90115902-D98E-4A28-A0E8-17AE47B581D9\",\"MasterReasonType_ReasonTypename\":\"\",\"CommentIfNotReceived\":\"\",\"SystemMCSCITDeliveryScanStatus_Guid\":\"FA77F00A-1096-407F-89AD-73A5EEEED4BE\"}"
, "{\"Guid\":\"B0A653BE-090D-48D6-8D8F-AC1172E4C581\",\"MasterActualJobHeader_Guid\":\"D9D82D15-D00D-4558-8436-FB9F3D5D7E6A\",\"MasterActualJobServiceStopLegs_Guid\":\"036E24C4-8917-4635-8F79-A2F7C2EDA5B8\",\"MasterCurrency_Guid\":\"82011826-B47D-4747-A533-A89F42DECF24\",\"SealNo\":\"Elle003\",\"DeliveryDatetime\":\"2021-01-07T12:53:15.937\",\"STCValue\":55200.50,\"CITDeliveryStatus\":1,\"CustomerOrderNo\":301,\"UserCreated\":\"DPTest\",\"DatetimeCreated\":\"2021-01-07T12:53:15.937\",\"UniversalDatetimeCreated\":\"2021-01-07T05:53:16Z\",\"DatetimeModified\":\"2021-01-07T12:53:15.937\",\"UniversalDatetimeModified\":\"2021-01-07T05:53:16Z\",\"SystemMCSCITDeliveryCommodityType_Guid\":\"984A9101-7FEB-4AE1-812D-0145D4B62975\",\"SystemMCSCITDeliveryScanStatus_Guid\":\"B36390A5-E3F2-477E-A35A-C9CD6FE25E0C\"}");

                result = result.AddDbSet<T, TblMasterCurrency>("{\"Guid\":\"82011826-B47D-4747-A533-A89F42DECF24\",\"MasterCurrencyAbbreviation\":\"USD\",\"MasterCurrencyReportDisplay\":\"USD\",\"MasterCurrencyDescription\":\"US Dollas\",\"MasterCurrencyDiscrepancyAcceptable\":25.66,\"FlagDisable\":false,\"DatetimeCreated\":\"2014-02-10T09:14:58.1270000Z\",\"UniversalDatetimeCreated\":\"2014-02-10T09:14:58.1270000Z\",\"UserModifed\":\"PridG\",\"DatetimeModified\":\"2020-09-11T15:03:42Z\",\"UniversalDatetimeModified\":\"2020-09-11T08:03:42.4541653Z\",\"Symbol\":\"$\",\"ReferenceId\":\"82011826\"}");
                
                    
               return result;
            }
        }
    }
}
