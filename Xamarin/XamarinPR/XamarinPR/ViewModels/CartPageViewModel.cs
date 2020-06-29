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
using System.Linq;
using System.CodeDom.Compiler;
using Newtonsoft.Json.Linq;

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
                if (igrnitem.Count() == 0)
                {
                    await _pageService.DisplayAlert("No Record", "No item found", "OK");
                    return;
                }
                string cardcardkey = igrnitem.FirstOrDefault().LineVendor.BoKey;

                foreach (PurchaseDeliveryDetail dtl in igrnitem)
                {
                    if (dtl.LineVendor.BoKey != cardcardkey)
                    {
                        await _pageService.DisplayAlert("Failed", "Different supplier found", "OK");
                        return;
                    }
                }
                ContentPage newpage = new GetDocDetails();
                var vm = newpage.BindingContext as GetDocDetailsViewModel;
                vm.sourcepage = page;
                await _pageService.PushModalAsync(newpage);
            });
        }
        public CartPage page;
        private bool _noitemfound;
        public bool noitemfound
        {
            get => _noitemfound;
            set
            {
                SetValue(ref _noitemfound, value);
            }
        }
        private string _docnumber;
        public string docnumber
        {
            get => _docnumber;
            set
            {
                SetValue(ref _docnumber, value);
            }
        }
        public ICommand submit { get; private set; }
        public async Task postGRN()
        {
            if (await _pageService.DisplayAlert("Alert", "Generate Delivery Order with DO Number[" + docnumber + "]", "OK", "Cancel"))
            {
                page.IsEnabled = false;
                string cardcardkey = igrnitem.FirstOrDefault().LineVendor.BoKey;

                PurchaseDelivery post = new PurchaseDelivery() { NumAtCard = docnumber };
                post.CardCode = new vwBusinessPartners() { BoKey = cardcardkey };

                foreach (PurchaseDeliveryDetail dtl in igrnitem)
                {
                    post.PurchaseDeliveryDetail.Add(new PurchaseDeliveryDetail() { Oid = dtl.Oid });
                }

                string json = JsonConvert.SerializeObject(post);
                JObject jobj = JObject.Parse(json);
                string address = Settings.GeneralUrl + "/api/gengrn";
                using (var client = new HttpClientWapi())
                {
                    var content = await client.RequestSvrAsync(address, jobj);
                    if (client.isSuccessStatusCode)
                    {
                        page._grnitemlist.BeginRefresh();
                        await getGRNItem();
                        page._grnitemlist.EndRefresh();
                        page.IsEnabled = true;

                        await _pageService.DisplayAlert("Success", "New GRN [" + content + "] created", "OK");
                        return;
                    }

                    await _pageService.DisplayAlert("Alert", "Failed", "OK");
                }

                page.IsEnabled = true;
            }
        }
        public async Task getGRNItem()
        {
            noitemfound = true;

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

                    if (grnitem.Count > 0)
                        noitemfound = false;
                    //page._bplist.SetBinding(ListView.SelectedItemProperty, "SelectedPO");
                    //page._bplist.ItemDisplayBinding = new Binding("WhsName");

                }
            }

            page._NoItemFound.IsVisible = noitemfound;
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