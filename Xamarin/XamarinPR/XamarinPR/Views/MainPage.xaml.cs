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
        public MainPage()
        {
            var vm = new MainPageViewModel(new PageService());
            BindingContext = vm;
            vm.getCount.Invoke();
            InitializeComponent();

            //CartTab.SetBinding(TabBadge.BadgeTextProperty, new Binding("Source=CartCount"));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}