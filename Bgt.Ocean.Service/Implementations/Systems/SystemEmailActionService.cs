using Bgt.Ocean.Models.Customer;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Systems;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Implementations.Systems
{
    public interface ISystemEmailActionService
    {
        IEnumerable<CustomerEmailActionModel> GetEmailAction(Guid languageGuid);
    }

    public class SystemEmailActionService : ISystemEmailActionService
    {
        private readonly ISystemEmailActionRepository _systemEmailActionRepository;

        public SystemEmailActionService(
            ISystemEmailActionRepository systemEmailActionRepository)
        {
            _systemEmailActionRepository = systemEmailActionRepository;
        }

        public IEnumerable<CustomerEmailActionModel> GetEmailAction(Guid languageGuid)
        {
            return _systemEmailActionRepository.GetEmailAction(languageGuid);
        }


    }
}
