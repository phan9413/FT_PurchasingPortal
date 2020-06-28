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
    public class CartPageViewModel : BaseViewModel
    {
        private IEnumerable<PurchaseDeliveryDetail> igrnitem;
        public ObservableCollection<PurchaseDeliveryDetail> grnitem;
        public PurchaseDeliveryDetail SelectedGRNItem;

        private readonly IPageService _pageService;
        public CartPageViewModel(IPageService pageService)
        {
            _pageService = pageService;
            submit = new Command(async () =>
            {
                page.IsEnabled = false;
                await submitForm();
            });
        }
        public CartPage page;
        public ICommand submit { get; private set; }
        public async Task submitForm()
        {

            await _pageService.DisplayAlert("Pending", "Under Construction...", "OK");
            page.IsEnabled = true;
        }
        public async Task getGRNItem()
        {
            string Url = Settings.GeneralUrl;
            string username  = Settings.CurrentUser;
            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(Url + "/api/getgrnitem/" + username);

                if (client.isSuccessStatusCode)
                {
                    //bplist = new ObservableCollection<vwBusinessPartners>();
                    //bplist = JsonConvert.DeserializeObject<ObservableCollection<vwBusinessPartners>>(content);
                    igrnitem = JsonConvert.DeserializeObject<IEnumerable<PurchaseDeliveryDetail>>(content);
                    grnitem = new ObservableCollection<PurchaseDeliveryDetail>(igrnitem);

                    page._grnitemlist.ItemsSource = grnitem;
                    //page._bplist.SetBinding(ListView.SelectedItemProperty, "SelectedPO");
                    //page._bplist.ItemDisplayBinding = new Binding("WhsName");

                }

            }
        }
        public async Task<bool> removeGRNItem()
        {
            if (SelectedGRNItem is null)
            {
                await _pageService.DisplayAlert("Fail", "Select GRN Item", "OK");
                return false;
            }
            string Url = Settings.GeneralUrl;
            string oid = SelectedGRNItem.Oid.ToString();
            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsyncDelete(Url + "/api/PurchaseDeliveryDetail/" + oid);

                if (client.isSuccessStatusCode)
                {
                    return true;

                }
                await _pageService.DisplayAlert("Fail", "Not Found", "OK");
                return false;

            }
        }

    }
}