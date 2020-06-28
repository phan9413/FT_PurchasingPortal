using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinPR.Models;
using XamarinPR.Services;
using XamarinPR.ViewModels;

namespace XamarinPR.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChoosePOItem : ContentPage
    {
        public ListView _poitemlist;
        public ChoosePOItem()
        {
            var vm = new ChoosePOItemViewModel(new PageService());
            vm.page = this;
            BindingContext = vm;
            InitializeComponent();
            _poitemlist = this.poitemlist;

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
            var item = e.Item as PurchaseOrderDetail;
            vm.hideOrShowItem(item);
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
    }
}