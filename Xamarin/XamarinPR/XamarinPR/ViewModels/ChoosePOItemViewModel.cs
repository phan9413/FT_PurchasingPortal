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
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace XamarinPR.ViewModels
{
    public class ChoosePOItemViewModel : BaseViewModel
    {
        private ObservableCollection<PurchaseOrderDetail> poitemlist;
        private IEnumerable<PurchaseOrderDetail> ipoitemlist;
        private PurchaseOrderDetail oldselecteditem { get; set; }
        public ChoosePOItemViewModel(IPageService pageService)
        {
            _pageService = pageService;
            //SelectedPOItem = new PurchaseOrderDetail();

            submit = new Command(() =>
            {
                page.IsEnabled = false;
                submitForm();
            });
            eraseqty = new Action(() =>
            {
                if (SelectedPOItem == null)
                {
                    _pageService.DisplayAlert("Fail", "Please Tap an Item 1st", "OK");
                    return;
                }
                SelectedPOItem.OpenQty = 0;
            });
            init();
        }
        private async void init()
        {
            await getPOItem();
        }
        public async void submitForm()
        {

            await _pageService.DisplayAlert("Success", "GRN items submitted", "OK");
            page.IsEnabled = true;
        }

        public ChoosePOItem page;

        private PurchaseOrderDetail _SelectedPOItem;
        public PurchaseOrderDetail SelectedPOItem
        {
            get { return _SelectedPOItem; }
            set
            {
                _SelectedPOItem = value;
                SetValue(ref _SelectedPOItem, value);
            }
        }
        private vwWarehouses _SelectedWhs;
        public vwWarehouses SelectedWhs
        {
            get { return _SelectedWhs; }
            set
            {
                _SelectedWhs = value;
                SetValue(ref _SelectedWhs, value);
            }
        }

        private readonly IPageService _pageService;

        public ICommand submit { get; private set; }
        public Action eraseqty { get; private set; }

        public async Task getPOItem()
        {
            string Url = Settings.GeneralUrl;
            string cardcodekey = ((vwBusinessPartners)Application.Current.Properties[PropertyHelper.BusinessPartnerProp]).BoKey;
            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(Url + "/api/getopenpoitem/" + cardcodekey);

                if (client.isSuccessStatusCode)
                {
                    //bplist = new ObservableCollection<vwBusinessPartners>();
                    //bplist = JsonConvert.DeserializeObject<ObservableCollection<vwBusinessPartners>>(content);
                    ipoitemlist = JsonConvert.DeserializeObject<IEnumerable<PurchaseOrderDetail>>(content);
                    poitemlist = new ObservableCollection<PurchaseOrderDetail>(ipoitemlist);

                    page._poitemlist.ItemsSource = poitemlist;
                    //page._bplist.SetBinding(ListView.SelectedItemProperty, "SelectedPO");
                    //page._bplist.ItemDisplayBinding = new Binding("WhsName");

                }

            }
        }

        public void filterList(string text)
        {
            text = text.ToUpper();
            poitemlist = new ObservableCollection<PurchaseOrderDetail>(ipoitemlist.Where(pp => pp.title.ToUpper().Contains(text) || (pp.PurchaseOrder != null && pp.PurchaseOrder.DocNo.ToUpper().Contains(text))));
            page._poitemlist.ItemsSource = poitemlist;
        }
        public async void gotoChooseWhs()
        {
            var detailpage = new ChooseWhs();
            var vm = detailpage.BindingContext as ChooseWhsViewModel;
            vm.sourcepage = page;
            vm.SelectedWhs = SelectedPOItem.WhsCode;
            await _pageService.PushModalAsync(detailpage);
        }
        public void hideOrShowItem(PurchaseOrderDetail item)
        {
            int index = poitemlist.IndexOf(item);
            if (oldselecteditem == item)
            {
                //poitemlist[index].IsButtonVisible = !poitemlist[index].IsButtonVisible;
            }
            else
            {
                if (oldselecteditem != null)
                {
                    int oldindex = poitemlist.IndexOf(oldselecteditem);
                    if (oldindex > -1)
                        poitemlist[oldindex].IsButtonVisible = false;
                }
                poitemlist[index].IsButtonVisible = true;
            }
            oldselecteditem = item;
        }

    }
}