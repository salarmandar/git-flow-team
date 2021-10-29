using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bgt.Ocean.Infrastructure.CompareHelper
{
    public static class CompareHelper
    {
        public static T Clone<T>(this T source)
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(source);
                return JsonConvert.DeserializeObject<T>(serialized);
            }
            catch
            {
                return source;
            }
        }
        private class LambdaComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _lambdaComparer;
            private readonly Func<T, int> _lambdaHash;

            public LambdaComparer(Func<T, T, bool> lambdaComparer) :
                this(lambdaComparer, o => 0)
            {
            }

            public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
            {
                if (lambdaComparer == null)
                    throw new ArgumentNullException("lambdaComparer");
                if (lambdaHash == null)
                    throw new ArgumentNullException("lambdaHash");

                _lambdaComparer = lambdaComparer;
                _lambdaHash = lambdaHash;
            }

            public bool Equals(T x, T y)
            {
                return _lambdaComparer(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _lambdaHash(obj);
            }
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> enumerable, Func<T, T, bool> comparer)
        {
            return enumerable.Distinct(new LambdaComparer<T>(comparer));
        }

    }
}