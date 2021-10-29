

using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DataSoruce.Test
{
    public static class Util
    {
        public static DbSet<T> AddDbSet<T, T1>(this DbSet<T> result, params string[] row) where T : class
        {
            if (typeof(T1) == typeof(T) && result == null)
            {
                var queryableList = JsonConvert.DeserializeObject<IEnumerable<T>>($"[{string.Join(",", row)}]").AsQueryable();
                var mockSet = new Mock<DbSet<T>>();
                mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableList.Provider);
                mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableList.Expression);
                mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableList.ElementType);
                mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableList.GetEnumerator());
                return mockSet.Object;
            }
            return result;
        }

        public static DbSet<T> ReBuildDbSet<T>(this IQueryable<T> queryableList) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableList.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableList.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableList.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableList.GetEnumerator());
            return mockSet.Object;
        }
        public static DbSet<T> ReBuildDbSet<T>(this IEnumerable<T> ienumerableList) where T : class
        {
            return ienumerableList.AsQueryable().ReBuildDbSet();
        }
    }
}
