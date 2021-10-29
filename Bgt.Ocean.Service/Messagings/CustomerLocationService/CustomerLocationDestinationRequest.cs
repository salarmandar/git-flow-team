using Bgt.Ocean.Models.CustomerLocation;
using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.CustomerLocationService
{


    //MultiBr:TV,P
    public class GetMultiBrDestinationDetailRequest : RequestBase
    {

        /// <summary>
        /// [MasterRoute] Pick Up Customer
        /// </summary>
        public Guid OriginCustomerLocation_Guid { get; set; }
        /// <summary>
        /// [MasterRoute] Pick Up Site
        /// </summary>
        public Guid OriginSite_Guid { get; set; }

        /// <summary>
        /// [MasterRoute] Job Type
        /// </summary>
        public Guid? SystemSubServiceType_Guid { get; set; }

        /// <summary>
        /// [MasterRoute] Job Type
        /// </summary>
        public Guid SystemServiceJobType_Guid { get; set; }

        //Client:: For master route
        /// <summary>
        /// [MasterRoute] LOB
        /// </summary>
        public Guid MasterLineOfBusiness_Guid { get; set; }
        /// <summary>
        /// [MasterRoute] Delivery Day Of Week
        ///  delivery_daysequence = pickup_daysequence + DayInvault
        /// </summary>
        public Guid SystemDayOfWeek_Guid { get; set; }
        /// <summary>
        /// [MasterRoute] Delivery Check Box Show All
        /// </summary>
        public bool FlagShowAll { get; set; }
        /// <summary>
        /// [MasterRoute] Delivery Master Route
        /// </summary>
        public Guid MasterRoute_Guid { get; set; }
    }
    //MultiBr:BCD
    public class GetMultiBrOriginDetailRequest
    {
        /// <summary>
        /// BCD: Delivery Customer
        /// </summary>
        public Guid DestLocation_Guid { get; set; }
        /// <summary>
        /// BCD: Delivery Site
        /// </summary>
        public Guid DestSite_Guid { get; set; }

    }
    //MultiBr:TV,P,BCD
    public class MultiBrDetailResponse
    {
        public MultiBrDetailView LocationDestination { get; set; }
    }


    public class GetAdhocAllCustomerAndLocationRequest : RequestBase
    {

        /// <summary>
        /// [MasterRoute] selected Site 
        /// </summary>
        public Guid Site_Guid { get; set; }

        /// <summary>
        /// [MasterRoute] selected customer
        /// </summary>
        public Guid? MasterCustomer_Guid { get; set; }

        /// <summary>
        /// [MasterRoute] not allow master site,origin site 
        /// </summary>
        public Guid? OriginSite_Guid { get; set; }
    }

    public class GetAdhocAllCustomerAndLocationResponse
    {
        public MultiBrDetailView LocationDestination { get; set; }
    }


    public class GetMasterRouteAllCustomerAndLocationRequest : RequestBase
    {

        /// <summary>
        /// [MasterRoute] selected Site 
        /// </summary>
        public Guid Site_Guid { get; set; }
        /// <summary>
        /// [MasterRoute] selected Job Type
        /// </summary>
        public Guid SystemServiceJobType_Guid { get; set; }
        /// <summary>
        /// [MasterRoute] selected Sub Service Type
        /// </summary>
        public Guid? SystemSubServiceType_Guid { get; set; }
        /// <summary>
        /// [MasterRoute] selected LOB
        /// </summary>
        public Guid MasterLineOfBusiness_Guid { get; set; }
        /// <summary>
        /// [MasterRoute] selected Day Of Week
        /// </summary>
        public Guid SystemDayOfWeek_Guid { get; set; }
        /// <summary>
        /// [MasterRoute] selected Check Box Show All
        /// </summary>
        public bool FlagShowAll { get; set; }
        /// <summary>
        /// [MasterRoute] selected Master Route
        /// </summary>
        public Guid MasterRoute_Guid { get; set; }

        /// <summary>
        /// [MasterRoute] selected customer
        /// </summary>
        public Guid? MasterCustomer_Guid { get; set; }

        /// <summary>
        /// [MasterRoute] not allow master site,origin site 
        /// </summary>
        public Guid? OriginSite_Guid { get; set; }

        /// <summary>
        /// [MasterRoute] Action Job 'P' or 'D'
        /// </summary>
        public string strJobActionAbb { get; set; }
    }

    public class GetMasterRouteAllCustomerAndLocationResponse
    {
        public MultiBrDetailView LocationDestination { get; set; }
    }


    public class GetAllSitePathRequest
    {
        /// <summary>
        /// Not null
        /// </summary>
        public Guid OriginSite_Guid { get; set; }
        /// <summary>
        /// Not null
        /// </summary>
        public Guid DestinationSite_Guid { get; set; }
    }

    public class GetAllSitePathResponse
    {
        public IEnumerable<DropdownSitePathView> sitepath { get; set; }
    }

    public class ChangeCustomerview
    {
        public Guid CurrentCustomerGuid { get; set; }
        public string CurrentCustomerName { get; set; }
        public Guid LocationGuid { get; set; }
        public string LocationName { get; set; }
        public IEnumerable<CustomerView> NewCustomer { get; set; }
    }
    public class CustomerView
    {
        public Guid Guid { get; set; }
        public string CustomerName { get; set; }
    }

}
