
using Bgt.Ocean.Infrastructure.Util;
using System;

namespace Bgt.Ocean.Models.FleetMaintenance
{

    public class VendorView
    {
        public Guid VendorGuid { get; set; }
        public string VendorName { get; set; }
    }

    public class MaintenanceStatusView
    {
        public Guid MaintenanceStatusGuid { get; set; }
        public string MaintenanceStatusName { get; set; }
        public EnumMaintenanceStatus MaintenanceStatusID { get; set; }
    }

    public class MaintenanceCategoryView
    {
        public Guid? MasterMaintenanceCategoryGuid { get; set; }
        public string MaintenanceCategoryName { get; set; }
    }

    //public class MaintenanceCategoryItemsView
    //{

    //    public Guid? MasterMaintenanceCategoryItemGuid { get; set; }
    //    public Guid? MasterMaintenanceCategoryGuid { get; set; }
    //    public string MaintenanceCategoryName { get; set; }
    //    public string ItemName { get; set; }
    //}
    public class DropdownViewModel<T> where T : class
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public T Obj { get; set; }

        public static DropdownViewModel<T> Create(string text, string value, T obj)
        {
            return new DropdownViewModel<T>
            {
                Text = text,
                Value = value,
                Obj = obj
            };
        }
    }

}
