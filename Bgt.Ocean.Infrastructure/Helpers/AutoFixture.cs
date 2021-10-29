using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Infrastructure.Helpers
{
    public static class AutoFixture
    {
    
        public static TDummy CreateDummy<TDummy>() where TDummy : class
            => Fixture.Create<TDummy>();

        public static List<TDummy> CreateDummy<TDummy>(int count) where TDummy : class
           => Fixture.CreateMany<TDummy>(count).ToList();

        public static List<TDummy> CreateDummy<TDummy>(int count, Action<Fixture> setup) where TDummy : class
            => CreateFixture(setup).CreateMany<TDummy>(count).ToList();

        public static TDummy CreateDummy<TDummy>(Action<Fixture> setup) where TDummy : class
            => CreateFixture(setup).Create<TDummy>();

        public static TDummy CreateDummyAndModify<TDummy>(Action<TDummy> modifyData) where TDummy : class
        {
            var dummy = Fixture.Create<TDummy>();
            modifyData(dummy);

            return dummy;
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
