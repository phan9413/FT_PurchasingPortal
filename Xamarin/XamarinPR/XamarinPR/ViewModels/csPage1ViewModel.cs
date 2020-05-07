using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using XamarinPR.Views;
using XamarinPR.Models;

namespace XamarinPR.ViewModels
{
    class csPage1ViewModel : INotifyPropertyChanged
    {
        public csPage1ViewModel()
        {
            AllNotes = new ObservableCollection<csPage1Model>();
            EraseCommand = new Command(() => 
            {
                TheNote = string.Empty;
                AllNotes.Clear();
            });

            SaveCommand = new Command(() =>
            {
                AllNotes.Add(new csPage1Model { text = TheNote });
                TheNote = string.Empty;
            });

            SelectedNoteCommand = new Command(async () =>
            {
                if (SelectedNote is null) return;

                var page2vm = new csPage2ViewModel
                {
                    Note = _SelectedNote.text
                };

                var page2 = new csPage2(page2vm);
                //await Application.Current.MainPage.Navigation.PushModalAsync(page2);
                await Application.Current.MainPage.Navigation.PushAsync(page2);

            });
        }

        public ObservableCollection<csPage1Model> AllNotes { get; set; }
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
        csPage1Model _SelectedNote;
        public csPage1Model SelectedNote
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
