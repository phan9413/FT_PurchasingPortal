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
    public partial class ChoosePO : ContentPage
    {
        public Picker _whspick;
        public ListView _polist;
        public ChoosePO()
        {
            var vm = new ChoosePOViewModel(new PageService());
            vm.page = this;
            BindingContext = vm;
            InitializeComponent();
            _whspick = this.whspicker;
            _polist = this.polist;
        }
        protected override async void OnAppearing()
        {
            var vm = BindingContext as ChoosePOViewModel;
            await vm.getWhs();
            await vm.getPO();

            base.OnAppearing();

        }
        private async void polist_Refreshing(object sender, EventArgs e)
        {
            var vm = BindingContext as ChoosePOViewModel;
            await vm.getPO();
            _polist.EndRefresh();
        }

        private void posearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = BindingContext as ChoosePOViewModel;
            if (e.OldTextValue != e.NewTextValue)
            {
                vm.filterList(e.NewTextValue);
            }
        }
        private void gotomenu_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new MainPage();
        }
    }
}