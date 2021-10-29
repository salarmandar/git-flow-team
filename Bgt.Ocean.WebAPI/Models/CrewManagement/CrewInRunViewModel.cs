using Bgt.Ocean.Infrastructure.Helpers;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.WebAPI.Models.CrewManagement
{
    public class CrewInRunViewModel
    {
        public IEnumerable<Guid> EmployeeGuid { get; set; }
        public Guid DailyRunGuid { get; set; }
        public string WorkDateStr { get; set; }
        public string DateFormat { get; set; }
        public DateTime? WorkDate
        {
            get
            {
                return WorkDateStr.ChangeFromStringToDate(DateFormat);
            }
        }
    }
}