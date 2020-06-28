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
    public partial class CartPage : ContentPage
    {
        public ListView _grnitemlist;
        public MainPage _parent;
        public CartPage()
        {
            var vm = new CartPageViewModel(new PageService());
            BindingContext = vm;
            vm.page = this;
            InitializeComponent();
            _grnitemlist = this.grnitemlist;
        }
        protected override async void OnAppearing()
        {
            var vm = BindingContext as CartPageViewModel;
            base.OnAppearing();
            await vm.getGRNItem();
            vm.SelectedGRNItem = null;
        }
        private async void grnitemlist_Refreshing(object sender, EventArgs e)
        {
            var vm = BindingContext as CartPageViewModel;
            await vm.getGRNItem();
            _grnitemlist.EndRefresh();
            vm.SelectedGRNItem = null;
        }
        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var vm = BindingContext as CartPageViewModel;
            vm.SelectedGRNItem = e.Item as PurchaseDeliveryDetail;
        }
        private async void Remove_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as CartPageViewModel;
            bool rtn = await vm.removeGRNItem();
            if (rtn)
            {
                await vm.getGRNItem();
                if (_parent != null)
                    await _parent.setbadgetext();
            }
        }
    }

}

