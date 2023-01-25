using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace WhatIfF1.Util.Enumerables
{
    public class SortedObservableRangeCollection<T> : ObservableRangeCollection<T>
    {
        private readonly IComparer<T> _comparer;

        public SortedObservableRangeCollection(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public SortedObservableRangeCollection(IEnumerable<T> collection, IComparer<T> comparer) : base(collection)
        {
            _comparer = comparer;
        }

        public override void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Items.Add(item);
            }

            SortItems();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void SortItems()
        {
            var sorted = Items.ToList();
            sorted.Sort((elA, elB) => _comparer.Compare(elA, elB));
            Items.Clear();

            foreach (T item in sorted)
            {
                Items.Add(item);
            }
        }
    }
}
