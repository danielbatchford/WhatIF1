﻿using System;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Threading;
using WhatIfF1.Util;

namespace WhatIfF1.Logging
{
    public class Logger : PropertyChangedWrapper
    {
        #region LazyInitialization

        public static Logger Instance => _lazy.Value;

        private readonly static Lazy<Logger> _lazy = new Lazy<Logger>(() => new Logger());


        #endregion LazyInitialization

        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Color _defaultColor;
        private readonly Color _warningColor;
        private readonly Color _errorColor;
        private readonly Color _exceptionColor;

        private readonly int _timeoutInterval;

        private readonly DispatcherTimer _timeoutTimer;

        private string _currentMessage;

        public string CurrentMessage
        {
            get => _currentMessage;
            private set
            {
                _currentMessage = value;
                OnPropertyChanged();
            }
        }

        private Color _currentColor;
        public Color CurrentColor
        {
            get => _currentColor;
            private set
            {
                _currentColor = value;
                OnPropertyChanged();
            }
        }

        private Logger()
        {
            // Configure logger
            log4net.Config.XmlConfigurator.Configure();

            _defaultColor = Color.FromArgb(0, 0, 0, 0);
            _warningColor = Color.FromRgb(216, 87, 42);
            _errorColor = Color.FromRgb(196, 32, 33);
            _exceptionColor = Color.FromRgb(70, 11, 11);

            CurrentColor = _defaultColor;

            _timeoutInterval = 5;

            _timeoutTimer = new DispatcherTimer(new TimeSpan(0, 0, _timeoutInterval),
                                                DispatcherPriority.Normal,
                                                OnTimeoutTick,
                                                System.Windows.Application.Current.Dispatcher);
        }

        private void OnTimeoutTick(object sender, EventArgs e)
        {
            CurrentColor = _defaultColor;
            CurrentMessage = null;
        }

        public void Exception(Exception exception)
        {
            _logger.Error(exception);

            Console.WriteLine(exception.Message);

            CurrentMessage = exception.Message;
            CurrentColor = _exceptionColor;

            ResetTimerCountdown();
        }

        public void Info(string message)
        {
            _logger.Info(message);
            Console.WriteLine(message);

            CurrentMessage = message;
            CurrentColor = _defaultColor;

            ResetTimerCountdown();
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
            Console.WriteLine(message);

            CurrentMessage = message;
            CurrentColor = _warningColor;

            ResetTimerCountdown();
        }

        public void Error(string message)
        {
            _logger.Error(message);
            Console.WriteLine(message);

            CurrentMessage = message;
            CurrentColor = _errorColor;

            ResetTimerCountdown();
        }

        private void ResetTimerCountdown()
        {
            _timeoutTimer.Stop();
            _timeoutTimer.Start();
        }
    }
}
