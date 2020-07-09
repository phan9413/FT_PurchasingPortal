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
    public partial class GetDocDetails : ContentPage
    {
        private Entry _docno;
        public GetDocDetails()
        {
            var vm = new GetDocDetailsViewModel(new PageService());
            vm.page = this;
            BindingContext = vm;
            InitializeComponent();
            _docno = this.docno;
        }

        protected override void OnAppearing()
        {
            var vm = BindingContext as GetDocDetailsViewModel;
            if (typeof(CartPage) == vm.sourcepage.GetType())
            {
                _docno.Placeholder = "DO Number";
            }
            else if (typeof(MainMenu) == vm.sourcepage.GetType())
            {
                _docno.Placeholder = "PO Number";
            }
            base.OnAppearing();
        }
        private async void scanbarcode_Clicked(object sender, EventArgs e)
        {
            try
            {
                var scanner = DependencyService.Get<IQrScanningService>();
                var result = await scanner.ScanAsync();
                if (result != null)
                {
                    _docno.Text = result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}