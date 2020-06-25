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

namespace XamarinPR.ViewModels
{
    public class CartPageViewModel : BaseViewModel
    {
        private readonly IPageService _pageService;
        public CartPageViewModel(IPageService pageService)
        {
            _pageService = pageService;
        }

    }
}