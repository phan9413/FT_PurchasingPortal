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
        public Label _NoItemFound;
        private ViewCell _lastCell;

        public CartPage()
        {
            var vm = new CartPageViewModel(new PageService());
            BindingContext = vm;
            vm.page = this;
            InitializeComponent();
            _grnitemlist = this.grnitemlist;
            _NoItemFound = this.NoItemFound;
        }
        private async Task refreshlist()
        {
            var vm = BindingContext as CartPageViewModel;
            await vm.getGRNItem();
            if (Application.Current.MainPage.GetType() == typeof(MainPage))
            {
                _parent = Application.Current.MainPage as MainPage;
                if (_parent != null)
                    _parent.setbadgetext(vm.grnitem.Count.ToString());
            }
            vm.SelectedGRNItem = null;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await refreshlist();
        }
        private async void grnitemlist_Refreshing(object sender, EventArgs e)
        {
            await refreshlist();
            _grnitemlist.EndRefresh();
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
                await refreshlist();
            }
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
    }

}

