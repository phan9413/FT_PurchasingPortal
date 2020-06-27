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
    public class ChooseWhsBinViewModel : BaseViewModel
    {
        private ObservableCollection<vwWarehouseBins> whslist;
        public IEnumerable<vwWarehouseBins> iwhslist;

        public ChooseWhsBinViewModel(IPageService pageService)
        {
            _pageService = pageService;
            SelectedWhs = new vwWarehouseBins();
            back = new Command(async () =>
            {
                await _pageService.PopModalAsync();
            });

            submit = new Command(async () =>
            {
                await submitForm();
            });
        }
        public void refresh()
        {
            whslist = new ObservableCollection<vwWarehouseBins>(iwhslist);
            page._whslist.ItemsSource = whslist;
        }

        public async Task submitForm()
        {
            if (SelectedWhs == null)
            {
                await _pageService.DisplayAlert("Fail 1", "Please Select Bin", "OK");
                return;
            }
            else if (SelectedWhs.BoKey == null)
            {
                await _pageService.DisplayAlert("Fail 2", "Please Select Bin", "OK");
                return;
            }

            if (typeof(ChoosePOItem) == sourcepage.GetType())
            {
                var vm = ((ChoosePOItem)sourcepage).BindingContext as ChoosePOItemViewModel;
                // do genGRNItem();
                await vm.postGRNItem(SelectedWhs);

            }
            await _pageService.PopModalAsync();

        }

        public Page sourcepage;
        public ChooseWhsBin page;

        private vwWarehouseBins _SelectedWhs;
        public vwWarehouseBins SelectedWhs
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
        public ICommand back { get; private set; }

        public void filterList(string text)
        {
            text = text.ToUpper();
            whslist = new ObservableCollection<vwWarehouseBins>(iwhslist.Where(pp => pp.BinCode.ToUpper().Contains(text) || pp.WhsCode.ToUpper().Contains(text) || pp.WhsName.ToUpper().Contains(text)));
            page._whslist.ItemsSource = whslist;
        }

    }
}