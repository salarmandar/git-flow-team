using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Bgt.Ocean.Infrastructure.CompareHelper;

namespace Bgt.Ocean.Repository.EntityFramework.Repositories.Systems
{
    public interface ISystemMessageRepository : IRepository<TblSystemMessage>
    {
        TblSystemMessage FindByMsgId(int msgId, Guid languageId);
        IEnumerable<TblSystemMessage> FindByLanguage(Guid languageId);

        string HistoryMappingParameters(string pattern, string param, int MsgID = 0);
        TblSystemMessage GetMessage(int messageId, Guid languageGuid);
    }

    public class SystemMessageRepository : Repository<OceanDbEntities, TblSystemMessage>, ISystemMessageRepository
    {
        public IEnumerable<TblSystemMessage> FindByLanguage(Guid languageId)
        {
            return DbContext.TblSystemMessage.Where(e => e.SystemLanguage_Guid == languageId);
        }
        private bool IsJSON(string txt)
        {

            try
            {
                JArray.Parse(txt ?? string.Empty);
                return true;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine("Error \n");
                return false;
            }
        }
        public string HistoryMappingParameters(string pattern, string param, int MsgID = 0)
        {

            JArray objparam = null;
            string defaultspliter = ",";
            string jsonspliter = "|spliter|";
            string[] stringSeparators = new string[] { defaultspliter };

            if (IsJSON(param))
            {
                stringSeparators = new string[] { jsonspliter };
                objparam = JsonConvert.DeserializeObject<JArray>(param);
                param = string.Join("|spliter|", objparam.Values());
            }

            string msg = "Error MsgID " + MsgID + " : message pattern is required ({0}) and existing parameter is ({1}).";
            int pat = Regex.Matches(Regex.Replace(pattern,
                              @"(\{{2}|\}{2})", ""),
                              @"\{\d+(?:\:?[^}]*)\}").Count;

            msg = pat == 0 ? pattern : msg;
            if (!string.IsNullOrEmpty(param))
            {
                string[] text = param.Split(stringSeparators, StringSplitOptions.None);
                int par = text.Length;
                msg = par == pat ? string.Format(pattern, text) : string.Format(msg, pat, par);
            }

            //fixed param is null
            if (pat != 0 && string.IsNullOrEmpty(param))
                msg = string.Format(msg, pat, 0);

            return msg;
        }

        public SystemMessageRepository(IDbFactory<OceanDbEntities> dbFactory) : base(dbFactory)
        {
        }

        public TblSystemMessage FindByMsgId(int msgId, Guid languageId)
        {
            var messages = DbContext.TblSystemMessage.Where(e => e.MsgID == msgId);
            if (messages == null)
            {
                return GetMessageEN(messages.ToList(), msgId).Clone();
            }
            else
            {
                var message = messages.FirstOrDefault(e => e.SystemLanguage_Guid == languageId);
                if (message == null)
                {
                    message = GetMessageEN(messages.ToList(), msgId);
                }

                return message.Clone();
            }
        }

        public TblSystemMessage GetMessage(int messageId, Guid languageGuid)
        {
            using (var context = new OceanDbEntities())
            {
                return context.TblSystemMessage.FirstOrDefault(o => o.MsgID == messageId && o.SystemLanguage_Guid == languageGuid);
            }
        }

        private TblSystemMessage GetMessageEN(List<TblSystemMessage> messages, int msgId)
        {
            // Enlish - US
            var enGuid = Guid.Parse("6fa2bd67-0794-4a9e-a13b-2d81ddb574a0");
            var defaultEn = messages.FirstOrDefault(e => e.SystemLanguage_Guid == enGuid);
            if (defaultEn == null)
            {
                defaultEn = new TblSystemMessage();
                defaultEn.MsgID = -404;
                defaultEn.MessageTextContent = $"This message {msgId} is not found.";
                defaultEn.MessageTextTitle = "Message error.";
            }

            return defaultEn;
        }
    }
}
