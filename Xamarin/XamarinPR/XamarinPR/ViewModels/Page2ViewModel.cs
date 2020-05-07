using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace XamarinPR.ViewModels
{
    public class Page2ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Page2ViewModel(string note)
        {
            NoteText = note;

            DismissCommand = new Command(async () =>
            {
                await Application.Current.MainPage.Navigation.PopModalAsync();
                //await Application.Current.MainPage.Navigation.PopAsync();
            });
        }

        string _NoteText;
        public string NoteText
        {
            get => _NoteText;
            set
            {
                _NoteText = value;
                OnPropertyChanged("NoteText");
            }

        }

        public Command DismissCommand { get; }

        public void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
