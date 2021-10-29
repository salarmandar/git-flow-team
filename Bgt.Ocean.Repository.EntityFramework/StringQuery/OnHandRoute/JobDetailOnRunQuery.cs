using Bgt.Ocean.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bgt.Ocean.Repository.EntityFramework.StringQuery.OnHandRoute
{
    public class JobDetailOnRunQuery
    {
        public string GetJobDetailOnRun
        {
            get
            {
                string schemaTxt = EnumGlobalEnvironment.IsStaging() ? "stg" : "dbo";
                return @"DECLARE @TableServiceType Table(Guid			UNIQUEIDENTIFIER,
						 		                         DisplayText	NVARCHAR(200))
                         INSERT INTO @TableServiceType
                         SELECT	lang.Guid,lang.DisplayText
                         FROM	TblSystemDisplayTextControlsLanguage lang
                         WHERE	lang.SystemLanguageGuid = @LanguageGuid			
                         		AND lang.Guid IN (SELECT jt1.SystemDisplayTextControlsAbb_Guid -- Abbr Name
                         					      FROM TblSystemServiceJobType jt1
                         					      UNION 
                         					      SELECT jt2.SystemDisplayTextControlsName_Guid -- Full Name
                         					      FROM TblSystemServiceJobType jt2)

                         SELECT	l.MasterRunResourceDaily_Guid as DailyRunGuid,
		                        h.Guid as JobGuid,
		                        l.JobOrder as JobSeq,
		                        h.JobNo as JobID,
		                        jt.ServiceJobTypeID as JobTypeID,
		                        (SELECT [" + schemaTxt + @"].[GetStringJobTypeMultibranchByLanguage](@LanguageGuid, h.FlagJobInterBranch,h.FlagMultiBranch,
										ISNULL((SELECT DisplayText 
										        FROM @TableServiceType TblJobTypeLangugae 
												WHERE TblJobTypeLangugae.Guid = jt.SystemDisplayTextControlsAbb_Guid),
												ISNULL(jt.ServiceJobTypeNameAbb,'')
											  ))
								) as JobType,
		                        st.StatusJobID as JobStatusID,
		                        ISNULL((SELECT [" + schemaTxt + @"].[GetStringJobStatusInterbranchByLanguage](@LanguageGuid, h.SystemStatusJobID,
										IIF(h.FlagJobInterBranch = 1 OR h.FlagMultiBranch=1,1,0),
										    h.FlagChkOutInterBranchComplete,st.StatusJobName,l.SequenceStop,jt.ServiceJobTypeID)),'') 
								as JobStatus,
		                        ac.ActionNameAbbrevaition as JobAction,
		                        lob.LOBAbbrevaitionName as LOB,
		                        mc.MachineID,
		                        ISNULL(c.CustomerFullName,'')+ ' - ' + ISNULL(cl.BranchName,'') as LocationName,
								jtGrpName.ServiceJobTypeID as GroupJobTypeID,
								jtGrpName.DisplayText as GroupJobTypeName
                         FROM TblMasterActualJobServiceStopLegs l
                         INNER JOIN TblMasterActualJobHeader h ON h.Guid = l.MasterActualJobHeader_Guid 
                         										AND l.MasterRunResourceDaily_Guid IN ({DailyRunGuid})
                         										AND ((@FlagIncludeJobCancel = 0 and h.FlagCancelAll = 0) 
                                                                     or @FlagIncludeJobCancel = 1)
                         INNER JOIN TblSystemServiceJobType jt ON jt.Guid = h.SystemServiceJobType_Guid
                         INNER JOIN TblSystemJobStatus st ON st.StatusJobID = h.SystemStatusJobID
                         INNER JOIN TblSystemJobAction ac ON ac.Guid = l.CustomerLocationAction_Guid
                         INNER JOIN TblSystemLineOfBusiness lob ON lob.Guid = h.SystemLineOfBusiness_Guid
                         INNER JOIN TblMasterCustomerLocation cl ON cl.Guid = l.MasterCustomerLocation_Guid
                         INNER JOIN TblMasterCustomer c ON c.Guid = cl.MasterCustomer_Guid and FlagChkCustomer = 1
                         LEFT JOIN sfo.SFOTblMasterMachine mc ON mc.Guid = cl.Guid
                         OUTER APPLY (SELECT TOP 1 lan.DisplayText,jtname.ServiceJobTypeID
									  FROM	TblSystemDisplayTextControlsLanguage lan
									  INNER JOIN TblSystemServiceJobType jtname on jtname.SystemDisplayTextControlsName_Guid = lan.Guid
									  WHERE	lan.SystemLanguageGuid = @LanguageGuid 
									        AND jtname.ServiceJobTypeID = (CASE WHEN jt.ServiceJobTypeID = 14 THEN 0
																				WHEN jt.ServiceJobTypeID = 15 THEN 2
																				WHEN jt.ServiceJobTypeID = 16 THEN 11
																				ELSE jt.ServiceJobTypeID
																		   END)
											) as jtGrpName
                         ORDER BY l.JobOrder,h.JobNo";
            }
        }
    }
}
