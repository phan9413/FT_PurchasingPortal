using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinPR.Services
{
    public interface IPageService
    {
        Task PushAsync(Page page);
        Task PushModalAsync(Page page);
        Task PopModalAsync();
        Task<bool> DisplayAlert(string title, string message, string ok, string cancel = "");
    }
}
