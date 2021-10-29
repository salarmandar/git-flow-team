using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Infrastructure.Storages;
using System;

namespace Bgt.Ocean.Service.Messagings
{
    public class RequestBase
    {


        #region Original 
        private string _userName { get; set; }
        private DateTimeOffset _universalDatetime { get; set; }
        private Guid? _userLanguageGuid { get; set; }
        private string _userFormatDate { get; set; }

        public Guid? UserLanguageGuid
        {
            get
            {
                if (_userLanguageGuid == default(Guid?))
                {
                    _userLanguageGuid = ApiSession.UserLanguage_Guid;
                }
                return _userLanguageGuid;
            }
            set
            {
                _userLanguageGuid = value;
            }
        }
        public string UserFormatDate
        {
            get
            {
                if (string.IsNullOrEmpty(_userFormatDate))
                {
                    _userFormatDate = ApiSession.UserFormatDate;
                }
                return _userFormatDate;
            }
            set
            {
                _userFormatDate = value;
            }
        }

        public string UserFormatDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(_userFormatDate))
                {
                    _userFormatDate = ApiSession.UserFormatDate + " HH:mm";
                }
                return _userFormatDate;
            }
            set
            {
                _userFormatDate = value;
            }
        }
        /// <summary>
        /// UTC now + local offset
        /// local offset come from client-side by integer offset in header like -420 is UTC +7
        /// </summary>
        public DateTimeOffset ClientDateTime
        {
            get
            {
                return DateTimeHelper.UtcNowOffsetLocal;
            }
        }
        /// <summary>
        /// Request by user
        /// </summary>
        public string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(_userName))
                {
                    _userName = ApiSession.UserName;
                }
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        /// <summary>
        /// DateTime from client-side [undefined offset]
        /// </summary>
        public DateTime LocalClientDateTime { get { return ClientDateTime.DateTime; } }
        /// <summary>
        /// DateTime offset zero (dateNow) - Ocean back-end style
        /// </summary>
        public DateTimeOffset UniversalDatetime
        {
            get
            {
                if (_universalDatetime == default(DateTimeOffset))
                {
                    _universalDatetime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                }
                return _universalDatetime;
            }
            set
            {
                _universalDatetime = value;
            }
        }
        #endregion

    }
}
