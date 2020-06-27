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
    public class ChoosePOViewModel : BaseViewModel
    {
        private ObservableCollection<vwWarehouses> whslist;
        private ObservableCollection<PurchaseOrder> polist;
        private IEnumerable<vwWarehouses> iwhslist;
        private IEnumerable<PurchaseOrder> ipolist;

        public ChoosePOViewModel(IPageService pageService)
        {
            _pageService = pageService;
            //SelectedWhs = new vwWarehouses();
            //SelectedPO = new PurchaseOrder();

            submit = new Command(async () =>
            {
                await submitForm();
            });
            init();
        }
        private async Task init()
        {
            await getWhs();
            await getPO();
        }
        public async Task submitForm()
        {
            if (SelectedPO == null || SelectedWhs == null)
            {
                await _pageService.DisplayAlert("Fail 1", "Please Select Warehouse and PO", "OK");
                return;
            }
            else if (SelectedPO.DocNo == null || SelectedWhs.BoKey == null)
            {
                await _pageService.DisplayAlert("Fail 2", "Please Select Warehouse and PO", "OK");
                return;
            }
            Application.Current.Properties[PropertyHelper.PurchaseOrderProp] = SelectedPO;
            Application.Current.Properties[PropertyHelper.WarehouseProp] = SelectedWhs;

            await _pageService.DisplayAlert("Success", "Warehouse and PO selected", "OK");
        }

        public ChoosePO page;

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

        private PurchaseOrder _SelectedPO;
        public PurchaseOrder SelectedPO
        {
            get { return _SelectedPO; }
            set
            {
                _SelectedPO = value;
                SetValue(ref _SelectedPO, value);
            }
        }

        private readonly IPageService _pageService;

        public ICommand submit { get; private set; }
        public async Task getWhs()
        {
            string Url = Settings.GeneralUrl;
            string company = ((Company)Application.Current.Properties[PropertyHelper.CompanyProp]).BoCode;
            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(Url + "/api/wh/whslist/" + company);

                if (client.isSuccessStatusCode)
                {
                    //whslist = new ObservableCollection<vwWarehouses>();
                    //whslist = JsonConvert.DeserializeObject<ObservableCollection<vwWarehouses>>(content);
                    iwhslist = JsonConvert.DeserializeObject<IEnumerable<vwWarehouses>>(content);
                    whslist = new ObservableCollection<vwWarehouses>(iwhslist);

                    page._whspick.ItemsSource = whslist;
                    //page._whspick.SetBinding(Picker.SelectedItemProperty, "SelectedWhs");
                    //page._whspick.ItemDisplayBinding = new Binding("WhsName");

                    if (Application.Current.Properties[PropertyHelper.WarehouseProp] != null)
                    {
                        SelectedWhs = Application.Current.Properties[PropertyHelper.WarehouseProp] as vwWarehouses;
                        page._whspick.SelectedIndex = page._whspick.Items.IndexOf(SelectedWhs.WhsName);
                    }
                }
                
            }
        }

        public async Task getPO()
        {
            string Url = Settings.GeneralUrl;
            string cardcodekey = ((vwBusinessPartners)Application.Current.Properties[PropertyHelper.BusinessPartnerProp]).BoKey;
            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(Url + "/api/getopenpo/" + cardcodekey);

                if (client.isSuccessStatusCode)
                {
                    //bplist = new ObservableCollection<vwBusinessPartners>();
                    //bplist = JsonConvert.DeserializeObject<ObservableCollection<vwBusinessPartners>>(content);
                    ipolist = JsonConvert.DeserializeObject<IEnumerable<PurchaseOrder>>(content);
                    polist = new ObservableCollection<PurchaseOrder>(ipolist);

                    page._polist.ItemsSource = polist;
                    //page._bplist.SetBinding(ListView.SelectedItemProperty, "SelectedPO");
                    //page._bplist.ItemDisplayBinding = new Binding("WhsName");

                }

            }
        }

        public void filterList(string text)
        {
            text = text.ToUpper();
            polist = new ObservableCollection<PurchaseOrder>(ipolist.Where(pp => pp.DocNo.ToUpper().Contains(text)));
            page._polist.ItemsSource = polist;
        }

    }
}