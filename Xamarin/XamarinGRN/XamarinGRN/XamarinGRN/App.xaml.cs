using System;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinGRN.Views;

namespace XamarinGRN
{
    public partial class App : Application
    {
        public static HttpClient client; //<--- to be reused by App pages / views, maintain single connection

        public App()
        {
            InitializeComponent();
            client = new HttpClient(); //<---- maintain one client always in app connection

            MainPage = new LoginPage();
            //MainPage = new ChooseWhsBP();
            //DependencyService.Register<MockDataStore>();
            //MainPage = new MainPage();

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
