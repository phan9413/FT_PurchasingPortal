using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinGRN.Models;
using XamarinGRN.Services;
using XamarinGRN.ViewModels;
using ZXing.Mobile;

namespace XamarinGRN.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChoosePOItem : ContentPage
    {
        public ListView _poitemlist;
        public Label _NoItemFound;
        private ViewCell _lastCell;
        public ChoosePOItem()
        {
            var vm = new ChoosePOItemViewModel(new PageService());
            vm.page = this;
            BindingContext = vm;
            InitializeComponent();
            _poitemlist = this.poitemlist;
            _NoItemFound = this.NoItemFound;
        }
        private async void poitemlist_Refreshing(object sender, EventArgs e)
        {
            var vm = BindingContext as ChoosePOItemViewModel;
            await vm.getPOItem();
            _poitemlist.EndRefresh();
        }
        private void poitemsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = BindingContext as ChoosePOItemViewModel;
            if (e.OldTextValue != e.NewTextValue)
            {
                vm.filterList(e.NewTextValue);
            }
        }
        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var vm = BindingContext as ChoosePOItemViewModel;
            var item = e.Item as PurchaseDeliveryDetail;
            vm.hideOrShowItem(item);
        }
        private void gotomenu_Clicked(object sender, EventArgs e)
        {
            MainPage page = new MainPage();
            Application.Current.MainPage = page;
        }
        private void gotocart_Clicked(object sender, EventArgs e)
        {
            MainPage page = new MainPage();
            page.CurrentPage = page._CartPage;
            Application.Current.MainPage = page;
        }

        private void InitMenu_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as ChoosePOItemViewModel;
            vm.eraseqty.Invoke();
        }

        private void switch_Toggled(object sender, ToggledEventArgs e)
        {
            var vm = BindingContext as ChoosePOItemViewModel;
            vm.SelectedPOItem.isselected = !vm.SelectedPOItem.isselected;
        }
        private async void gotobin_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as ChoosePOItemViewModel;
            await vm.gotoChooseBin();
        }
        private async void gotowh_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as ChoosePOItemViewModel;
            await vm.gotoChooseWhs();
        }
        private async void gotopo_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as ChoosePOItemViewModel;
            await vm.postGRNItem();
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            if (_lastCell != null)
                _lastCell.View.BackgroundColor = Color.Transparent;
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                viewCell.View.BackgroundColor = Color.LightSkyBlue;
                _lastCell = viewCell;
            }
        }

        private async void scanbarcode_Clicked(object sender, EventArgs e)
        {
            try
            {
                var scanner = DependencyService.Get<IQrScanningService>();
                var result = await scanner.ScanAsync();
                if (result != null)
                {
                    var vm = BindingContext as ChoosePOItemViewModel;
                    vm.SelectedPOItem.BatchNumber = result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}