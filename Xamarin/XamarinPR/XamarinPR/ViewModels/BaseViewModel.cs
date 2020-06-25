using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace XamarinPR.ViewModels
{
    public class BaseViewModel : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            GC.Collect();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyname = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
        protected void SetValue<T>(ref T backField, T value, [CallerMemberName] string propertyname = null)
        {
            if (EqualityComparer<T>.Default.Equals(backField, value))
                return;
            backField = value;
            OnPropertyChanged(nameof(propertyname));
        }
        //public void DisplayAlert(string title, string message, string okBtn, string cancelBtn = "")
        //{
        //    if (cancelBtn.Length > 0)
        //        App.Current.MainPage.DisplayAlert(title, message, okBtn, cancelBtn);
        //    else
        //        App.Current.MainPage.DisplayAlert(title, message, okBtn);

        //}
    }
}
