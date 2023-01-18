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
            if(indexes.Any(index => index >= list.Count || index < 0))
            {
                throw new ArgumentException("Index list contains indexes out of bounds");
            }

            if(indexes.Count == 0)
            {
                return new List<List<T>> { list };
            }

            IList<List<T>> splitList = new List<List<T>>(indexes.Count + 1);

            for (int i = 0; i < indexes.Count; i++)
            {
                int index = indexes[i];

                if(i == 0)
                {
                    splitList.Add(list.GetRange(0, index));
                }
                else if(i == indexes.Count - 1) 
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
    }
}
