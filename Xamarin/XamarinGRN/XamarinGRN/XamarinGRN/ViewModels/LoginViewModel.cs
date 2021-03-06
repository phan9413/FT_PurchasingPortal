﻿using System;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;
using XamarinGRN.Helpers;
using XamarinGRN.Models;
using XamarinGRN.Views;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XamarinGRN.Services;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace XamarinGRN.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IPageService _pageService;
        public LoginViewModel(IPageService pageService)
        {
            _pageService = pageService;
            SubmitCommand = new Command(async () => 
            {
                await OnSubmit();
            });
        }

        private string _Url;
        public string Url
        {
            get { return _Url; }
            set
            {
                _Url = value;
                SetValue(ref _Url, value);
            }
        }

        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                SetValue(ref _UserName, value);
            }
        }
        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                SetValue(ref _Password, value);
            }
        }
        public ICommand SubmitCommand { protected set; get; }
        public async Task OnSubmit()
        {
            if (this.OnProcessLoading) return;
            this.ShowLoading("Signing...");
            try
            {
                if (UserName == FTSetting.SettingPageUser && Password == FTSetting.SettingPageUserPassword)
                {
                    this.HideLoading();
                    Application.Current.MainPage = new SettingPage();

                }
                else
                {
                    Application.Current.Properties[PropertyHelper.CompanyProp] = null;
                    Application.Current.Properties[PropertyHelper.WarehouseProp] = null;
                    Application.Current.Properties[PropertyHelper.BusinessPartnerProp] = null;
                    Application.Current.Properties[PropertyHelper.PurchaseOrderProp] = null;

                    if (string.IsNullOrEmpty(UserName))
                    {
                        await _pageService.DisplayAlert("Alert", "User Name is empty", "OK");
                        return;
                    }
                    string address = Url + "/login";
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
                            if (Settings.GeneralUrl != Url)
                                Settings.GeneralUrl = Url;

                            Settings.CurrentUser = UserName;
                            #region get user details
                            content = await client.RequestSvrAsync(Url + "/api/systemusers/" + UserName);

                            if (client.isSuccessStatusCode)
                            {
                                UserLogin usr = JsonConvert.DeserializeObject<UserLogin>(content);
                                if (usr.Company != null)
                                    Application.Current.Properties[PropertyHelper.CompanyProp] = usr.Company;
                                if (usr.Employee.WhsCode != null)
                                {
                                    Application.Current.Properties[PropertyHelper.WarehouseProp] = usr.Employee.WhsCode;
                                }
                            }
                            #endregion
                            this.HideLoading();
                            Application.Current.MainPage = new MainPage();
                            //Application.Current.MainPage = new NavigationPage(new MainPage());
                            //await _pageService.PushAsync(new GetPODetail());
                            return;
                        }
                        else
                        {
                            await _pageService.DisplayAlert("Alert", client.lastErrorDesc +
                                "\nLog in Failed", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Error", ex.Message, "OK");
            }
            this.HideLoading();

        }
    }
}