﻿using System;

namespace Bgt.Ocean.Service.Messagings.BrinksService
{
    public class CustomerGeneralResponse
    {
        public System.Guid Guid { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerFullName { get; set; }
        public Nullable<System.Guid> MasterCountry_Guid { get; set; }
        public string MasterCountryName { get; set; }
        public Nullable<bool> FlagHaveState { get; set; }
        public Nullable<bool> FlagInputCityManual { get; set; }
        public Nullable<System.DateTime> DatetimeModified { get; set; }
        public Nullable<System.DateTimeOffset> UniversalDatetimeModified { get; set; }
        public Nullable<bool> FlagDisable { get; set; }
        public Nullable<bool> FlagChkCustomer { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string ZipCode { get; set; }
        public string CommercialRegistrationID { get; set; }
        public string Email { get; set; }
        public Nullable<System.Guid> MasterDistrict_Guid { get; set; }
        public string DistrictName { get; set; }
        public Nullable<System.Guid> MasterCity_Guid { get; set; }
        public string CityName { get; set; }
        public int FlagCityIsCountryState { get; set; }
        public string TaxID { get; set; }
        public string SecondTaxID { get; set; }
    }
}
