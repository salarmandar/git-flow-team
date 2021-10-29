using System;
using System.Collections.Generic;
using System.Linq;
using Bgt.Ocean.Models.RunControl;
using Bgt.Ocean.Infrastructure.Storages;
using Bgt.Ocean.Infrastructure.Util;

namespace Bgt.Ocean.Service.Messagings.RunControlService
{
    public class JobPropertiesRequest
    {
        public Guid JobGuid { get; set; }
        public Guid LanguageGuid { get; set; } = ApiSession.UserLanguage_Guid.GetValueOrDefault();
        public string FormatDateUser { get; set; }
        public Guid SiteGuid { get; set; }
        /// <summary>
        /// more detail >>>
        /// </summary>
        public CashAddPropertiesTab TabID { get; set; }
    }


    public class JobPropertiesResponse : BaseResponse
    {
        public JobPropertiesView JobProperties { get; set; }

        public IEnumerable<CashAddAvailableTab> SVD_AvailableTab { get; set; }

    }

    public class CashAddAvailableTab
    {
        public CashAddPropertiesTab TabID { get; set; }
        public CashAddPropertiesTab ParentID { get; set; }
        public bool HasChild { get; set; }
        public IEnumerable<HidePanel> HidePanels { get; set; }

        public static void CreateTab(List<CashAddAvailableTab> Tabs, IEnumerable<HidePanel> hidePanels, CashAddPropertiesTab tabID, CashAddPropertiesTab parentID, bool hasChild)
        {

            var panels = Enumerable.Empty<HidePanel>();
            var flagHideAll = false;
            switch (tabID)
            {
                case CashAddPropertiesTab.None:
                    break;
                case CashAddPropertiesTab.tabDetail:
                    break;
                case CashAddPropertiesTab.tabLeg:
                    break;
                case CashAddPropertiesTab.tabServiceDetail:
                    break;
                case CashAddPropertiesTab.tabSVD_CITDelivery:
                    break;
                case CashAddPropertiesTab.tabSVD_NoteWithdraw_MachineReport_ActualCount:
                    var screenInTab = new JobScreen[] { JobScreen.MachineReport, JobScreen.ActualCount };
                    var FieldInActualScreen = new JobField[] { JobField.EntireScreen, JobField.Dispense_Beginning_TotalATM };
                    var FieldInMachineScreen = new JobField[] { JobField.EntireScreen };
                    flagHideAll = screenInTab.All(o => hidePanels.Any(e=>e.JobScreen == o));

                    panels = hidePanels.Where(o => (o.JobScreen == JobScreen.ActualCount && o.JobField.Any(e => FieldInActualScreen.Contains(e)) )  
                                                || (o.JobScreen == JobScreen.MachineReport ));//&& o.JobField.Any(e => FieldInMachineScreen.Contains(e) 
                    break;            
                case CashAddPropertiesTab.tabSVD_NoteWithdraw_CashAdd_CashReturn:
                    break;
                case CashAddPropertiesTab.tabSVD_SmallBagDeposit_SmallBag:
                    break;
                case CashAddPropertiesTab.tabSVD_CapturedCard:
                    break;
                case CashAddPropertiesTab.tabSVD_Checklist:
                    break;
                case CashAddPropertiesTab.tabHistory:
                    break;
                default:
                    break;
            }

            if (!flagHideAll)
            {
                Tabs.Add(new CashAddAvailableTab()
                {
                    TabID = tabID,
                    ParentID = parentID,
                    HasChild = hasChild,
                    HidePanels = panels
                });
            }
        }

    }
}
