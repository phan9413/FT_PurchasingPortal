using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinPR.Helpers;
using XamarinPR.Services;
using XamarinPR.ViewModels;

namespace XamarinPR.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu : ContentPage
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void newgrn_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties[PropertyHelper.BusinessPartnerProp] = null;
            Application.Current.Properties[PropertyHelper.PurchaseOrderProp] = null;

            Application.Current.MainPage = new NavigationPage(new ChooseBP());
        }

        private async void newpogrn_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties[PropertyHelper.BusinessPartnerProp] = null;
            Application.Current.Properties[PropertyHelper.PurchaseOrderProp] = null;

            ContentPage newpage = new GetDocDetails();
            var vm = newpage.BindingContext as GetDocDetailsViewModel;
            vm.sourcepage = this;
            PageService serv = new PageService();
            await serv.PushModalAsync(newpage);

        }

        public void gotoPOItem()
        {
            Page newpage = new NavigationPage(new ChoosePOItem());
            Application.Current.MainPage = newpage;
            //PageService serv = new PageService();
            //await serv.PushAsync(new ChoosePOItem());
        }
    }
}