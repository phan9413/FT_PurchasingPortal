using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using XamarinPR.Models;
using XamarinPR.Helpers;
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using XamarinPR.Services;

namespace XamarinPR.ViewModels
{
    class GetPODetailViewModel : BaseViewModel
    {
        List<PurchaseOrderDetail> _list;
        private readonly IPageService _pageService;
        public GetPODetailViewModel(IPageService pageService)
        {
            _pageService = pageService;
            entryString = string.Empty;
            //PurchaseOrderDetail = new ObservableCollection<PurchaseOrderDetail>();
            submitGRN = new Command(async () =>
            {
                await postGRN();
            });

            searchPO = new Command(async () => 
            {
                await getPOItem();
            });
        }
        private async Task postGRN()
        {
            string address = Settings.GeneralUrl + "api/submitgrnfrompo/" + entryString;

            PurchaseDelivery obj = new PurchaseDelivery();
            foreach (PurchaseOrderDetail dtl in PurchaseOrderDetail)
            {
                if (dtl.OpenQty > 0)
                {
                    obj.PurchaseDeliveryDetail.Add(new PurchaseDeliveryDetail
                    {
                        Baseline = dtl.Oid,
                        Quantity = dtl.OpenQty
                    });
                }
            }
            string json = JsonConvert.SerializeObject(obj);
            JObject jobj = JObject.Parse(json);

            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(address, jobj);
                if (client.isSuccessStatusCode)
                {
                    return;
                }

                await _pageService.DisplayAlert("Alert", entryString + " not found.", "OK");
            }
        }
        private async Task getPOItem()
        {
            string address = Settings.GeneralUrl + "api/getpoitem/" + entryString;

            using (var client = new HttpClientWapi())
            {
                var content = await client.RequestSvrAsync(address);
                if (client.isSuccessStatusCode)
                {
                    _list = new List<PurchaseOrderDetail>();
                    //DisplayAlert("Alert", "PO found.", "OK");
                    JObject value = JObject.Parse(content);
                    string detalclassname = "PurchaseOrderDetail";
                    JArray jarray = (JArray)value[detalclassname];
                    //foreach (JObject Jdtl in jarray.Children())
                    //{
                    //    PurchaseOrderDetail a = JsonConvert.DeserializeObject<PurchaseOrderDetail>(Jdtl.ToString());
                    //    _list.Add(a);
                    //}
                    _list = JsonConvert.DeserializeObject<List<PurchaseOrderDetail>>(jarray.ToString());

                    PurchaseOrderDetail = new ObservableCollection<PurchaseOrderDetail>(_list);
                    OnPropertyChanged(nameof(PurchaseOrderDetail));
                    //PurchaseOrderDetail = JsonConvert.DeserializeObject<ObservableCollection<PurchaseOrderDetail>>(jarray.ToString());
                    return;
                }

                await _pageService.DisplayAlert("Alert", entryString + " not found.", "OK");
            }
        }

        public void addOrMinusItem(double value)
        {
            int index = PurchaseOrderDetail.IndexOf(_oldPurchaseOrderDetail);
            double openqty = PurchaseOrderDetail[index].OpenQty + value;
            if (openqty >= 0)
                PurchaseOrderDetail[index].OpenQty = openqty;
        }
        public void hideOrShowItem(PurchaseOrderDetail item)
        {
            int index = PurchaseOrderDetail.IndexOf(item);
            if (_oldPurchaseOrderDetail == item)
            {
                PurchaseOrderDetail[index].IsButtonVisible = !PurchaseOrderDetail[index].IsButtonVisible;
            }
            else
            {
                if (_oldPurchaseOrderDetail != null)
                {
                    int oldindex = PurchaseOrderDetail.IndexOf(_oldPurchaseOrderDetail);
                    if (oldindex > -1)
                        PurchaseOrderDetail[oldindex].IsButtonVisible = false;
                }
                PurchaseOrderDetail[index].IsButtonVisible = true;
            }
            _oldPurchaseOrderDetail = item;
        }
        public ICommand submitGRN { get; private set; }
        public ICommand searchPO { get; private set; }
        //public Command SelectedCommand { get; }

        private string _entryString;
        public string entryString
        {
            get => _entryString;
            set
            {
                //_entryString = value;
                //OnPropertyChanged();
                SetValue(ref _entryString, value);
            }
        }

        private PurchaseOrderDetail _SelectedItem;
        public PurchaseOrderDetail SelectedItem
        {
            get => _SelectedItem;
            set
            {
                //_SelectedItem = value;
                //OnPropertyChanged();
                SetValue(ref _SelectedItem, value);
            }
        }
        private PurchaseOrderDetail _oldPurchaseOrderDetail { get; set; }
        public List<PurchaseOrderDetail> test { get; set; }
        public ObservableCollection<PurchaseOrderDetail> PurchaseOrderDetail { get; set; }

    }
}
