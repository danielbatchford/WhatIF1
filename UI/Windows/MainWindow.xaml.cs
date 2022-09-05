﻿using Syncfusion.Windows.Shared;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WhatIfF1.Logging;
using WhatIfF1.Scenarios;

namespace WhatIfF1.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ChromelessWindow
    {
        public MainWindow()
        {
            // Retrieve app and set window title
            App app = (App)Application.Current;
            Title = $"{app.AppName} - v{app.Version}";

            // Retrieve window resolution
            bool widthValid = int.TryParse(ConfigurationManager.AppSettings["windowWidth"], out int width);
            bool heightValid = int.TryParse(ConfigurationManager.AppSettings["windowHeight"], out int height);

            if (!widthValid || !heightValid)
            {
                throw new WindowIntiializationException("Failed to update main window width and height values as their values were invalid");
            }

            // Set window dimensions
            Width = width;
            Height = height;

            InitializeComponent();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            // Set window data context
            DataContext = await ScenarioStore.Instance;

            // Set log bar data context
            LogBarGrid.DataContext = Logger.Instance;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
#if DEBUG
            if (e.Key != Key.D0)
            {
                return;
            }

            TestClass.RunTestCode();
            Logger.Instance.Info("Test ran");
#endif
        }
    }
}
