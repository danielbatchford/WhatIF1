using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace WhatIfF1.Util.Enumerables
{
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        public ObservableRangeCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Items.Add(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
