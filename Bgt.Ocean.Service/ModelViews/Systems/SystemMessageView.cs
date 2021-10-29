using Bgt.Ocean.Models;
using Bgt.Ocean.Service.Messagings.AdhocService;
using System;

namespace Bgt.Ocean.Service.ModelViews.Systems
{
    public class SystemMessageView
    {
        public int MsgID { get; set; }
        public Guid SystemLanguage_Guid { get; set; }
        public string MessageTextContent { get; set; }

        private string _title { get; set; }
        public string MessageTextTitle { get { return (_title ?? string.Empty).Contains(MsgID.ToString()) ? _title : $"{_title} (Msg ID: {MsgID})"; } set { _title = value; } }

        private bool _success { get; set; }
        public bool IsSuccess { get { return _success || MsgID == 0; } set { _success = value; } }
        public bool IsWarning { get; set; }

        public SystemMessageView()
        {

        }

        public SystemMessageView(TblSystemMessage tblmsg)
        {
            this.MsgID = tblmsg.MsgID;
            this.MessageTextContent = tblmsg.MessageTextContent;
            this.MessageTextTitle = tblmsg.MessageTextTitle;
            this.SystemLanguage_Guid = tblmsg.SystemLanguage_Guid;
            this.IsSuccess = MsgID == 0;
        }        

        public SystemMessageView(TblSystemMessage tblmsg, AdhocTempData tempdata)
        {
            string[] arr_params = tempdata.MsgParams_ForAdhocJob.ToArray();
            this.MsgID = tblmsg.MsgID;
            this.MessageTextContent = string.Format(tblmsg.MessageTextContent, arr_params);
            this.MessageTextTitle = tblmsg.MessageTextTitle;
            this.SystemLanguage_Guid = tblmsg.SystemLanguage_Guid;
            this.IsSuccess = true;
        }
    }
}
