﻿namespace Aero2Reload.Config.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    PropertyChangedEventHandler handler = this.PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs(propertyName));
                    }
                }));
        }
    }
}