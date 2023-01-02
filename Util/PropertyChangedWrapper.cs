using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WhatIfF1.Util
{
    public class PropertyChangedWrapper : INotifyPropertyChanged
    {
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
