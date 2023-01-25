using System;

namespace WhatIfF1.Util.Events
{
    public class ItemChangedEventArgs<T> : EventArgs
    {
        public T OldValue { get; }
        public T NewValue { get; }

        public ItemChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
