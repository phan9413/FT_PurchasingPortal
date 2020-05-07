using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using XamarinPR.Views;

namespace XamarinPR.ViewModels
{
    public class Page1ViewModel : INotifyPropertyChanged
    {
        public Page1ViewModel()
        {
            AllNotes = new ObservableCollection<string>();
            EraseCommand = new Command(() => 
            {
                TheNote = string.Empty;
            });

            SaveCommand = new Command(() =>
            {
                AllNotes.Add(TheNote);
                TheNote = string.Empty;
            });

            SelectedNoteCommand = new Command(async () =>
            {
                var page2vm = new Page2ViewModel(SelectedNote);
                var page2 = new Page2();
                page2.BindingContext = page2vm;
                await Application.Current.MainPage.Navigation.PushModalAsync(page2);
                //await Application.Current.MainPage.Navigation.PushAsync(page2);

            });
        }

        public ObservableCollection<string> AllNotes { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        string _TheNote;
        public string TheNote
        {
            get => _TheNote;
            set 
            {
                _TheNote = value;
                OnPropertyChanged("TheNote");
                //var args = new PropertyChangedEventArgs(nameof(TheNote));
                //PropertyChanged?.Invoke(this, args);
            }

        }
        string _SelectedNote;
        public string SelectedNote
        {
            get => _SelectedNote;
            set
            {
                _SelectedNote = value;
                OnPropertyChanged("SelectedNote");
                //var args = new PropertyChangedEventArgs(nameof(TheNote));
                //PropertyChanged?.Invoke(this, args);
            }

        }

        public Command SaveCommand { get; }
        public Command EraseCommand { get; }
        public Command SelectedNoteCommand { get; }
        /// <summary>
        /// Handle the on property changed, value update to screen
        /// </summary>
        /// <param name="propertyname"></param>
        public void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
