using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XamarinPR.Models
{
    public class PurchaseOrder
    {
        public PurchaseOrder()
        {
            PurchaseOrderDetail = new List<PurchaseOrderDetail>();
        }
        public List<PurchaseOrderDetail> PurchaseOrderDetail;
    }

    public class PurchaseOrderDetail : INotifyPropertyChanged
    {
        public PurchaseOrderDetail()
        {
            //IsButtonVisible = false;
        }

        string _title;
        [JsonIgnore]
        public string title 
        { 
            get => _title; 
            set
            {
                _title = value;
                OnPropertyChanged(nameof(title));
            }
        }
        public int Oid { get; set; }
        public string Dscription { get; set; }
        public int Baseline { get; set; }
        public double Quantity { get; set; }
        public double CopyQty { get; set; }

        double _OpenQty;
        public double OpenQty
        {
            get => _OpenQty;
            set
            {
                _OpenQty = value;
                OnPropertyChanged(nameof(OpenQty));
                title = Dscription + " [" + OpenQty.ToString() + "]";
            }

        }

        bool _IsButtonVisible;
        [JsonIgnore]
        public bool IsButtonVisible
        {
            get => _IsButtonVisible;
            set
            {
                _IsButtonVisible = value;
                OnPropertyChanged(nameof(IsButtonVisible));
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;

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
