using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinPR.Services;
using XamarinPR.ViewModels;

namespace XamarinPR.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseWhs : ContentPage
    {
        public ListView _whslist;

        public ChooseWhs()
        {
            var vm = new ChooseWhsViewModel(new PageService());
            vm.page = this;
            BindingContext = vm;
            InitializeComponent();
            _whslist = this.whslist;
        }
        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    var vm = BindingContext as ChooseWhsViewModel;

        //    if (vm.SelectedWhs != null)
        //    {
        //        _whslist.SelectedItem = vm.SelectedWhs;
        //    }
        //}
        private async void whslist_Refreshing(object sender, EventArgs e)
        {
            var vm = BindingContext as ChooseWhsViewModel;
            await vm.getWhs();
            _whslist.EndRefresh();
        }

        private void whssearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = BindingContext as ChooseWhsViewModel;
            if (e.OldTextValue != e.NewTextValue)
            {
                vm.filterList(e.NewTextValue);
            }

        }

    }
}