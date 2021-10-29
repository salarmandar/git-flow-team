namespace Bgt.Ocean.Infrastructure.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using Util;

    /// <summary>
    /// a collection of extension methods for IEnumerable<T>
    /// </summary>
    public static class EnumerableHelper
    {

        /// <summary>
        /// Take(count) List and Add "..." instead
        /// Ex. 1,2,3,...
        /// </summary>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<string> To3DotAfterTake(this IEnumerable<string> list, int count)
        {
            return list.Count() > 3 ? (list.Take(3).Union(new string[] { "..." })) : list;
        }

        /// <summary>
        /// find the index of an item in the collection similar to List<T>.FindIndex()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="finder"></param>
        /// <returns></returns>
        public static int FindIndex<T>(this IEnumerable<T> list, Predicate<T> finder)
        {
            return list.ToList().FindIndex(finder);
        }

        public static T GetElementByIndex<T>(int index, IEnumerable<T> array)
        {
            return array.ElementAt(index);
        }

        public static T ConvertToEnumJobScreen<T>(this Guid item)
        {
            var type = typeof(T);
            var description = item.ToString().ToLower();
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description.ToLower() == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name.ToLower() == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", nameof(item));
        }

        public static Guid GetJobScreenGuid(this JobScreen value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return Guid.Parse(attributes.First().Description);
            }
            return Guid.Parse(value.ToString());
        }
    }


}
