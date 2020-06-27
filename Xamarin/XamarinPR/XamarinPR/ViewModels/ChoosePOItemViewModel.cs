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
        private IEnumerable<vwWarehouseBins> ibinlist;
        private IEnumerable<vwWarehouseBins> iwhslist;

        private ObservableCollection<PurchaseOrderDetail> poitemlist;
        private IEnumerable<PurchaseOrderDetail> ipoitemlist;
        private PurchaseOrderDetail oldselecteditem { get; set; }
        public ChoosePOItemViewModel(IPageService pageService)
        {
            _pageService = pageService;
            //SelectedPOItem = new PurchaseOrderDetail();

            submit = new Command(async () =>
            {
                page.IsEnabled = false;
                await submitForm();
            });
            eraseqty = new Action(async() =>
            {
                if (SelectedPOItem == null)
                {
                    await _pageService.DisplayAlert("Fail", "Please Tap an Item 1st", "OK");
                    return;
                }
                SelectedPOItem.OpenQty = 0;
            });
            init();
        }
        public async Task init()
        {
            await getWhs();
            await getBin();
            await getPOItem();
        }
        public async Task submitForm()
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
        public async Task getWhs()
        {
            string Url = Settings.GeneralUrl;
            string company = ((Company)Application.Current.Properties[PropertyHelper.CompanyProp]).BoCode;
            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(Url + "/api/whbin/whslist/" + company);

                if (client.isSuccessStatusCode)
                {
                    //whslist = new ObservableCollection<vwWarehouseBins>();
                    //whslist = JsonConvert.DeserializeObject<ObservableCollection<vwWarehouseBins>>(content);
                    iwhslist = JsonConvert.DeserializeObject<IEnumerable<vwWarehouseBins>>(content);

                    //page._whslist.ItemsSource = whslist;
                    //page._whspick.SetBinding(Picker.SelectedItemProperty, "SelectedWhs");
                    //page._whspick.ItemDisplayBinding = new Binding("WhsName");

                    //if (Application.Current.Properties[PropertyHelper.WarehouseProp] != null)
                    //{
                    //    SelectedWhs = Application.Current.Properties[PropertyHelper.WarehouseProp] as vwWarehouseBins;
                    //    page._whslist.SelectedIndex = page._whslist.Items.IndexOf(SelectedWhs.WhsName);
                    //}
                }

            }
        }
        public async Task getBin()
        {
            string Url = Settings.GeneralUrl;
            string company = ((Company)Application.Current.Properties[PropertyHelper.CompanyProp]).BoCode;
            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(Url + "/api/whbin/binlist/" + company);

                if (client.isSuccessStatusCode)
                {
                    //whslist = new ObservableCollection<vwWarehouseBins>();
                    //whslist = JsonConvert.DeserializeObject<ObservableCollection<vwWarehouseBins>>(content);
                    ibinlist = JsonConvert.DeserializeObject<IEnumerable<vwWarehouseBins>>(content);

                    //page._whslist.ItemsSource = whslist;
                    //page._whspick.SetBinding(Picker.SelectedItemProperty, "SelectedWhs");
                    //page._whspick.ItemDisplayBinding = new Binding("WhsName");

                    //if (Application.Current.Properties[PropertyHelper.WarehouseProp] != null)
                    //{
                    //    SelectedWhs = Application.Current.Properties[PropertyHelper.WarehouseProp] as vwWarehouseBins;
                    //    page._whslist.SelectedIndex = page._whslist.Items.IndexOf(SelectedWhs.WhsName);
                    //}
                }

            }
        }

        public async Task postGRNItem(vwWarehouseBins wh = null)
        {
            try
            {
                List<PurchaseDeliveryDetail> post = new List<PurchaseDeliveryDetail>();
                foreach (PurchaseOrderDetail dtl in ipoitemlist)
                {
                    if (dtl.isselected && dtl.OpenQty > 0)
                    {
                        PurchaseDeliveryDetail obj = new PurchaseDeliveryDetail();
                        obj.Baseline = dtl.Oid;
                        obj.Quantity = dtl.OpenQty;
                        if (!string.IsNullOrEmpty(dtl.BatchNumber))
                            obj.BatchNumber = dtl.BatchNumber;
                        if (wh != null)
                            obj.BinCode = new vwWarehouseBins(){ BoKey = wh.BoKey };

                        post.Add(obj);
                    }
                }
                if (post.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(post);
                    JObject jobj = JObject.Parse(json);

                    string address = Settings.GeneralUrl + "api/submitgrnitem/" + Settings.CurrentUser;
                    using (var client = new HttpClientWapi())
                    {
                        var content = await client.RequestSvrAsync(address, jobj);
                        if (client.isSuccessStatusCode)
                        {
                            return;
                        }

                        await _pageService.DisplayAlert("Alert", " not found.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Alert", ex.Message, "OK");
            }
        }
        public void filterList(string text)
        {
            text = text.ToUpper();
            poitemlist = new ObservableCollection<PurchaseOrderDetail>(ipoitemlist.Where(pp => pp.title.ToUpper().Contains(text) || (pp.PurchaseOrder != null && pp.PurchaseOrder.DocNo.ToUpper().Contains(text))));
            page._poitemlist.ItemsSource = poitemlist;
        }
        public async Task gotoChooseWhs()
        {
            var detailpage = new ChooseWhsBin();
            var vm = detailpage.BindingContext as ChooseWhsBinViewModel;
            vm.sourcepage = page;
            vm.iwhslist = iwhslist;
            await _pageService.PushModalAsync(detailpage);
        }
        public async Task gotoChooseBin()
        {
            var detailpage = new ChooseWhsBin();
            var vm = detailpage.BindingContext as ChooseWhsBinViewModel;
            vm.sourcepage = page;
            vm.iwhslist = ibinlist;
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