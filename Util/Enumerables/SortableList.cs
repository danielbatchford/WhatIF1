using System;
using System.Collections;
using System.Collections.Generic;

namespace WhatIfF1.Util.Enumerables
{
    public class SortableList<T> : IList<T> where T : IComparable<T>
    {
        private readonly List<T> _list;

        public int Count => _list.Count;
        bool ICollection<T>.IsReadOnly => false;

        public T this[int index] { get => _list[index]; set => _list[index] = value; }

        public SortableList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        public SortableList()
        {
            _list = new List<T>();
        }

        public void Add(T item)
        {
            if (Count == 0)
            {
                _list.Add(item);
                return;
            }

            if (_list[Count - 1].CompareTo(item) <= 0)
            {
                _list.Add(item);
                return;
            }

            if (_list[0].CompareTo(item) >= 0)
            {
                _list.Insert(0, item);
                return;
            }

            int index = _list.BinarySearch(item);

            if (index < 0)
            {
                index = ~index;
            }

            _list.Insert(index, item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new ArgumentException("Cannot use \"insert()\" on a sorted list");
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
    }
}
