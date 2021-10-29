

using Bgt.Ocean.Infrastructure.Helpers;

namespace Bgt.Ocean.Models
{

    public partial class TblMasterDailyRunResource_Alarm
    {
        public void CleanupDirty()
        {
            EmployeeName = EmployeeName.Truncate(500);
            Phone = Phone.Truncate(100);
            SiteName = SiteName.Truncate(150);
            RouteName = RouteName.Truncate(300);
            RunNo = RunNo.Truncate(200);
            Latitude = Latitude.Truncate(50);
            Longitude = Longitude.Truncate(50);
            UserCreated = UserCreated.Truncate(200);
            UserAcknowledged = UserAcknowledged.Truncate(200);
            UserDeactivated = UserDeactivated.Truncate(200);
        }
    }
}
