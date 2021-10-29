using Bgt.Ocean.Infrastructure.Util;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Models
{
    #region ### A ###
    [Serializable]
    partial class TblMasterActualJobHeader
    {
        public int JobTypeID { get; set; }
    }
    #endregion
    #region ### C ###
    [Serializable]
    partial class TblMasterRouteJobServiceStopLegs
    {
        public Guid CustomerGuid { get; set; }
        public string Time { get; set; }
        public string ActionNameAbbr { get; set; }
    }
    #endregion
    #region ### D ###
    [Serializable]
    partial class TblMasterDistrict
    {
        public string ProvinceStateName { get; set; }
    }
    #endregion
    #region ### H ###

    partial class TblMasterHistory_ActualJob
    {
        public TblMasterHistory_ActualJob()
        {
            FlagIsStaging = EnumGlobalEnvironment.IsStaging();
        }
    }

    #endregion
    #region ### L ###
    [Serializable]
    partial class TblMasterActualJobServiceStopLegs
    {
        public string ActionNameAbbr { get; set; }
    }
    #endregion
    #region ### N ###
    [Serializable]
    partial class TblMasterNemoTrafficFactorValue
    {
        public string DayOfWeekName { get; set; }
    }
    #endregion
    #region ### S ###
    [Serializable]
    partial class SFOJobListByRunResult
    {
        public string LastGeneratedStr { get; set; }
        public string ExpiredDateStr { get; set; }
    }
    #endregion
    #region ### P ###
    partial class PrevaultDepartmentBarcodeScanOutResult
    {
        public Nullable<System.Guid> InternalDepartmentGuid { get; set; }
        public string InternalDepartmentName { get; set; }
        public bool FlagScan { get; set; }

        //For grouping non-barcode
        public List<PrevaultDepartmentBarcodeScanOutResult> ItemsInGroup { get; set; } = new List<PrevaultDepartmentBarcodeScanOutResult>();
        public List<PrevaultDepartmentBarcodeScanOutResult> ShowItemsInGroup { get; set; } = new List<PrevaultDepartmentBarcodeScanOutResult>();

    }

    partial class PrevaultDepartmentSealConsolidateScanOutResult
    {
        public bool FlagScan { get; set; }
        public bool FlagKeyIn { get; set; } //TFS#51617 - True: Key-in , False: Barcode scan
    }
    #endregion
}
