using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories;
using Bgt.Ocean.Repository.EntityFramework.Repositories.CustomerLocation;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bgt.Ocean.Service.Implementations.Nemo.NemoSync
{
    #region Interface
    public interface INemoSyncService
    {
        bool NemoSyncStart();

        Task<bool> SyncMasterCustomer(List<RequestHeader> requestHeaders);
    }
    #endregion

    #region Service
    public class NemoSyncService : INemoSyncService
    {
        #region Objects & Variables
        private readonly INemoAuthenticationService _nemoAuthenticationService;


        private readonly IMasterCustomerRepository _masterCustomerRepository;
        private readonly IMasterCustomerLocationRepository _masterCustomerLocationRepository;
        private readonly IMasterSiteRepository _masterSiteRepository;
        private readonly ISystemEnvironment_GlobalRepository _systemEnvironment_GlobalRepository;
        private readonly ISystemServiceJobTypeRepository _systemServiceJobTypeRepository;

        private static TblSystemEnvironment_Global NEMO_SIZE_SYNC;
        #endregion

        #region Constructor
        public NemoSyncService(INemoAuthenticationService nemoAuthenticationService
            , IMasterCustomerRepository masterCustomerRepository
            , IMasterCustomerLocationRepository masterCustomerLocationRepository
            , IMasterSiteRepository masterSiteRepository
            , ISystemEnvironment_GlobalRepository systemEnvironment_GlobalRepository
            , ISystemServiceJobTypeRepository systemServiceJobTypeRepository)
        {
            _nemoAuthenticationService = nemoAuthenticationService;

            _masterCustomerRepository = masterCustomerRepository;
            _masterCustomerLocationRepository = masterCustomerLocationRepository;
            _masterSiteRepository = masterSiteRepository;
            _systemEnvironment_GlobalRepository = systemEnvironment_GlobalRepository;
            _systemServiceJobTypeRepository = systemServiceJobTypeRepository;

            var nemoSyncSize = Task.Run(() => _systemEnvironment_GlobalRepository.FindByAppKeyAsync("NEMO_SIZE_SYNC"));

            NEMO_SIZE_SYNC = nemoSyncSize.Result;
        }
        #endregion

        public bool NemoSyncStart()
        {
            var authen = _nemoAuthenticationService.GetNemoAutentication();
            if (authen != null && authen.Success)
            {
                List<RequestHeader> requestHeaders = new List<RequestHeader>();
                requestHeaders.Add(new RequestHeader() { Name = "Token", Value = _nemoAuthenticationService.GetToken()?.Token });

                var syncServiceJobType = Task.Run(() => SyncServiceJobType(requestHeaders));
                var syncMasterSite = Task.Run(() => SyncMasterSite(requestHeaders));
                var syncMasterCustomer = Task.Run(() => SyncMasterCustomer(requestHeaders));

                syncServiceJobType.Wait();
                syncMasterSite.Wait();
                syncMasterCustomer.Wait();

                return true;
            }
            return false;
        }

        public async Task<bool> SyncServiceJobType(List<RequestHeader> requestHeaders)
        {
            await _systemServiceJobTypeRepository.FindByLastNemoSyncAsync(DateTime.Now);
            return true;
        }

        public async Task<bool> SyncMasterSite(List<RequestHeader> requestHeaders)
        {
            await _masterSiteRepository.FindByLastNemoSyncAsync(DateTime.Now);
            return true;
        }

        public async Task<bool> SyncMasterCustomer(List<RequestHeader> requestHeaders)
        {
            var data = await _masterCustomerRepository.FindByLastNemoSyncAsync(DateTime.Now);
            int size = NEMO_SIZE_SYNC == null ? 500 : Convert.ToInt32(NEMO_SIZE_SYNC.AppValue1);
            var customer = data.Select(o => o.CustomerGuid).Distinct().Take(size).ToList();

            await _masterCustomerLocationRepository.FindByLastNemoSyncAsync(DateTime.Now, customer);
            return true;
        }
    }
    #endregion
}
