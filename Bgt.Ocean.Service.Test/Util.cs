using AutoFixture;
using Bgt.Ocean.Infrastructure.Configuration;
using Bgt.Ocean.Infrastructure.Storages;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Bgt.Ocean.Service.Test
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

        public static object InvokeMethod<T>(this T obj, string methodName, params object[] param) where T : class
        {
            MethodInfo privMethod = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            return privMethod.Invoke(obj, param);
        }
        private static object DynamicMock(Type type)
        {
            var mock = typeof(Mock<>).MakeGenericType(type).GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            return mock.GetType().GetProperties().Single(f => f.Name == "Object" && f.PropertyType == type).GetValue(mock, new object[] { });
        }
        public static T CreateInstance<T>(string userName, Guid userLanguageGuid = default(Guid), string userFormatDate = "dd/MM/yyyy") where T : class
        {
            ApiSession.UserLanguage_Guid = userLanguageGuid;
            ApiSession.UserName = userName;
            ApiSession.UserFormatDate = userFormatDate;
            return CreateInstance<T>();
        }
        public static T CreateInstance<T>() where T : class
        {
            var constructorInfo = typeof(T).GetConstructors();
            foreach (var item in constructorInfo)
            {
                if (item == null) continue;
                var lobject = item.GetParameters().Select(o => { return DynamicMock(o.ParameterType); }).ToArray();
                return (T)item.Invoke(lobject);
            }
            return default(T);
        }

        public static T CreateInstanceWithRepository<T>(List<object> di)
        {
            var constructorInfo = typeof(T).GetConstructors();
            foreach (var item in constructorInfo)
            {
                if (item == null) continue;
                var lobject = item.GetParameters().Select(o =>
                {
                    var repo = di.FirstOrDefault(d => d.GetType().GetInterfaces().Any(i => i == o.ParameterType));
                    if (repo != null)
                    {   
                        //manual mock repo
                        return repo;
                    }
                    else
                    {
                        //auto mock repo
                        return DynamicMock(o.ParameterType);
                    }
                }).ToArray();
                return (T)item.Invoke(lobject);
            }
            return default(T);
        }

        public static Mock<T> GetMock<T>(this object obj, string fieldName) where T : class
        {
            var f = (T)obj.GetType()
                          .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                          .GetValue(obj);
            return Mock.Get(f);
        }
        public static Mock<T> GetMock<T>(this object obj) where T : class
        {
            try
            {
                var f = (T)obj.GetType()
                   .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                   .Select(o => o.GetValue(obj))
                   .Single(o => o is T);
                return Mock.Get(f);
            }
            catch (Exception)
            {
                throw new Exception("mock not found or more than one !");
            }
        }

        public static void CreateFakeContext()
        {
            //fake context
            if (HttpContext.Current == null)
            {
                HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://stackoverflow/", ""),
                new HttpResponse(new StringWriter())
                );
            }
        }

        public static class AssertEx
        {
            public static T NoExceptionThrown<T>(Action action) where T : Exception
            {
                try
                {
                    action();
                    return null;
                }
                catch (T)
                {
                    return default(T);
                }
            }
        }

    }
}
