using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinGRN.Helpers;

namespace XamarinGRN.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        Entry _urltxt;
        public SettingPage()
        {
            InitializeComponent();
            _urltxt = this.urltxt;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _urltxt.Text = Settings.GeneralUrl;
        }

        private void submitbtn_Clicked(object sender, EventArgs e)
        {
            Settings.GeneralUrl = _urltxt.Text;
            Application.Current.MainPage = new LoginPage();
        }
    }
}