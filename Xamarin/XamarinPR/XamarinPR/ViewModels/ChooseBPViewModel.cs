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
    public class ChooseBPViewModel : BaseViewModel
    {
        private ObservableCollection<vwWarehouses> whslist;
        private ObservableCollection<vwBusinessPartners> bplist;
        private IEnumerable<vwWarehouses> iwhslist;
        private IEnumerable<vwBusinessPartners> ibplist;

        public ChooseBPViewModel(IPageService pageService)
        {
            _pageService = pageService;
            //SelectedWhs = new vwWarehouses();
            //SelectedBP = new vwBusinessPartners();

            submit = new Command(() =>
            {
                submitForm();
            });
            init();
        }
        private async void init()
        {
            await getBP();
        }

        public async void submitForm()
        {
            if (SelectedBP == null)
            {
                await _pageService.DisplayAlert("Fail 1", "Please Select BP", "OK");
                return;
            }
            else if (SelectedBP.BoKey == null)
            {
                await _pageService.DisplayAlert("Fail 2", "Please Select BP", "OK");
                return;
            }
            Application.Current.Properties[PropertyHelper.BusinessPartnerProp] = SelectedBP;

            await _pageService.PushAsync(new ChoosePOItem());
        }
        public ChooseBP page;

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

        private vwBusinessPartners _SelectedBP;
        public vwBusinessPartners SelectedBP
        {
            get { return _SelectedBP; }
            set
            {
                _SelectedBP = value;
                SetValue(ref _SelectedBP, value);
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

        public async Task getBP()
        {
            string Url = Settings.GeneralUrl;
            string company = ((Company)Application.Current.Properties[PropertyHelper.CompanyProp]).BoCode;
            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(Url + "/api/bp/supplierlist/" + company);

                if (client.isSuccessStatusCode)
                {
                    //bplist = new ObservableCollection<vwBusinessPartners>();
                    //bplist = JsonConvert.DeserializeObject<ObservableCollection<vwBusinessPartners>>(content);
                    ibplist = JsonConvert.DeserializeObject<IEnumerable<vwBusinessPartners>>(content);
                    bplist = new ObservableCollection<vwBusinessPartners>(ibplist);

                    page._bplist.ItemsSource = bplist;
                    //page._bplist.SetBinding(ListView.SelectedItemProperty, "SelectedBP");
                    //page._bplist.ItemDisplayBinding = new Binding("WhsName");

                }

            }
        }

        public void filterList(string text)
        {
            text = text.ToUpper();
            bplist = new ObservableCollection<vwBusinessPartners>(ibplist.Where(pp => pp.CardCode.ToUpper().Contains(text) || (pp.CardName != null && pp.CardName.ToUpper().Contains(text))));
            page._bplist.ItemsSource = bplist;
        }

    }
}