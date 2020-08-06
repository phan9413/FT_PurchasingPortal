using System;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;
using XamarinGRN.Helpers;
using XamarinGRN.Models;
using XamarinGRN.Views;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XamarinGRN.Services;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Security;

namespace XamarinGRN.ViewModels
{
    public class ChoosePOItemViewModel : BaseViewModel
    {
        private IEnumerable<vwWarehouses> iwarehouselist;
        private IEnumerable<vwWarehouseBins> ibinlist;
        private IEnumerable<vwWarehouseBins> iwhslist;

        private ObservableCollection<PurchaseDeliveryDetail> poitemlist;
        private IEnumerable<PurchaseDeliveryDetail> ipoitemlist;
        private PurchaseDeliveryDetail oldselecteditem { get; set; }
        public ChoosePOItemViewModel(IPageService pageService)
        {
            _pageService = pageService;
            //SelectedPOItem = new PurchaseOrderDetail();

            //submit = new Command(async () =>
            //{
            //    page.IsEnabled = false;
            //    await submitForm();
            //});
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
            await getWarehouse();
            await getWhs();
            await getBin();
            await getPOItem();
        }
        //public async Task submitForm()
        //{

        //    await _pageService.DisplayAlert("Success", "GRN items submitted", "OK");
        //    page.IsEnabled = true;
        //}

        public ChoosePOItem page;
        private bool _noitemfound;
        public bool noitemfound
        {
            get => _noitemfound;
            set
            {
                SetValue(ref _noitemfound, value);
            }
        }

        private PurchaseDeliveryDetail _SelectedPOItem;
        public PurchaseDeliveryDetail SelectedPOItem
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
        public async Task getWarehouse()
        {
            string Url = Settings.GeneralUrl;
            string company = ((Company)Application.Current.Properties[PropertyHelper.CompanyProp]).BoCode;
            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(Url + "/api/wh/whslist/" + company);

                if (client.isSuccessStatusCode)
                {
                    //whslist = new ObservableCollection<vwWarehouseBins>();
                    //whslist = JsonConvert.DeserializeObject<ObservableCollection<vwWarehouseBins>>(content);
                    iwarehouselist = JsonConvert.DeserializeObject<IEnumerable<vwWarehouses>>(content);

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

        public async Task getPOItem()
        {
            if (this.OnProcessLoading) return;
            this.ShowLoading("Loading...");
            try
            {
                noitemfound = true;

                string Url = Settings.GeneralUrl;
                string cardcodekey = "";
                string pono = "";
                if (Application.Current.Properties[PropertyHelper.BusinessPartnerProp] != null)
                    cardcodekey = ((vwBusinessPartners)Application.Current.Properties[PropertyHelper.BusinessPartnerProp]).BoKey;
                else if (Application.Current.Properties[PropertyHelper.PurchaseOrderProp] != null)
                    pono = ((PurchaseDelivery)Application.Current.Properties[PropertyHelper.PurchaseOrderProp]).DocNo;

                using (var client = new HttpClientWapi())
                {
                    string content = "";
                    if (!string.IsNullOrEmpty(cardcodekey))
                        content = await client.RequestSvrAsync(Url + "/api/getopengrnitem/" + cardcodekey);
                    else
                        content = await client.RequestSvrAsync(Url + "/api/getopengrnitembydoc/" + pono);

                    if (client.isSuccessStatusCode)
                    {
                        //bplist = new ObservableCollection<vwBusinessPartners>();
                        //bplist = JsonConvert.DeserializeObject<ObservableCollection<vwBusinessPartners>>(content);
                        ipoitemlist = JsonConvert.DeserializeObject<IEnumerable<PurchaseDeliveryDetail>>(content);
                        poitemlist = new ObservableCollection<PurchaseDeliveryDetail>(ipoitemlist);

                        page._poitemlist.ItemsSource = poitemlist;

                        if (poitemlist.Count > 0)
                            noitemfound = false;
                        //page._bplist.SetBinding(ListView.SelectedItemProperty, "SelectedPO");
                        //page._bplist.ItemDisplayBinding = new Binding("WhsName");

                    }

                }
                page._NoItemFound.IsVisible = noitemfound;
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Error", ex.Message, "OK");
            }
            this.HideLoading();
        }
        public async Task getWhs()
        {
            if (this.OnProcessLoading) return;
            this.ShowLoading("Loading...");
            try
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
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Error", ex.Message, "OK");
            }
            this.HideLoading();

        }
        public async Task getBin()
        {
            if (this.OnProcessLoading) return;
            this.ShowLoading("Loading...");
            try
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
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Error", ex.Message, "OK");
            }
            this.HideLoading();

        }

        public async Task postGRNItem(vwWarehouseBins wh = null)
        {
            if (this.OnProcessLoading) return;
            this.ShowLoading("Submitting...");
            try
            {
                if (wh != null)
                {
                    Application.Current.Properties[PropertyHelper.WarehouseProp] = iwarehouselist.Where(pp => pp.WhsCode == wh.WhsCode).FirstOrDefault(); ;
                }
                List<PurchaseReturnDetail> post = new List<PurchaseReturnDetail>();
                string whskey = "";
                string bokey = "";// ((vwBusinessPartners)Application.Current.Properties[PropertyHelper.BusinessPartnerProp]).BoKey;
                foreach (PurchaseDeliveryDetail dtl in ipoitemlist)
                {
                    if (dtl.isselected && dtl.OpenQty > 0)
                    {
                        PurchaseReturnDetail obj = new PurchaseReturnDetail();
                        obj.Baseline = dtl.Oid;
                        obj.BaseDocNo = dtl.docno;
                        obj.Quantity = dtl.OpenQty;
                        if (!string.IsNullOrEmpty(dtl.BatchNumber))
                            obj.BatchNumber = dtl.BatchNumber;
                        whskey = "";

                        if (wh != null)
                            whskey = wh.BoKey;
                        else
                            whskey = iwhslist.Where(pp => pp.WhsCode == dtl.WhsCode.WhsCode && pp.BinAbsEntry == 0).FirstOrDefault().BoKey;

                        obj.BinCode = new vwWarehouseBins() { BoKey = whskey };
                        obj.LineVendor = new vwBusinessPartners() { BoKey = dtl.PurchaseDelivery.CardCode.BoKey };

                        post.Add(obj);
                    }
                }
                if (post.Count > 0)
                {
                    string json = JsonConvert.SerializeObject(post);
                    JArray jobj = JArray.Parse(json);

                    string address = Settings.GeneralUrl + "/api/genpuritem";
                    using (var client = new HttpClientWapi())
                    {
                        var content = await client.RequestSvrAsync(address, jobj);
                        if (client.isSuccessStatusCode)
                        {
                            this.HideLoading();
                            page._poitemlist.BeginRefresh();
                            await getPOItem();
                            page._poitemlist.EndRefresh();
                            return;
                        }
                        else
                        {
                            await _pageService.DisplayAlert("Alert", " not found.", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Error", ex.Message, "OK");
            }
            this.HideLoading();
        }
        public void filterList(string text)
        {
            text = text.ToUpper();
            poitemlist = new ObservableCollection<PurchaseDeliveryDetail>(ipoitemlist.Where(pp => pp.title.ToUpper().Contains(text) || (pp.PurchaseDelivery != null && pp.PurchaseDelivery.DocNo.ToUpper().Contains(text))));
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
        public void hideOrShowItem(PurchaseDeliveryDetail item)
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