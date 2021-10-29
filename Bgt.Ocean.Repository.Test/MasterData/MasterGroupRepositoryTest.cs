using Bgt.Ocean.Models;
using Bgt.Ocean.Repository.EntityFramework.Repositories.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bgt.Ocean.Repository.Test.MasterData
{
    public class MasterGroupRepositoryTest
    {
        public class GetPermittedGroupWithCorrectSiteHandle : BaseTest
        {
            private const string USER_CREATED = "Xunit_GetPermittedGroup";
            private readonly IMasterGroupRepository _repository;

            public GetPermittedGroupWithCorrectSiteHandle()
            {
                _repository = new MasterGroupRepository(
                        _mockDbFactory.Object
                    );
            }

            public static IEnumerable<object[]> CorrectSiteHandle()
            {
                #region Arrange 1 Single Group allow all

                var siteGuid1 = Guid.NewGuid();
                var groupGuidList1 = new List<Guid>(1);

                Action setup1 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup = Util.CreateDummy<TblMasterGroup>();
                        newGroup.FlagAllowAcknowledgeAlarm = true;
                        newGroup.FlagAllowDeactivateAlarm = true;
                        newGroup.FlagDisable = false;
                        newGroup.UserCreated = USER_CREATED;

                        var newGroupSiteHandle = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle.MasterSite_Guid = siteGuid1;
                        newGroupSiteHandle.Description = USER_CREATED;
                        newGroupSiteHandle.MasterGroup_Guid = newGroup.Guid;

                        db.TblMasterGroup.Add(newGroup);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle);

                        db.SaveChanges();

                        groupGuidList1.Add(newGroup.Guid);
                    }
                };

                #endregion

                #region Arrange 2 Multiple Group mixing with allow all, allow some and not allow

                var siteGuid2 = Guid.NewGuid();
                var groupGuidList2 = new List<Guid>(2);

                Action setup2 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup1 = Util.CreateDummy<TblMasterGroup>();
                        newGroup1.FlagAllowAcknowledgeAlarm = true;
                        newGroup1.FlagAllowDeactivateAlarm = true;
                        newGroup1.FlagDisable = false;
                        newGroup1.UserCreated = USER_CREATED;

                        var newGroup2 = Util.CreateDummy<TblMasterGroup>();
                        newGroup2.FlagAllowAcknowledgeAlarm = true;
                        newGroup2.FlagAllowDeactivateAlarm = false;
                        newGroup2.FlagDisable = false;
                        newGroup2.UserCreated = USER_CREATED;

                        var newGroup3 = Util.CreateDummy<TblMasterGroup>();
                        newGroup3.FlagAllowAcknowledgeAlarm = false;
                        newGroup3.FlagAllowDeactivateAlarm = true;
                        newGroup3.FlagDisable = false;
                        newGroup3.UserCreated = USER_CREATED;

                        var newGroup4 = Util.CreateDummy<TblMasterGroup>();
                        newGroup3.FlagAllowAcknowledgeAlarm = false;
                        newGroup3.FlagAllowDeactivateAlarm = false;
                        newGroup3.FlagDisable = false;
                        newGroup3.UserCreated = USER_CREATED;

                        // same site has multiple group handle
                        var newGroupSiteHandle1 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle1.MasterSite_Guid = siteGuid2;
                        newGroupSiteHandle1.Description = USER_CREATED;
                        newGroupSiteHandle1.MasterGroup_Guid = newGroup1.Guid;

                        var newGroupSiteHandle2 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle2.MasterSite_Guid = siteGuid2;
                        newGroupSiteHandle2.Description = USER_CREATED;
                        newGroupSiteHandle2.MasterGroup_Guid = newGroup2.Guid;

                        var newGroupSiteHandle3 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle3.MasterSite_Guid = siteGuid2;
                        newGroupSiteHandle3.Description = USER_CREATED;
                        newGroupSiteHandle3.MasterGroup_Guid = newGroup3.Guid;

                        var newGroupSiteHandle4 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle4.MasterSite_Guid = siteGuid2;
                        newGroupSiteHandle4.Description = USER_CREATED;
                        newGroupSiteHandle4.MasterGroup_Guid = newGroup4.Guid;

                        db.TblMasterGroup.Add(newGroup1);
                        db.TblMasterGroup.Add(newGroup2);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle1);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle2);

                        db.SaveChanges();

                        groupGuidList2.Add(newGroup1.Guid);
                        groupGuidList2.Add(newGroup2.Guid);
                        groupGuidList2.Add(newGroup3.Guid);
                        groupGuidList2.Add(newGroup4.Guid);
                    }
                };

                #endregion

                #region Arrange 3 Single Group allow only acknowledge

                var siteGuid3 = Guid.NewGuid();
                var groupGuidList3 = new List<Guid>(1);

                Action setup3 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup = Util.CreateDummy<TblMasterGroup>();
                        newGroup.FlagAllowAcknowledgeAlarm = true;
                        newGroup.FlagAllowDeactivateAlarm = false;
                        newGroup.FlagDisable = false;
                        newGroup.UserCreated = USER_CREATED;

                        var newGroupSiteHandle = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle.MasterSite_Guid = siteGuid3;
                        newGroupSiteHandle.Description = USER_CREATED;
                        newGroupSiteHandle.MasterGroup_Guid = newGroup.Guid;

                        db.TblMasterGroup.Add(newGroup);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle);

                        db.SaveChanges();

                        groupGuidList3.Add(newGroup.Guid);
                    }
                };

                #endregion

                #region Arrange 4 Single Group allow only deactivate

                var siteGuid4 = Guid.NewGuid();
                var groupGuidList4 = new List<Guid>(1);

                Action setup4 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup = Util.CreateDummy<TblMasterGroup>();
                        newGroup.FlagAllowAcknowledgeAlarm = false;
                        newGroup.FlagAllowDeactivateAlarm = true;
                        newGroup.FlagDisable = false;
                        newGroup.UserCreated = USER_CREATED;

                        var newGroupSiteHandle = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle.MasterSite_Guid = siteGuid4;
                        newGroupSiteHandle.Description = USER_CREATED;
                        newGroupSiteHandle.MasterGroup_Guid = newGroup.Guid;

                        db.TblMasterGroup.Add(newGroup);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle);

                        db.SaveChanges();

                        groupGuidList4.Add(newGroup.Guid);
                    }
                };

                #endregion

                #region Arrange 5 Multiple Group allow only acknowledge

                var siteGuid5 = Guid.NewGuid();
                var groupGuidList5 = new List<Guid>(2);

                Action setup5 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup1 = Util.CreateDummy<TblMasterGroup>();
                        newGroup1.FlagAllowAcknowledgeAlarm = true;
                        newGroup1.FlagAllowDeactivateAlarm = false;
                        newGroup1.FlagDisable = false;
                        newGroup1.UserCreated = USER_CREATED;

                        var newGroup2 = Util.CreateDummy<TblMasterGroup>();
                        newGroup2.FlagAllowAcknowledgeAlarm = true;
                        newGroup2.FlagAllowDeactivateAlarm = false;
                        newGroup2.FlagDisable = false;
                        newGroup2.UserCreated = USER_CREATED;

                        // same site has multiple group handle
                        var newGroupSiteHandle1 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle1.MasterSite_Guid = siteGuid5;
                        newGroupSiteHandle1.Description = USER_CREATED;
                        newGroupSiteHandle1.MasterGroup_Guid = newGroup1.Guid;

                        var newGroupSiteHandle2 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle2.MasterSite_Guid = siteGuid5;
                        newGroupSiteHandle2.Description = USER_CREATED;
                        newGroupSiteHandle2.MasterGroup_Guid = newGroup2.Guid;

                        db.TblMasterGroup.Add(newGroup1);
                        db.TblMasterGroup.Add(newGroup2);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle1);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle2);

                        db.SaveChanges();

                        groupGuidList5.Add(newGroup1.Guid);
                        groupGuidList5.Add(newGroup2.Guid);
                    }
                };

                #endregion

                #region Arrange 6 Multiple Group allow only deactivate

                var siteGuid6 = Guid.NewGuid();
                var groupGuidList6 = new List<Guid>(2);

                Action setup6 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup1 = Util.CreateDummy<TblMasterGroup>();
                        newGroup1.FlagAllowAcknowledgeAlarm = false;
                        newGroup1.FlagAllowDeactivateAlarm = true;
                        newGroup1.FlagDisable = false;
                        newGroup1.UserCreated = USER_CREATED;

                        var newGroup2 = Util.CreateDummy<TblMasterGroup>();
                        newGroup2.FlagAllowAcknowledgeAlarm = false;
                        newGroup2.FlagAllowDeactivateAlarm = true;
                        newGroup2.FlagDisable = false;
                        newGroup2.UserCreated = USER_CREATED;

                        // same site has multiple group handle
                        var newGroupSiteHandle1 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle1.MasterSite_Guid = siteGuid6;
                        newGroupSiteHandle1.Description = USER_CREATED;
                        newGroupSiteHandle1.MasterGroup_Guid = newGroup1.Guid;

                        var newGroupSiteHandle2 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle2.MasterSite_Guid = siteGuid6;
                        newGroupSiteHandle2.Description = USER_CREATED;
                        newGroupSiteHandle2.MasterGroup_Guid = newGroup2.Guid;

                        db.TblMasterGroup.Add(newGroup1);
                        db.TblMasterGroup.Add(newGroup2);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle1);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle2);

                        db.SaveChanges();

                        groupGuidList6.Add(newGroup1.Guid);
                        groupGuidList6.Add(newGroup2.Guid);
                    }
                };

                #endregion

                #region Arrange 7 has 2 groups, 1 allow all, 1 not allow all

                var siteGuid7 = Guid.NewGuid();
                var groupGuidList7 = new List<Guid>(2);

                Action setup7 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup1 = Util.CreateDummy<TblMasterGroup>();
                        newGroup1.FlagAllowAcknowledgeAlarm = true;
                        newGroup1.FlagAllowDeactivateAlarm = true;
                        newGroup1.FlagDisable = false;
                        newGroup1.UserCreated = USER_CREATED;

                        var newGroup2 = Util.CreateDummy<TblMasterGroup>();
                        newGroup2.FlagAllowAcknowledgeAlarm = false;
                        newGroup2.FlagAllowDeactivateAlarm = false;
                        newGroup2.FlagDisable = false;
                        newGroup2.UserCreated = USER_CREATED;

                        // same site has multiple group handle
                        var newGroupSiteHandle1 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle1.MasterSite_Guid = siteGuid7;
                        newGroupSiteHandle1.Description = USER_CREATED;
                        newGroupSiteHandle1.MasterGroup_Guid = newGroup1.Guid;

                        var newGroupSiteHandle2 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle2.MasterSite_Guid = siteGuid7;
                        newGroupSiteHandle2.Description = USER_CREATED;
                        newGroupSiteHandle2.MasterGroup_Guid = newGroup2.Guid;

                        db.TblMasterGroup.Add(newGroup1);
                        db.TblMasterGroup.Add(newGroup2);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle1);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle2);

                        db.SaveChanges();

                        groupGuidList7.Add(newGroup1.Guid);
                    }
                };

                #endregion

                #region Arrange 8 has 2 groups, 1 allow some, 1 not allow all

                var siteGuid8 = Guid.NewGuid();
                var groupGuidList8 = new List<Guid>(2);

                Action setup8 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup1 = Util.CreateDummy<TblMasterGroup>();
                        newGroup1.FlagAllowAcknowledgeAlarm = true;
                        newGroup1.FlagAllowDeactivateAlarm = false;
                        newGroup1.FlagDisable = false;
                        newGroup1.UserCreated = USER_CREATED;

                        var newGroup2 = Util.CreateDummy<TblMasterGroup>();
                        newGroup2.FlagAllowAcknowledgeAlarm = false;
                        newGroup2.FlagAllowDeactivateAlarm = false;
                        newGroup2.FlagDisable = false;
                        newGroup2.UserCreated = USER_CREATED;

                        // same site has multiple group handle
                        var newGroupSiteHandle1 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle1.MasterSite_Guid = siteGuid8;
                        newGroupSiteHandle1.Description = USER_CREATED;
                        newGroupSiteHandle1.MasterGroup_Guid = newGroup1.Guid;

                        var newGroupSiteHandle2 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle2.MasterSite_Guid = siteGuid8;
                        newGroupSiteHandle2.Description = USER_CREATED;
                        newGroupSiteHandle2.MasterGroup_Guid = newGroup2.Guid;

                        db.TblMasterGroup.Add(newGroup1);
                        db.TblMasterGroup.Add(newGroup2);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle1);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle2);

                        db.SaveChanges();

                        groupGuidList8.Add(newGroup1.Guid);
                    }
                };

                #endregion

                return new List<object[]>
                {
                    new object[] { siteGuid1, groupGuidList1, setup1 },
                    new object[] { siteGuid2, groupGuidList2, setup2 },
                    new object[] { siteGuid3, groupGuidList3, setup3 },
                    new object[] { siteGuid4, groupGuidList4, setup4 },
                    new object[] { siteGuid5, groupGuidList5, setup5 },
                    new object[] { siteGuid6, groupGuidList6, setup6 },
                    new object[] { siteGuid7, groupGuidList7, setup7 },
                    new object[] { siteGuid8, groupGuidList8, setup8 }
                };
            }

            //[Theory]
            //[MemberData(nameof(CorrectSiteHandle))]
            public void CorrectSiteHandle_ShouldHasCorrectGroup(Guid siteGuid, IEnumerable<Guid> groupGuidList, Action setup)
            {
                setup();

                var masterGroupList = _repository.GetPermittedAlarmGroupBySite(siteGuid);

                Assert.NotEmpty(masterGroupList);
                Assert.All(masterGroupList, data =>
                {
                    Assert.True(data.FlagAllowAcknowledge || data.FlagAllowDeactivate);
                    Assert.Contains(siteGuid, data.MasterSiteHandleList);
                });

                Assert.True(groupGuidList.Union(masterGroupList.Select(e => e.Guid)).Count() == groupGuidList.Count());
            }

            public static IEnumerable<object[]> IncorrectSiteHandle()
            {
                #region Arrange 1 (incorrect site guid)

                var incorrectSite1 = Guid.NewGuid();

                Action setup1 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup = Util.CreateDummy<TblMasterGroup>();
                        newGroup.FlagAllowAcknowledgeAlarm = true;
                        newGroup.FlagAllowDeactivateAlarm = true;
                        newGroup.FlagDisable = false;
                        newGroup.UserCreated = USER_CREATED;

                        var newGroupSiteHandle = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle.MasterSite_Guid = Guid.NewGuid();
                        newGroupSiteHandle.Description = USER_CREATED;
                        newGroupSiteHandle.MasterGroup_Guid = newGroup.Guid;

                        db.TblMasterGroup.Add(newGroup);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle);

                        db.SaveChanges();
                    }

                };

                #endregion

                #region Arrange 2 (correct site guid but not allow both acknowledge and deactivate)

                var incorrectSite2 = Guid.NewGuid();

                Action setup2 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup = Util.CreateDummy<TblMasterGroup>();
                        newGroup.FlagAllowAcknowledgeAlarm = false;
                        newGroup.FlagAllowDeactivateAlarm = false;
                        newGroup.FlagDisable = false;
                        newGroup.UserCreated = USER_CREATED;

                        var newGroupSiteHandle = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle.MasterSite_Guid = incorrectSite2;
                        newGroupSiteHandle.Description = USER_CREATED;
                        newGroupSiteHandle.MasterGroup_Guid = newGroup.Guid;

                        db.TblMasterGroup.Add(newGroup);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle);

                        db.SaveChanges();
                    }

                };

                #endregion

                return new List<object[]>
                {
                    new object[] { incorrectSite1, setup1 },
                    new object[] { incorrectSite2, setup2 }
                };

            }

            //[Theory]
            //[MemberData(nameof(IncorrectSiteHandle))]
            public void IncorrectSiteHandle_ShouldEmptyGroup(Guid incorrectSiteGuid, Action setup)
            {
                setup();
                var response = _repository.GetPermittedAlarmGroupBySite(incorrectSiteGuid);
                Assert.Empty(response);
            }



            public override void Dispose()
            {
                lock (_lockDbEntities)
                {
                    using (var db = GetOODbContext())
                    {
                        var newGroup = db.TblMasterGroup.Where(e => e.UserCreated == USER_CREATED);
                        var newGroupSite = db.TblMasterGroup_Site.Where(e => e.Description == USER_CREATED);
                        var newGroupMenuCommand = db.TblMasterGroup_MenuCommand.Where(e => newGroup.Any(g => g.Guid == e.MasterGroup_Guid));
                        
                        db.TblMasterGroup_MenuCommand.RemoveRange(newGroupMenuCommand);
                        db.TblMasterGroup_Site.RemoveRange(newGroupSite);
                        db.TblMasterGroup.RemoveRange(newGroup);

                        db.TblMasterMenuDetailCommand.RemoveRange(newGroupMenuCommand.Select(e => e.TblMasterMenuDetailCommand));
                        db.TblMasterMenuDetail.RemoveRange(newGroupMenuCommand.Select(e => e.TblMasterMenuDetailCommand.TblMasterMenuDetail));

                        db.SaveChanges();
                    }
                }
            }

        }

        public class GetPermittedGroupByUser : BaseTest
        {
            private const string USER_CREATED = "Xunit_GetPermittedGroupByUser";
            private readonly IMasterGroupRepository _repository;

            public GetPermittedGroupByUser()
            {
                _repository = new MasterGroupRepository(
                        _mockDbFactory.Object
                    );
            }


            public static IEnumerable<object[]> CorrectUser()
            {
                #region Arrange 1 (Single group has correct user)

                var userGuid1 = Guid.NewGuid();
                var groupList1 = new List<Guid>();

                Action setup1 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup = Util.CreateDummy<TblMasterGroup>();
                        newGroup.FlagAllowAcknowledgeAlarm = true;
                        newGroup.FlagAllowDeactivateAlarm = true;
                        newGroup.FlagDisable = false;
                        newGroup.UserCreated = USER_CREATED;

                        var newUserGroup = Util.CreateDummy<TblMasterUserGroup>();
                        newUserGroup.MasterUser_Guid = userGuid1;
                        newUserGroup.MasterGroup_Guid = newGroup.Guid;
                        newUserGroup.UserCreated = USER_CREATED;

                        var newGroupSiteHandle = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle.MasterSite_Guid = Guid.NewGuid();
                        newGroupSiteHandle.Description = USER_CREATED;
                        newGroupSiteHandle.MasterGroup_Guid = newGroup.Guid;

                        var newGroupSiteHandle2 = Util.CreateDummy<TblMasterGroup_Site>();
                        newGroupSiteHandle.MasterSite_Guid = Guid.NewGuid();
                        newGroupSiteHandle.Description = USER_CREATED;
                        newGroupSiteHandle.MasterGroup_Guid = newGroup.Guid;

                        db.TblMasterGroup.Add(newGroup);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle);
                        db.TblMasterGroup_Site.Add(newGroupSiteHandle2);
                        db.TblMasterUserGroup.Add(newUserGroup);

                        db.SaveChanges();

                        groupList1.Add(newGroup.Guid);
                    }
                };

                #endregion

                #region Arrange 1 (multiple group has correct group returned)

                var userGuid2 = Guid.NewGuid();
                var groupList2 = new List<Guid>();

                Action setup2 = () =>
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        db.Database.Log = log => System.Diagnostics.Debug.WriteLine(log);

                        var siteGuid = Guid.NewGuid();

                        var newGroup = Util.CreateDummy<TblMasterGroup>();
                        newGroup.FlagAllowAcknowledgeAlarm = true;
                        newGroup.FlagAllowDeactivateAlarm = true;
                        newGroup.FlagDisable = false;
                        newGroup.UserCreated = USER_CREATED;

                        var newGroup2 = Util.CreateDummy<TblMasterGroup>();
                        newGroup2.FlagAllowAcknowledgeAlarm = true;
                        newGroup2.FlagAllowDeactivateAlarm = false;
                        newGroup2.FlagDisable = false;
                        newGroup2.UserCreated = USER_CREATED;

                        var newGroup3 = Util.CreateDummy<TblMasterGroup>();
                        newGroup3.FlagAllowAcknowledgeAlarm = false;
                        newGroup3.FlagAllowDeactivateAlarm = true;
                        newGroup3.FlagDisable = false;
                        newGroup3.UserCreated = USER_CREATED;

                        var newGroup4 = Util.CreateDummy<TblMasterGroup>();
                        newGroup4.FlagAllowAcknowledgeAlarm = false;
                        newGroup4.FlagAllowDeactivateAlarm = false;
                        newGroup4.FlagDisable = false;
                        newGroup4.UserCreated = USER_CREATED;

                        var newUserGroupList = Util.CreateDummy<TblMasterUserGroup>(4)
                        .Select(e =>
                        {
                            e.MasterUser_Guid = userGuid2;
                            e.UserCreated = USER_CREATED;
                            e.FlagDisable = false;
                            return e;
                        })
                        .ToList();

                        newUserGroupList[0].MasterGroup_Guid = newGroup.Guid;
                        newUserGroupList[1].MasterGroup_Guid = newGroup2.Guid;
                        newUserGroupList[2].MasterGroup_Guid = newGroup3.Guid;
                        newUserGroupList[3].MasterGroup_Guid = newGroup4.Guid;

                        var newGroupSiteHandle = newUserGroupList.Select(e =>
                        {
                            var newData = Util.CreateDummy<TblMasterGroup_Site>();
                            newData.MasterSite_Guid = siteGuid;
                            newData.Description = USER_CREATED;
                            newData.MasterGroup_Guid = newGroup.Guid;

                            return newData;
                        }).ToList();

                        newGroupSiteHandle[0].MasterGroup_Guid = newGroup.Guid;
                        newGroupSiteHandle[1].MasterGroup_Guid = newGroup2.Guid;
                        newGroupSiteHandle[2].MasterGroup_Guid = newGroup3.Guid;
                        newGroupSiteHandle[3].MasterGroup_Guid = newGroup4.Guid;

                        db.TblMasterGroup.Add(newGroup);
                        db.TblMasterGroup.Add(newGroup2);
                        db.TblMasterGroup.Add(newGroup3);
                        db.TblMasterGroup.Add(newGroup4);

                        db.TblMasterGroup_Site.AddRange(newGroupSiteHandle);
                        db.TblMasterUserGroup.AddRange(newUserGroupList);

                        db.SaveChanges();

                        groupList2.AddRange(new Guid[] { newGroup.Guid, newGroup2.Guid, newGroup3.Guid });
                    }
                };

                #endregion

                return new List<object[]>
                {
                    new object[] { userGuid1, setup1, groupList1 },
                    new object[] { userGuid2, setup2, groupList2 }
                };
            }

            //[Theory]
            //[MemberData(nameof(CorrectUser))]
            public void CorrectUserAndGroupAllowAll_ShouldHasCorrectGroup(Guid userGuid, Action setup, IEnumerable<Guid> expectingGroupList)
            {
                setup();

                var response = _repository.GetPermittedAlarmGroupByUser(userGuid);

                Assert.NotEmpty(response);
                Assert.All(response, data =>
                {
                    Assert.True(data.FlagAllowAcknowledge || data.FlagAllowDeactivate);
                });

                Assert.True(expectingGroupList.Union(response.Select(e => e.Guid)).Count() == expectingGroupList.Count());

            }


            public override void Dispose()
            {
                lock (_lockDbEntities)
                {
                    using (var db = Util.GetDbContextWithLog())
                    {
                        var newGroup = db.TblMasterGroup.Where(e => e.UserCreated == USER_CREATED);
                        var newGroupSite = db.TblMasterGroup_Site.Where(e => e.Description == USER_CREATED);
                        var newGroupMenuCommand = db.TblMasterGroup_MenuCommand.Where(e => newGroup.Any(g => g.Guid == e.MasterGroup_Guid));
                        var newUserGroup = db.TblMasterUserGroup.Where(e => e.UserCreated == USER_CREATED);

                        db.TblMasterGroup_MenuCommand.RemoveRange(newGroupMenuCommand);
                        db.TblMasterGroup_Site.RemoveRange(newGroupSite);
                        db.TblMasterUserGroup.RemoveRange(newUserGroup);
                        db.TblMasterGroup.RemoveRange(newGroup);

                        db.TblMasterMenuDetailCommand.RemoveRange(newGroupMenuCommand.Select(e => e.TblMasterMenuDetailCommand));
                        db.TblMasterMenuDetail.RemoveRange(newGroupMenuCommand.Select(e => e.TblMasterMenuDetailCommand.TblMasterMenuDetail));

                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
