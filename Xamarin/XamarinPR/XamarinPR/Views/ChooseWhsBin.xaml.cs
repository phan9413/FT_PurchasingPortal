using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinPR.Helpers;
using XamarinPR.Models;
using XamarinPR.Services;
using XamarinPR.ViewModels;

namespace XamarinPR.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseWhsBin : ContentPage
    {
        public ListView _whslist;

        public ChooseWhsBin()
        {
            var vm = new ChooseWhsBinViewModel(new PageService());
            vm.page = this;
            BindingContext = vm;
            InitializeComponent();
            _whslist = this.whslist;
        }
        protected override void OnAppearing()
        {
            var vm = BindingContext as ChooseWhsBinViewModel;
            vm.refresh();
            base.OnAppearing();

            if (Application.Current.Properties[PropertyHelper.WarehouseProp] != null)
            {
                var whs = Application.Current.Properties[PropertyHelper.WarehouseProp] as vwWarehouses;
                this.whssearch.Text = whs.WhsCode;
            }

        }
        private void whssearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = BindingContext as ChooseWhsBinViewModel;
            if (e.OldTextValue != e.NewTextValue)
            {
                vm.filterList(e.NewTextValue);
            }

        }

    }
}