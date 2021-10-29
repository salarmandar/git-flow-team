using AutoFixture;
using Bgt.Ocean.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Repository.Test
{
    public static class Util
    {
        public static Mock<TMock> CreateMock<TMock>() where TMock : class 
            => new Mock<TMock>();

        public static TDummy CreateDummy<TDummy>() where TDummy : class
            => Fixture.Create<TDummy>();

        public static TDummy CreateDummy<TDummy>(Action<Fixture> setup) where TDummy : class
            => CreateFixture(setup).Create<TDummy>();

        public static TDummy CreateDummyAndModify<TDummy>(Action<TDummy> modifyData) where TDummy : class
        {
            var dummy = Fixture.Create<TDummy>();
            modifyData(dummy);

            return dummy;
        }

        public static List<TDummy> CreateDummy<TDummy>(int count) where TDummy : class
            => Fixture.CreateMany<TDummy>(count).ToList();

        public static List<TDummy> CreateDummy<TDummy>(int count, Action<Fixture> setup) where TDummy : class
            => CreateFixture(setup).CreateMany<TDummy>(count).ToList();
        
        public static OceanDbEntities GetDbContextWithLog()
        {
            var db = new OceanDbEntities();
            db.Database.Log = log => System.Diagnostics.Debug.WriteLine(log);

            return db;
        }

        public static string RandomString(int length)
        {
            var random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static Fixture CreateFixture(Action<Fixture> setup)
        {
            var fixture = new Fixture();

            setup(fixture);

            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            return fixture;
        }

        private static Fixture Fixture
        {
            get
            {
                var fixture = new Fixture();
                fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
                fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

                return fixture;
            }
        }
            
    }

    

}
