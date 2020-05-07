using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using XamarinPR.Models;

namespace XamarinPR.ViewModels
{
    public class csPage2ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public csPage2ViewModel()
        {
            DismissCommand = new Command(async () =>
            {
                //await Application.Current.MainPage.Navigation.PopModalAsync();
                await Application.Current.MainPage.Navigation.PopAsync();
            });
        }

        string _Note;
        public string Note
        {
            get => _Note;
            set
            {
                _Note = value;
                OnPropertyChanged("Note");
            }
            //var args = new PropertyChangedEventArgs(nameof(TheNote));
            //PropertyChanged?.Invoke(this, args);

        }

        public Command DismissCommand { get; }

        public void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
