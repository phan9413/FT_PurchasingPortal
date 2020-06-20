using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinPR.ViewModels;
using XamarinPR.Models;
using System.Collections.ObjectModel;

namespace XamarinPR.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetPODetail : ContentPage
    {
        public GetPODetail()
        {
            InitializeComponent();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var vm = BindingContext as GetPODetailViewModel;
            var item = e.Item as PurchaseOrderDetail;
            vm.hideOrShowItem(item);
        }

        private void btnAdd_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as GetPODetailViewModel;
            vm.addOrMinusItem(+1);

        }

        private void btnMinus_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as GetPODetailViewModel;
            vm.addOrMinusItem(-1);
        }

        private void Editor_Focused(object sender, FocusEventArgs e)
        {
        }

        private void Editor_Unfocused(object sender, FocusEventArgs e)
        {
        }

        private void CallMenuItem_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Alert", "Calling", "OK");
        }


        //private async void test()
        //{
        //    var vm = BindingContext as GetPODetailViewModel;
        //    await vm.getPOItem();


        //    ObservableCollection<PurchaseOrderDetail> a = null;
        //    a = vm.PurchaseOrderDetail;
        //    lstPO.ItemsSource = vm.PurchaseOrderDetail;


        //}
    }
}