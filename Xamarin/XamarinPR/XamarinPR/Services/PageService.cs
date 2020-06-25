using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinPR.Services
{
    public class PageService : IPageService
    {
        public async Task<bool> DisplayAlert(string title, string message, string ok, string cancel = "")
        {
            bool rtn = false;
            if (cancel == null || cancel.Length == 0)
                await Application.Current.MainPage.DisplayAlert(title, message, ok);
            else
                rtn = await Application.Current.MainPage.DisplayAlert(title, message, ok, cancel);

            return rtn;
        }

        public async Task PushAsync(Page page)
        {
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
    }
}
