using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bgt.Ocean.Repository.Test.DolphinData
{
    public class MobileATMCheckListEERepositoryTest : BaseTest
    {
        private const string USER_CREATED = "Xunit_GetCheckListEE";
        private readonly IMobileATMCheckListEERepository _repository;

        public MobileATMCheckListEERepositoryTest()
        {
            _repository = new MobileATMCheckListEERepository(
                    _mockDbFactory.Object
                );
        }

        public static IEnumerable<object[]> CheckListData()
        {
        
            var job1= Guid.NewGuid();
            var job2 = Guid.NewGuid();
            var job3 = Guid.NewGuid();
            var job4 = Guid.NewGuid();


            List<TblMobileATMCheckListEE> setup1_expected = new List<TblMobileATMCheckListEE>();
            Action setup1_haschecklist = () =>
            {
              
                using (var db = Util.GetDbContextWithLog())
                {
                    var legguid = Guid.NewGuid();
                    var checklists = Enumerable.Range(0, 5).Select((o, i) => new TblMobileATMCheckListEE
                    {
                        Guid = Guid.NewGuid(),
                        UserCreated = USER_CREATED,
                        FlagIsChecked = i % 2 == 0,
                        MasterActualJobServiceStopLeg_Guid = legguid
                    }).ToList();

                    var leg = new TblMasterActualJobServiceStopLegs();
                    leg.MasterActualJobHeader_Guid = job1;
                    leg.Guid = legguid;

                    db.TblMobileATMCheckListEE.AddRange(checklists);
                    db.TblMasterActualJobServiceStopLegs.Add(leg);

                    db.SaveChanges();

                    setup1_expected.AddRange(checklists);
                }
            };

            List<TblMobileATMCheckListEE> setup2_expected = new List<TblMobileATMCheckListEE>();
            Action setup2_nochecklist = () =>
            {
           
                using (var db = Util.GetDbContextWithLog())
                {
                    var legguid = Guid.NewGuid();
                    var leg = new TblMasterActualJobServiceStopLegs();
                    leg.MasterActualJobHeader_Guid = job2;
                    leg.Guid = legguid;

                    db.TblMasterActualJobServiceStopLegs.Add(leg);
                    db.SaveChanges();
                }
            };

            List<TblMobileATMCheckListEE> setup3_expected = new List<TblMobileATMCheckListEE>();
            Action setup3_allcheckedtrue = () =>
            {
               
                using (var db = Util.GetDbContextWithLog())
                {
                    var legguid = Guid.NewGuid();
                    var checklists = Enumerable.Range(0, 5).Select(o => new TblMobileATMCheckListEE
                    {
                        Guid = Guid.NewGuid(),
                        FlagIsChecked = true,
                        UserCreated = USER_CREATED,
                        MasterActualJobServiceStopLeg_Guid = legguid
                    }).ToList();

                    var leg = new TblMasterActualJobServiceStopLegs();
                    leg.MasterActualJobHeader_Guid = job3;
                    leg.Guid = legguid;

                    db.TblMobileATMCheckListEE.AddRange(checklists);
                    db.TblMasterActualJobServiceStopLegs.Add(leg);

                    db.SaveChanges();

                    setup3_expected.AddRange(checklists);
                }
            };


            List<TblMobileATMCheckListEE> setup4_expected = new List<TblMobileATMCheckListEE>();
            Action setup4_allcheckedfalse = () =>
            {
                using (var db = Util.GetDbContextWithLog())
                {
                    var legguid = Guid.NewGuid();
                    var checklists = Enumerable.Range(0, 5).Select(o => new TblMobileATMCheckListEE
                    {
                        Guid = Guid.NewGuid(),
                        FlagIsChecked = false,
                        UserCreated = USER_CREATED,
                        MasterActualJobServiceStopLeg_Guid = legguid
                    }).ToList();

                    var leg = new TblMasterActualJobServiceStopLegs();
                    leg.MasterActualJobHeader_Guid = job4;
                    leg.Guid = legguid;

                    db.TblMobileATMCheckListEE.AddRange(checklists);
                    db.TblMasterActualJobServiceStopLegs.Add(leg);

                    db.SaveChanges();

                    setup4_expected.AddRange(checklists);
                }
            };
            return new List<object[]>
                {

                    new object[] { job1, setup1_haschecklist, setup1_expected},
                    new object[] { job2, setup2_nochecklist, setup2_expected},
                    new object[] { job3, setup3_allcheckedtrue, setup3_expected},
                    new object[] { job4, setup4_allcheckedfalse, setup4_expected}
                };
        }
        //[Fact(Skip = "Test Script")]
        //[Theory]
        //[MemberData(nameof(CheckListData))]
        public void GetCheckListEE_ShouldHasCorrectList(Guid jobGuid, Action setup, List<TblMobileATMCheckListEE> expected)
        {
            setup();

            var checklist = _repository.GetCheckListEE(jobGuid);
            Assert.NotNull(checklist);
            Assert.All(checklist, data =>
            {
                Assert.Contains(data.Item, expected.Select(o => o.CheckListName));
                Assert.Contains(data.IsChecked, expected.Select(o => o.FlagIsChecked));
            });
            Assert.True(checklist.Count() == expected.Count);
        }
    }
}
