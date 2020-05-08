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

namespace XamarinPR.ViewModels
{
    class GetPODetailViewModel : INotifyPropertyChanged
    {
        List<PurchaseOrderDetail> _list;

        public GetPODetailViewModel()
        {
            entryString = string.Empty;
            //PurchaseOrderDetail = new ObservableCollection<PurchaseOrderDetail>();
            submitGRN = new Command(() =>
            {
                postGRN();
            });

            searchPO = new Command(() =>
            {
                getPOItem();
            });
        }
        public async void postGRN()
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

                DisplayAlert("Alert", entryString + " not found.", "OK");
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
        public async void getPOItem()
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

                DisplayAlert("Alert", entryString + " not found.", "OK");
            }
        }
        public Command submitGRN { get; }
        public Command searchPO { get; }
        //public Command SelectedCommand { get; }

        private string _entryString;
        public string entryString
        {
            get => _entryString;
            set
            {
                _entryString = value;
                OnPropertyChanged("entryString");
            }
        }

        private PurchaseOrderDetail _SelectedItem;
        public PurchaseOrderDetail SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        private PurchaseOrderDetail _oldPurchaseOrderDetail { get; set; }
        public List<PurchaseOrderDetail> test { get; set; }
        public ObservableCollection<PurchaseOrderDetail> PurchaseOrderDetail { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// to show display message dialog on phone screen
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="okBtn"></param>
        void DisplayAlert(string title, string message, string okBtn)
        {
            App.Current.MainPage.DisplayAlert(title, message, okBtn);
        }
        /// <summary>
        /// Handle the on property changed, value update to screen
        /// </summary>
        /// <param name="propertyname"></param>
        public void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
