using System;

namespace Bgt.Ocean.Models
{
    public partial class SFOTblTransactionServiceRequest
    {
        public Guid MasterCountryGuid
        {
            get
            {
                return TblMasterSite.MasterCountry_Guid;
            }
        }

        public bool IsDepartureToOnsite()
            => FlagStampArrive.GetValueOrDefault();

        public bool IsOnHold()
            => FlagOnHold.GetValueOrDefault();

        public int? BrinksSiteTimeZoneID
        {
            get
            {
                return TblMasterSite.TimeZoneID;
            }
        }

        public void SetDateTimeAndUserModified(string username)
        {
            UserModified = username;
            DatetimeModified = DateTime.Now;
            UniversalDatetimeModified = DateTime.UtcNow;
        }
    }
}
