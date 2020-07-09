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

            submit = new Command(async () =>
            {
                await submitForm();
            });
            init();
        }
        private async Task init()
        {
            await getBP();
            await getWhs();
        }

        public async Task submitForm()
        {
            if (SelectedBP == null || SelectedWhs == null)
            {
                await _pageService.DisplayAlert("Fail 1", "Please Select BP and Warehouse", "OK");
                return;
            }
            else if (SelectedBP.BoKey == null || SelectedWhs.BoKey == null)
            {
                await _pageService.DisplayAlert("Fail 2", "Please Select BP and Warehouse", "OK");
                return;
            }
            Application.Current.Properties[PropertyHelper.BusinessPartnerProp] = SelectedBP;
            Application.Current.Properties[PropertyHelper.WarehouseProp] = SelectedWhs;

            await _pageService.PushAsync(new ChoosePOItem());
        }
        public ChooseBP page;

        private vwWarehouses _SelectedWhs;
        public vwWarehouses SelectedWhs
        {
            get { return _SelectedWhs; }
            set
            {
                SetValue(ref _SelectedWhs, value);
            }
        }

        private vwBusinessPartners _SelectedBP;
        public vwBusinessPartners SelectedBP
        {
            get { return _SelectedBP; }
            set
            {
                SetValue(ref _SelectedBP, value);
            }
        }

        private readonly IPageService _pageService;

        public ICommand submit { get; private set; }
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
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Error", ex.Message, "OK");
            }
            this.HideLoading();
        }

        public async Task getBP()
        {
            if (this.OnProcessLoading) return;
            this.ShowLoading("Loading...");
            try
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
            catch (Exception ex)
            {
                await _pageService.DisplayAlert("Error", ex.Message, "OK");
            }
            this.HideLoading();
        }

        public void filterList(string text)
        {
            text = text.ToUpper();
            bplist = new ObservableCollection<vwBusinessPartners>(ibplist.Where(pp => pp.CardCode.ToUpper().Contains(text) || (pp.CardName != null && pp.CardName.ToUpper().Contains(text))));
            page._bplist.ItemsSource = bplist;
        }

    }
}