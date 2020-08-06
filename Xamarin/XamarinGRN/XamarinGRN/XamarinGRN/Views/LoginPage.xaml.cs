using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinGRN.Helpers;
using XamarinGRN.Services;
using XamarinGRN.ViewModels;

namespace XamarinGRN.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            var vm = new LoginViewModel(new PageService());
            this.BindingContext = vm;
            InitializeComponent();

            UserName.Completed += (object sender, EventArgs e) =>
            {
                Password.Focus();
            };

            Password.Completed += (object sender, EventArgs e) =>
            {
                vm.SubmitCommand.Execute(null);
            };
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Url.Text = Settings.GeneralUrl;
            UserName.Text = Settings.CurrentUser;
        }
    }
}