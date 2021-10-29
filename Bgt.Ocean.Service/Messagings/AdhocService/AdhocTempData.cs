using Bgt.Ocean.Service.ModelViews.Systems;
using System;
using System.Collections.Generic;

namespace Bgt.Ocean.Service.Messagings.AdhocService
{
    public class AdhocTempData
    {
        public AdhocTempData(CreateJobAdHocRequest request)
        {
            RouteName_LegD = request.ServiceStopLegDelivery.RouteName;
            RouteName_LegP = request.ServiceStopLegPickup.RouteName;
            RunResourceName_LegD = request.ServiceStopLegDelivery.RunResourceName;
            RunResourceName_LegP = request.ServiceStopLegPickup.RunResourceName;
            JobGuid = request.AdhocJobHeaderView.JobGuid;   //will be null when create job
        }
        public AdhocTempData()
        {
        }

        public string RouteName_LegP { get; set; }                  //Expected from Front End because it's in Dropdown

        public string RunResourceName_LegP { get; set; }            //Expected from Front End because it's in Dropdown

        public string RouteName_LegD { get; set; }                  //Expected from Front End because it's in Dropdown

        public string RunResourceName_LegD { get; set; }            //Expected from Front End because it's in Dropdown

        public string JobNo { get; set; }

        public Guid? JobGuid { get; set; }

        public bool isDispatchRun { get; set; } //check Leg P only because Leg D needs to have seal before assign to Dispatch Run

        public bool IsUseDolphin_LegP { get; set; }                 //Leg P uses Dolphin? check from Back End

        public bool IsUseDolphin_LegD { get; set; }                 //Leg D uses Dolphin? check from Back End

        public bool IsUpdateJob { get; set; }

        public int MsgId_ForAdhocJob { get; set; } = 0;

        public List<string> MsgParams_ForAdhocJob = new List<string>();
        public int ServiceJobTypeId { get; set; }


        public SystemMessageView ValidateMaxNumberJobs { get; set; }    //Let user know that current Run has jobs more than expected, then show warning message



        /// <summary>
        /// Job P, TV(P), T ---------> If job is TV and there are 2 Runs, check only TV(P) because it starts from P
        /// </summary>
        public void getMsgId_ForAdhocJob_LegP()
        {
            getMessageIDFromRouteAndRun(RouteName_LegP, RunResourceName_LegP);
        }


        /// <summary>
        /// Job D, TV(D) that has no Run in TV(P) ---------> If job is TV and there are 2 Runs, check only TV(P) because it starts from P
        /// </summary>
        public void getMsgId_ForAdhocJob_LegD()
        {
            getMessageIDFromRouteAndRun(RouteName_LegD, RunResourceName_LegD);
        }

        private void getMessageIDFromRouteAndRun(string RouteName, string RunName)
        {
            MsgParams_ForAdhocJob.Clear();
            if (!string.IsNullOrEmpty(RunName)) //have Run
            {
                if (IsUpdateJob)
                {
                    MsgId_ForAdhocJob = 324; //Job ID <b>{0}</b> and Run Resource No. <b>{1}</b> is updated successful. (MessageTextTitle: Updated)
                }
                else
                {
                    MsgId_ForAdhocJob = 321; //Job ID <b>{0}</b> has been generated and assigned to Run Resource No. <b>{1}</b>. (MessageTextTitle: Saved)
                }
                MsgParams_ForAdhocJob.Add(JobNo);       // {0}.
                MsgParams_ForAdhocJob.Add(RunName);     // {1}.
            }
            else if (!string.IsNullOrEmpty(RouteName)) //No Run but have Route
            {
                if (IsUpdateJob)
                {
                    MsgId_ForAdhocJob = 323; //The Route <b>{0}</b> is not available for this day, The Job ID <b>{1}</b> will be unassigned job. (MessageTextTitle: Updated)
                }
                else
                {
                    MsgId_ForAdhocJob = 320; //The Route <b>{0}</b> is not available for this day, The Job ID <b>{1}</b> will be unassigned job. (MessageTextTitle: Saved)
                }
                MsgParams_ForAdhocJob.Add(RouteName);   // {0}.
                MsgParams_ForAdhocJob.Add(JobNo);       // {1}. 
            }
            else //User didn't choose Route and Run 
            {
                if (IsUpdateJob)
                {
                    MsgId_ForAdhocJob = 325; //Job ID <b>{0}</b> is updated successful. (MessageTextTitle: Updated)
                }
                else
                {
                    MsgId_ForAdhocJob = 322; //Job ID <b>{0}</b> has been generated. (MessageTextTitle: Saved)
                }
                MsgParams_ForAdhocJob.Add(JobNo);       // {0}. 
            }
        }
    }
}
