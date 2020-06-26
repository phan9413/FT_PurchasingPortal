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
    public class ChooseWhsViewModel : BaseViewModel
    {
        private ObservableCollection<vwWarehouses> whslist;
        private IEnumerable<vwWarehouses> iwhslist;

        public ChooseWhsViewModel(IPageService pageService)
        {
            _pageService = pageService;
            SelectedWhs = new vwWarehouses();

            submit = new Command(() =>
            {
                submitForm();
            });
            init();
        }
        private async void init()
        {
            await getWhs();

        }

        public async void submitForm()
        {
            if (SelectedWhs == null)
            {
                await _pageService.DisplayAlert("Fail 1", "Please Select Warehouse", "OK");
                return;
            }
            else if (SelectedWhs.BoKey == null)
            {
                await _pageService.DisplayAlert("Fail 2", "Please Select Warehouse", "OK");
                return;
            }

            if (typeof(ChoosePOItem) == sourcepage.GetType())
            {
                var vm = ((ChoosePOItem)sourcepage).BindingContext as ChoosePOItemViewModel;
                vm.SelectedPOItem.WhsCode = SelectedWhs;
            }
            await _pageService.PopModalAsync();

        }

        public Page sourcepage;
        public ChooseWhs page;

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

                    page._whslist.ItemsSource = whslist;
                    //page._whspick.SetBinding(Picker.SelectedItemProperty, "SelectedWhs");
                    //page._whspick.ItemDisplayBinding = new Binding("WhsName");

                    //if (Application.Current.Properties[PropertyHelper.WarehouseProp] != null)
                    //{
                    //    SelectedWhs = Application.Current.Properties[PropertyHelper.WarehouseProp] as vwWarehouses;
                    //    page._whslist.SelectedIndex = page._whslist.Items.IndexOf(SelectedWhs.WhsName);
                    //}
                }

            }
        }

        public void filterList(string text)
        {
            text = text.ToUpper();
            whslist = new ObservableCollection<vwWarehouses>(iwhslist.Where(pp => pp.WhsCode.ToUpper().Contains(text) || pp.WhsName.ToUpper().Contains(text)));
            page._whslist.ItemsSource = whslist;
        }

    }
}