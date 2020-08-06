using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XamarinGRN.Models
{
    public class PurchaseReturn
    {
        public PurchaseReturn()
        {
            PurchaseReturnDetail = new List<PurchaseReturnDetail>();
        }
        public List<PurchaseReturnDetail> PurchaseReturnDetail;
        public string DocNo { get; set; }
        public Company Company { get; set; }
        public vwBusinessPartners CardCode { get; set; }
        public string NumAtCard { get; set; }
    }

    public class PurchaseReturnDetail : INotifyPropertyChanged
    {
        public PurchaseReturnDetail()
        {
            //IsButtonVisible = false;
        }
        [JsonIgnore]
        public string docno
        {
            get => PurchaseDelivery is null ? "" : PurchaseDelivery.DocNo;
        }
        [JsonIgnore]
        public string vendor
        {
            get => LineVendor is null ? "" : LineVendor.CardCode;
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
        private bool _isselected;
        [JsonIgnore]
        public bool isselected
        {
            get => _isselected;
            set
            {
                _isselected = value;
                OnPropertyChanged(nameof(isselected));
            }
        }
        private string _BatchNumber;
        public string BatchNumber
        {
            get => _BatchNumber;
            set
            {
                _BatchNumber = value;
                OnPropertyChanged(nameof(BatchNumber));
            }
        }
        private vwWarehouseBins _BinCode;
        public vwWarehouseBins BinCode
        {
            get => _BinCode;
            set
            {
                _BinCode = value;
                OnPropertyChanged(nameof(BinCode));
                title = settitle();
            }
        }
        public int Oid { get; set; }
        public PurchaseReturn PurchaseDelivery { get; set; }
        public vwItemMasters ItemCode { get; set; }
        public vwWarehouses WhsCode { get; set; }
        public vwBusinessPartners LineVendor { get; set; }
        public string Dscription { get; set; }
        public string UnitMsr { get; set; }
        public string BaseDocNo { get; set; }
        public int Baseline { get; set; }
        public double Quantity { get; set; }
        public double CopyQty { get; set; }
        public bool IsBeingDelete { get; set; }

        double _OpenQty;
        public double OpenQty
        {
            get => _OpenQty;
            set
            {
                _OpenQty = value;
                OnPropertyChanged(nameof(OpenQty));
                title = settitle();
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
        private string settitle()
        {
            if (ItemCode == null) return "";
            return string.Format("({2}) {0} [{1}]", ItemCode.ItemCode, ItemCode.ItemName, BinCode.BinCode);
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
