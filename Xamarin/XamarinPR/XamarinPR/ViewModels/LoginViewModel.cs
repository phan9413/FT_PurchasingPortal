using System;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;
using XamarinPR.Helper;
using XamarinPR.Models;
using XamarinPR.Views;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace XamarinPR.ViewModels
{
    public class LoginViewModel : IDisposable, INotifyPropertyChanged
    {
        public Action DisplayInvalidLoginPrompt;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string _Url;
        public string Url
        {
            get { return _Url; }
            set
            {
                _Url = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Url"));
            }
        }

        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                PropertyChanged(this, new PropertyChangedEventArgs("UserName"));
            }
        }
        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
            }
        }
        public ICommand SubmitCommand { protected set; get; }
        public LoginViewModel()
        {
            SubmitCommand = new Command(OnSubmit);
        }
        public async void OnSubmit()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                DisplayInvalidLoginPrompt();
                return;
            }
            string address = Url + "api/login";
            UserLogin request = new UserLogin()
            {
                UserName = UserName,
                Password = Password
            };

            using (var client = new HttpClientWapi())
            {

                var content = await client.RequestSvrAsync(address, request);
                if (client.isSuccessStatusCode)
                {
                    DisplayAlert("Alert", "Login Success.", "OK");

                    return;
                }

                DisplayAlert("Alert", client.lastErrorDesc +
                    "\nLog in Failed", "OK");
            }

        }
        /// <summary>
        /// to show display message dialog on phone screen
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="okBtn"></param>
        void DisplayAlert(string title, string message, string okBtn)
        {
            App.Current.MainPage.DisplayAlert(title, message, okBtn);
        }

        /// <summary>
        /// Handle the on property changed, value update to screen
        /// </summary>
        /// <param name="propertyname"></param>
        public void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            GC.Collect();
        }
    }
}