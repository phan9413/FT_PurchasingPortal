using System;
using System.ComponentModel;
using System.Windows.Input;

using Xamarin.Forms;
using XamarinPR.Helpers;
using XamarinPR.Models;
using XamarinPR.Views;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XamarinPR.Services;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace XamarinPR.ViewModels
{
    public class GetDocDetailsViewModel : BaseViewModel
    {

        public GetDocDetailsViewModel(IPageService pageService)
        {
            _pageService = pageService;
            back = new Command(async () =>
            {
                await _pageService.PopModalAsync();
            });

            submit = new Command(async () =>
            {
                await submitForm();
            });
        }

        public async Task submitForm()
        {
            if (string.IsNullOrEmpty(docnumber))
            {
                await _pageService.DisplayAlert("Fail", "Please fill in DO Number", "OK");
                return;
            }

            if (typeof(CartPage) == sourcepage.GetType())
            {
                var vm = ((CartPage)sourcepage).BindingContext as CartPageViewModel;
                vm.docnumber = docnumber;
                await vm.postGRN();

            }
            await _pageService.PopModalAsync();

        }

        public ContentPage sourcepage;
        public GetDocDetails page;

        private string _docnumber;
        public string docnumber
        {
            get => _docnumber;
            set
            {
                SetValue(ref _docnumber, value);
            }
        }
        private readonly IPageService _pageService;

        public ICommand submit { get; private set; }
        public ICommand back { get; private set; }


    }
}