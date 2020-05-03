using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ConditionalAppearance;

#region update log
// YKW - 20200212 - comments

#endregion

namespace FT_PurchasingPortal.Module.BusinessObjects
{
    //[DefaultClassOptions]
    [Persistent("OMOD")]
    //[ImageName("BO_Contact")]
    [DefaultProperty("BoFullName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    [Appearance("NewRecord", AppearanceItemType = "Action", TargetItems = "New", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsPosted")]
    [Appearance("DeleteRecord", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("LinkRecord", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkRecord", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    public class FTModule : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public FTModule(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        [Index(0)]
        public string BoFullName { get { return "Module"; } }

        private string _PurchaseRequest;
        [XafDisplayName("Purchase Request?")]
        public string PurchaseRequest
        {
            get { return _PurchaseRequest; }
            set { SetPropertyValue("PurchaseRequest", ref _PurchaseRequest, value); }
        }
        private string _PurchaseOrder;
        [XafDisplayName("Purchase Order?")]
        public string PurchaseOrder
        {
            get { return _PurchaseOrder; }
            set { SetPropertyValue("PurchaseOrder", ref _PurchaseOrder, value); }
        }
        private string _SalesQuotation;
        [XafDisplayName("Sales Quotation?")]
        public string SalesQuotation
        {
            get { return _SalesQuotation; }
            set { SetPropertyValue("PurchaseOrder", ref _SalesQuotation, value); }
        }
        private string _SalesAgreement;
        [XafDisplayName("Sales Agreement?")]
        public string SalesAgreement
        {
            get { return _SalesAgreement; }
            set { SetPropertyValue("SalesAgreement", ref _SalesAgreement, value); }
        }
        private string _SalesOrder;
        [XafDisplayName("Sales Order?")]
        public string SalesOrder
        {
            get { return _SalesOrder; }
            set { SetPropertyValue("SalesOrder", ref _SalesOrder, value); }
        }

        private string _StockTransferRequest;
        [XafDisplayName("Stock Request?")]
        public string StockTransferRequest
        {
            get { return _StockTransferRequest; }
            set { SetPropertyValue("StockTransferRequest", ref _StockTransferRequest, value); }
        }
        //private string _PersistentProperty;
        //[XafDisplayName("My display name"), ToolTip("My hint message")]
        //[ModelDefault("EditMask", "(000)-00"), Index(0), VisibleInListView(false)]
        //[Persistent("DatabaseColumnName"), RuleRequiredField(DefaultContexts.Save)]
        //public string PersistentProperty {
        //    get { return _PersistentProperty; }
        //    set { SetPropertyValue("PersistentProperty", ref _PersistentProperty, value); }
        //}

        //[Action(Caption = "My UI Action", ConfirmationMessage = "Are you sure?", ImageName = "Attention", AutoCommit = true)]
        //public void ActionMethod() {
        //    // Trigger a custom business logic for the current record in the UI (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112619.aspx).
        //    this.PersistentProperty = "Paid";
        //}
    }
}