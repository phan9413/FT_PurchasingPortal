using System;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;
using XamarinPR.Helpers;
using XamarinPR.Models;
using XamarinPR.Views;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XamarinPR.Services;
using System.Threading.Tasks;

namespace XamarinPR.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly IPageService _pageService;
        public MainPageViewModel(IPageService pageService)
        {
            _pageService = pageService;
            CartCount = "0";

            getCount = new Action(() =>
            {
                refreshCount();
            });

        }
        private async void refreshCount()
        {
            string Url = Settings.GeneralUrl;
            string UserName = Settings.CurrentUser;
            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(Url + "/api/GetGRNItemCount/" + UserName);

                if (client.isSuccessStatusCode)
                {
                    CartCount = content;
                }
            }
        }
        public Action getCount { get; private set; }

        private string _CartCount;
        public string CartCount
        {
            get { return _CartCount; }
            set
            {
                _CartCount = value;
                SetValue(ref _CartCount, value);
            }
        }

    }
}