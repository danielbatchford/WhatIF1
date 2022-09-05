using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhatIfF1.UI.Windows;

namespace WhatIfF1
{
    public static class TestClass
    {
        public static async Task<bool> RunTestCode()
        {
            Console.WriteLine((App.Current.MainWindow as MainWindow).ScenariosListView.DataContext);

            return true;
        }
    }
}
