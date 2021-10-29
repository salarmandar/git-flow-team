﻿using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Bgt.Ocean.Infrastructure.Helpers
{
    public static class ConcurrentBag
    {
        public static void AddRange<T>(this ConcurrentBag<T> @this, IEnumerable<T> toAdd)
        {
            foreach (var element in toAdd)
            {
                @this.Add(element);
            }
        }
    }
}
