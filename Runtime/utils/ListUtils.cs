using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NB.Charts.Utils
{
    internal static class ListUtils<T>
    {
        private static readonly FieldInfo ArrayField = typeof(List<T>).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.FieldType == typeof(T[]));

        /// <summary>
        /// Gets a slice of elements from a given list.
        /// NOTE: It is *crucial* that the list not be modified before whatever work requiring the slice is complete.
        /// </summary>
        public static ReadOnlySpan<T> Slice(IList<T> list, int start, int length)
        {
            return new ReadOnlySpan<T>((T[])ArrayField.GetValue(list), start, length);
        }
    }
}
