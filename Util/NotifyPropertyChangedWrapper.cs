using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WhatIfF1.Util
{
    public class NotifyPropertyChangedWrapper : INotifyPropertyChanged
    {
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnCollectionChanged(CollectionChangeAction action, object sender = null, object element = null)
        {
            sender = sender ?? this;
            CollectionChanged?.Invoke(sender, new CollectionChangeEventArgs(action, element));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<CollectionChangeEventArgs> CollectionChanged;
    }
}
