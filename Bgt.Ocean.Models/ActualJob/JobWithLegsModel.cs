using System.Collections.Generic;

namespace Bgt.Ocean.Models
{
    public class JobWithLegsModel
    {
        public TblMasterActualJobHeader JobHeader { get; set; }

        public List<TblMasterActualJobServiceStopLegs> JobLegs { get; set; } = new List<TblMasterActualJobServiceStopLegs>();

        public List<TblMasterActualJobItemsSeal> Seals { get; set; } = new List<TblMasterActualJobItemsSeal>();

        public List<TblMasterActualJobItemsCommodity> Commodities { get; set; } = new List<TblMasterActualJobItemsCommodity>();

        public bool FlagUpdate { get; set; } = false;
    }
}
