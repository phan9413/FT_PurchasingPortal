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
        public GetDocDetails()
        {
            var vm = new GetDocDetailsViewModel(new PageService());
            vm.page = this;
            BindingContext = vm;
            InitializeComponent();
        }
    }
}