﻿using System;
using System.Windows.Input;

namespace WhatIfF1.Util
{
    public class CommandHandler : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _func;
        private readonly Func<bool> _canExecute;

        public CommandHandler(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public CommandHandler(Func<bool> func, Func<bool> canExecute)
        {
            _func = func;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            if (_func is null)
            {
                _action();
            }
            else
            {
                _func();
            }
        }
    }
}
