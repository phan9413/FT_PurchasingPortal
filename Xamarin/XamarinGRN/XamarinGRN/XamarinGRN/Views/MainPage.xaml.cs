﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinGRN.Services;
using XamarinGRN.ViewModels;
using Plugin.Badge.Abstractions;

namespace XamarinGRN.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage 
    {
        public CartPage _CartPage;
        public MainPage()
        {
            var vm = new MainPageViewModel(new PageService());
            vm.page = this;
            BindingContext = vm;
            InitializeComponent();
            _CartPage = this.CartTab;
            Xamarin.Forms.PlatformConfiguration.AndroidSpecific.TabbedPage.SetIsSwipePagingEnabled(this, false);

        }

        protected override async void OnAppearing()
        {
            var vm = BindingContext as MainPageViewModel;
            base.OnAppearing();
            await vm.refreshCount();
            setbadgetext(vm.CartCount);
            vm.HideLoading();

            //CartTab.SetBinding(TabBadge.BadgeTextProperty, new Binding("Source=CartCount"));
        }
        public void setbadgetext(string value)
        {
            TabBadge.SetBadgeText(CartTab, value);

        }
    }
}