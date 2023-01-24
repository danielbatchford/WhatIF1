using System;
using System.Collections.Generic;
using System.Linq;

namespace WhatIfF1.Util.Extensions
{
    public static class EnumerableExtensions
    {
        public static IList<List<T>> SplitByIndexList<T>(this IEnumerable<T> enumerable, IList<int> indexes)
        {
            List<T> list = enumerable.ToList();

            // TODO - can make much more efficient
            if (indexes.Any(index => index >= list.Count || index < 0))
            {
                throw new ArgumentException("Index list contains indexes out of bounds");
            }

            if (indexes.Count == 0)
            {
                return new List<List<T>> { list };
            }

            IList<List<T>> splitList = new List<List<T>>(indexes.Count + 1);

            for (int i = 0; i < indexes.Count; i++)
            {
                int index = indexes[i];

                if (i == 0)
                {
                    splitList.Add(list.GetRange(0, index));
                }
                else if (i == indexes.Count - 1)
                {
                    list.GetRange(index, indexes.Count - index);
                }
                else
                {
                    splitList.Add(list.GetRange(index, indexes[i + 1] - index));
                }
            }

            return splitList;
        }

        /// <summary>
        /// Efficient way to find the closest index in a list from a provided value.
        /// </summary>
        /// <param name="sortedList"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int FindClosestIndex(this int[] sortedList, int value)
        {
            int n = sortedList.Length;

            int i = 0;
            int j = n;
            int mid = 0;

            // value outside the bounds of the array
            if (value <= sortedList[0])
            {
                return 0;
            }
            if (value >= sortedList[n - 1])
            {
                return n - 1;
            }

            while (i < j)
            {
                mid = (i + j) / 2;

                if (sortedList[mid] == value)
                {
                    return mid;
                }

                if (value < sortedList[mid])
                {
                    if (mid > 0 && value > sortedList[mid - 1])
                    {
                        return value - sortedList[mid - 1] >= value - sortedList[mid] ? mid : mid - 1;
                    }

                    j = mid;
                }
                else
                {
                    if (mid < n - 1 && value < sortedList[mid + 1])
                    {
                        return value - sortedList[mid] >= sortedList[mid + 1] - value ? mid + 1 : mid;
                    }

                    i = mid + 1;
                }
            }

            return mid;
        }
    }
}
