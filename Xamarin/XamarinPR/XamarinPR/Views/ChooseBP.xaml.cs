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
    public partial class ChooseBP : ContentPage
    {
        public Picker _whspick;
        public ListView _bplist;
        public ChooseBP()
        {
            var vm = new ChooseBPViewModel(new PageService());
            vm.page = this;
            BindingContext = vm;
            InitializeComponent();
            _whspick = this.whspicker;
            _bplist = this.bplist;

            //vm.refresh.Invoke();
        }
        protected override async void OnAppearing()
        {
            var vm = BindingContext as ChooseBPViewModel;
            await vm.getWhs();
            await vm.getBP();

            base.OnAppearing();

        }

        private async void bplist_Refreshing(object sender, EventArgs e)
        {
            var vm = BindingContext as ChooseBPViewModel;
            await vm.getBP();
            _bplist.EndRefresh();
        }

        private void bpsearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vm = BindingContext as ChooseBPViewModel;
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