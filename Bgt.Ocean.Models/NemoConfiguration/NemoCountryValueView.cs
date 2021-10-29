using System;

namespace Bgt.Ocean.Models.NemoConfiguration
{
    public class NemoCountryValueView
    {
        public Guid Guid { get; set; }
        public Guid? MasterCountry_Guid { get; set; }
        public Guid? SystemGlobalUnit_Guid { get; set; }
        public string LunchTime { get; set; }
        public int LunchDuration { get; set; }
        public bool FlagZoneUsing { get; set; }
        public bool FlagOverlapZone { get; set; }
        public string MaxOverlapDistance { get; set; }
        public int MaxServiceStop { get; set; }
        public string MaxLiability { get; set; }
        public int MaxDuration { get; set; }
        public string MaxCapacity { get; set; }
        public string MaxVolumn { get; set; }
        public string MaxDistanceBetweenStop { get; set; }
        public int MaxWaitTime { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DatetimeCreated { get; set; }
        public DateTimeOffset? UniversalDatetimeCreated { get; set; }
        public string UserModifed { get; set; }
        public DateTime? DatetimeModified { get; set; }
        public DateTimeOffset? UniversalDatetimeModified { get; set; }
        public string MaxRunTotalDistance { get; set; }
        public bool FlagTurnAround { get; set; }
        public string TurnAroundLeadTime { get; set; }
        public bool FlagAllowEarliestDispatchedTime { get; set; }
        public int MaxBayLoadingVehicle { get; set; }
        public int MaxBayLoadingTime { get; set; }
    }

    public class GlobalUnitText
    {
        public Guid Guid { get; set; }
        public string DisplayTxt { get; set; }
    }
}
