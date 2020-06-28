using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinPR.Services;
using XamarinPR.ViewModels;
using Plugin.Badge.Abstractions;

namespace XamarinPR.Views
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
        }

        protected override async void OnAppearing()
        {
            var vm = BindingContext as MainPageViewModel;
            base.OnAppearing();
            await setbadgetext();
            //CartTab.SetBinding(TabBadge.BadgeTextProperty, new Binding("Source=CartCount"));
        }
        public async Task setbadgetext()
        {
            var vm = BindingContext as MainPageViewModel;
            await vm.refreshCount();
            TabBadge.SetBadgeText(CartTab, vm.CartCount);

        }
    }
}