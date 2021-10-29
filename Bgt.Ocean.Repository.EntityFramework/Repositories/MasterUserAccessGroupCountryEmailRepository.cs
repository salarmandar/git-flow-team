namespace Bgt.Ocean.Repository.EntityFramework.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Configuration;
    using Models;

    public interface IMasterUserAccessGroupCountryEmailRepository : IRepository<TblMasterUserAccessGroupCountryEmail>
    {
        /// <summary>
        /// get the list of emails by country by guid.
        /// </summary>
        /// <param name="countryGuid">country Guid.</param>
        /// <returns>a list of emails by guid.</returns>
        IEnumerable<string> GetEmailsByCountry(Guid countryGuid);

        /// <summary>
        /// get the list of emails by country.
        /// </summary>
        /// <returns>a list of emails.</returns>
        IEnumerable<TblMasterUserAccessGroupCountryEmail> GetListUserAccessGroupCountryEmail();
    }


    /// <summary>
    /// Repository of table MasterUserAccessGroupCountryEmail.
    /// This table has data of emails per country.
    /// </summary>
    public class MasterUserAccessGroupCountryEmailRepository : Repository<OceanDbEntities, TblMasterUserAccessGroupCountryEmail>, IMasterUserAccessGroupCountryEmailRepository
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MasterUserAccessGroupCountryEmailRepository"/> class.
        /// </summary>
        /// <param name="factory">Interface factory.</param>
        public MasterUserAccessGroupCountryEmailRepository(IDbFactory<OceanDbEntities> factory)
            : base(factory)
        {
        }
        #endregion

        #region Methods

        #region PublicMethods

        /// <summary>
        /// get the list of emails by country by guid.
        /// </summary>
        /// <param name="countryGuid">country Guid.</param>
        /// <returns>a list of emails by guid.</returns>
        public IEnumerable<string> GetEmailsByCountry(Guid countryGuid)
        {
            IEnumerable<string> emails = new List<string>();
            IEnumerable<TblMasterUserAccessGroupCountryEmail> result = this.FindAll(x => x.MasterCountry_Guid == countryGuid);
            TblMasterUserAccessGroupCountryEmail emailByCountry = result != null && result.Count() > 0 ? result.FirstOrDefault() : new TblMasterUserAccessGroupCountryEmail();
            string emailSeparatedByComma = emailByCountry != null ? emailByCountry.EmailList : string.Empty;

            emails = this.ConvertstringToList(emailSeparatedByComma);
            return emails;
        }

        /// <summary>
        /// Get list of Emails.
        /// </summary>
        /// <returns>a list of emails.</returns>
        public IEnumerable<TblMasterUserAccessGroupCountryEmail> GetListUserAccessGroupCountryEmail()
        {
            return DbContext.TblMasterUserAccessGroupCountryEmail.ToList();
        }

        #endregion

        #region PrivateMethods
        /// <summary>
        /// Convert a string separated by comma to list of strings.
        /// </summary>
        /// <param name="emails">string with email sparated by comma.</param>
        /// <returns>list of emails.</returns>
        private IEnumerable<string> ConvertstringToList(string emails)
        {
            IEnumerable<string> emailsList = new List<string>();

            if (!string.IsNullOrEmpty(emails))
            {
                char delimiter = ',';
                emailsList = emails.Split(delimiter);
            }

            return emailsList;
        }
        #endregion
        #endregion
    }
}