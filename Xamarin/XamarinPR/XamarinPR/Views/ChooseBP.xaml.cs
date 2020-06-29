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
    public partial class ChooseBP : ContentPage
    {
        public Picker _whspick;
        public ListView _bplist;
        private ViewCell _lastCell;
        public ChooseBP()
        {
            var vm = new ChooseBPViewModel(new PageService());
            vm.page = this;
            BindingContext = vm;
            InitializeComponent();
            _whspick = this.whspicker;
            _bplist = this.bplist;

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