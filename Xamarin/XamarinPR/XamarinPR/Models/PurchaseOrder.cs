﻿using Newtonsoft.Json;
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

        public string DocNo { get; set; }
        public Company Company { get; set; }
        public vwBusinessPartners CardCode { get; set; }
    }

    public class PurchaseOrderDetail : INotifyPropertyChanged
    {
        public PurchaseOrderDetail()
        {
            //IsButtonVisible = false;
        }
        [JsonIgnore]
        public string docno
        {
            get => PurchaseOrder.DocNo;
        }

        private string _title;
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
        public int Oid { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public vwItemMasters ItemCode { get; set; }

        private vwWarehouses _WhsCode;
        public vwWarehouses WhsCode
        {
            get => _WhsCode;
            set
            {
                _WhsCode = value;
                OnPropertyChanged(nameof(WhsCode));
                title = settitle();
            }
        }
        public string Dscription { get; set; }
        public string UnitMsr { get; set; }
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
                title = settitle();
            }

        }
        private string settitle()
        {
            return string.Format("{0} [{1}] ({2})", ItemCode.ItemCode, ItemCode.ItemName, WhsCode.WhsCode);
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
