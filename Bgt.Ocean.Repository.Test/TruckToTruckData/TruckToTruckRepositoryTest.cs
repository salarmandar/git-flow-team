using Bgt.Ocean.Infrastructure.Helpers;
using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Run;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Bgt.Ocean.Infrastructure.Util.EnumRoute;

namespace Bgt.Ocean.Repository.Test.TruckToTruckData
{
    public class TruckToTruckRepositoryTest : BaseTest
    {
        private readonly IMasterDailyRunResourceRepository _masterDailyRunResource;

        public TruckToTruckRepositoryTest()
        {
            _masterDailyRunResource = new MasterDailyRunResourceRepository(
                    _mockDbFactory.Object
                );
        }

        public static IEnumerable<object[]> MockDBData()
        {
            var enGuid = Guid.Parse("6fa2bd67-0794-4a9e-a13b-2d81ddb574a0");
            
            var SiteGuid = Guid.NewGuid();

            var OldRunGuid = Guid.NewGuid();
            var NewRunGuid = Guid.NewGuid();

            List<Guid> legGuidList = new List<Guid>();
            legGuidList.AddRange(Enumerable.Range(0, 2).Select((o, i) => Guid.NewGuid()));

            List<TblMasterActualJobHeader> jobList = new List<TblMasterActualJobHeader>();
            jobList.Add(new TblMasterActualJobHeader() {
                Guid = Guid.NewGuid(),
                SystemStatusJobID = JobStatusHelper.OnTruck,
                JobTypeID = IntTypeJob.P
            });
            jobList.Add(new TblMasterActualJobHeader()
            {
                Guid = Guid.NewGuid(),
                SystemStatusJobID = JobStatusHelper.OnTruck,
                JobTypeID = IntTypeJob.TV
            });


            Action setup = () =>
            {

                using (var db = Util.GetDbContextWithLog())
                {         
                    var RunList = new List<TblMasterDailyRunResource>();
                    RunList.Add(new TblMasterDailyRunResource() {
                        Guid = NewRunGuid,
                        RunResourceDailyStatusID = DailyRunStatus.Ready,
                        MasterSite_Guid = SiteGuid,
                        StartTime = DateTime.Now,
                        FlagSentAutoDailyPlanReport = false
                    }); // New Run
                    RunList.Add(new TblMasterDailyRunResource() {
                        Guid = OldRunGuid,
                        RunResourceDailyStatusID = DailyRunStatus.Ready,
                        MasterSite_Guid = SiteGuid,
                        StartTime = DateTime.Now,
                        FlagSentAutoDailyPlanReport = false
                    }); // old Run

                    var LegList = new List<TblMasterActualJobServiceStopLegs>();
                    foreach(var itemlegGuidList in legGuidList)
                    {
                        LegList.Add(new TblMasterActualJobServiceStopLegs() {
                            Guid = itemlegGuidList,
                            MasterRunResourceDaily_Guid = OldRunGuid,
                            MasterActualJobHeader_Guid = Guid.NewGuid()
                        });
                    }
                    
                    foreach (var itemjobList in jobList)
                    {
                        LegList.Add(new TblMasterActualJobServiceStopLegs()
                        {
                            Guid = Guid.NewGuid(),
                            MasterRunResourceDaily_Guid = NewRunGuid,
                            MasterActualJobHeader_Guid = Guid.NewGuid()
                        });
                    }

                    var MessageList = new List<TblSystemMessage>();
                    MessageList.Add(new TblSystemMessage() { Guid = Guid.NewGuid(), SystemLanguage_Guid = enGuid, MsgID = -2157, MessageTextContent = "Error Text 2157" });
                    MessageList.Add(new TblSystemMessage() { Guid = Guid.NewGuid(), SystemLanguage_Guid = enGuid, MsgID = -2158, MessageTextContent = "Error Text 2158" });
                    MessageList.Add(new TblSystemMessage() { Guid = Guid.NewGuid(), SystemLanguage_Guid = enGuid, MsgID = -2159, MessageTextContent = "Error Text 2159" });
                    MessageList.Add(new TblSystemMessage() { Guid = Guid.NewGuid(), SystemLanguage_Guid = enGuid, MsgID = -2160, MessageTextContent = "Error Text 2160" });
                    MessageList.Add(new TblSystemMessage() { Guid = Guid.NewGuid(), SystemLanguage_Guid = enGuid, MsgID = -2161, MessageTextContent = "Error Text 2161" });
                    MessageList.Add(new TblSystemMessage() { Guid = Guid.NewGuid(), SystemLanguage_Guid = enGuid, MsgID = -2162, MessageTextContent = "Error Text 2162" });

                    db.TblMasterDailyRunResource.AddRange(RunList);
                    db.TblMasterActualJobServiceStopLegs.AddRange(LegList);                    
                    db.TblSystemMessage.AddRange(MessageList);

                    db.SaveChanges();                   
                }
            };          

            return new List<object[]>
                {
                    new object[] { jobList, legGuidList, OldRunGuid , NewRunGuid , enGuid, setup}
                };
        }

        //[Theory]
        //[MemberData(nameof(MockDBData))]
        private void ValidateTruckToTruck(List<TblMasterActualJobHeader> jobList , List<Guid> legGuidList , Guid oldDailyRunGuid, Guid newDailyRunGuid, Guid languageGuid, Action setup)
        {
            setup();

            var result = _masterDailyRunResource.TruckToTruckIsValidRun(oldDailyRunGuid, newDailyRunGuid, languageGuid);
            Assert.True(result.isSuccess);

            result = _masterDailyRunResource.TruckToTruckIsValidJobStatus(jobList, languageGuid);
            Assert.True(result.isSuccess);

            result = _masterDailyRunResource.TruckToTruckIsValidlegListMustBeInOldRun(legGuidList, oldDailyRunGuid, languageGuid);
            Assert.True(result.isSuccess);

            legGuidList.Add(Guid.NewGuid());
            result = _masterDailyRunResource.TruckToTruckIsValidlegListMustBeInOldRun(legGuidList, oldDailyRunGuid, languageGuid);
            Assert.True(!result.isSuccess);

            result = _masterDailyRunResource.TruckToTruckIsValidServiceJobType(jobList, newDailyRunGuid, languageGuid);
            Assert.True(result.isSuccess);
        }





    }
}
