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
    [Persistent("ODOC")]
    //[ImageName("BO_Contact")]
    [NavigationItem("Setup")]
    [DefaultProperty("BoFullName")]
    [ImageName("ModelEditor_Action_Modules")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    [Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [RuleCriteria("DocTypesDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class DocType : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public DocType(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsActive = true;
            //IsPassAccept = false;
            IsReqApp = false;
            IsNoAppReject = false;
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

        private string _BoCode;
        [XafDisplayName("Code"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleUniqueValue]
        [Appearance("BoCode", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(0)]
        public string BoCode
        {
            get { return _BoCode; }
            set
            {
                SetPropertyValue("BoCode", ref _BoCode, value);
            }
        }

        private string _BoName;
        [XafDisplayName("Name"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(1)]
        public string BoName
        {
            get { return _BoName; }
            set
            {
                SetPropertyValue("BoName", ref _BoName, value);
            }
        }

        [Index(2), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string BoFullName
        {
            get { return BoCode + "-" + BoName; }
        }

        private bool _IsActive;
        [XafDisplayName("Active")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("IsActive", Enabled = false)]
        [Index(10)]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }

        //private bool _IsPassAccept;
        //[ImmediatePostData]
        //[XafDisplayName("Pass = Accept")]
        ////[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        ////[RuleRequiredField(DefaultContexts.Save)]
        //[Index(30)]
        //public bool IsPassAccept
        //{
        //    get { return _IsPassAccept; }
        //    set
        //    {
        //        if (SetPropertyValue("IsPassAccept", ref _IsPassAccept, value))
        //        {
        //            if (!this.IsLoading)
        //            {
        //                IsReqApp = false;
        //                IsNoAppReject = false;
        //            }
        //        }
        //    }
        //}
        private bool _IsReqApp;
        [ImmediatePostData]
        [XafDisplayName("Require Approval")]
        //[Appearance("IsReqApp", Enabled = false, Criteria = "IsPassAccept")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(31)]
        public bool IsReqApp
        {
            get { return _IsReqApp; }
            set
            {
                if (SetPropertyValue("IsReqApp", ref _IsReqApp, value))
                {
                    if (!this.IsLoading)
                    {
                        IsNoAppReject = false;
                    }
                }
            }
        }
        private bool _IsNoAppReject;
        [XafDisplayName("Reject when no Approval")]
        //[Appearance("IsNoAppReject", Enabled = false, Criteria = "IsPassAccept")]
        [Appearance("IsNoAppReject2", Enabled = false, Criteria = "not IsReqApp")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(32)]
        public bool IsNoAppReject
        {
            get { return _IsNoAppReject; }
            set
            {
                SetPropertyValue("IsNoAppReject", ref _IsNoAppReject, value);
            }
        }

        [Browsable(false)]
        public string InvntItem
        {
            get
            {
                switch (this.BoCode)
                {
                    case DocTypeCodes.StockTransferRequest:
                        return "Y";
                }
                return "";
            }
        }
        [Browsable(false)]
        public string SellItem
        {
            get
            {
                switch (this.BoCode)
                {
                    case DocTypeCodes.SalesAgreement:
                    case DocTypeCodes.SalesQuotation:
                    case DocTypeCodes.SalesOrder:
                        return "Y";
                }
                return "";
            }
        }
        [Browsable(false)]
        public string PrchseItem
        {
            get
            {
                switch (this.BoCode)
                {
                    case DocTypeCodes.PurchaseRequest:
                    case DocTypeCodes.PurchaseOrder:
                    case DocTypeCodes.PurchaseDelivery:
                        return "Y";
                }
                return "";
            }
        }
        [Browsable(false)]
        public string CardType
        {
            get
            {
                string rtn = "";
                switch (this.BoCode)
                {
                    case DocTypeCodes.PurchaseRequest:
                    case DocTypeCodes.PurchaseOrder:
                    case DocTypeCodes.PurchaseDelivery:
                        rtn = "S";
                        break;
                    case DocTypeCodes.SalesAgreement:
                    case DocTypeCodes.SalesQuotation:
                    case DocTypeCodes.SalesOrder:
                        rtn = "C";
                        break;
                }
                return rtn;
            }
        }
        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }
        //public bool IsStockType
        //{
        //    get
        //    {
        //        switch (this.BoCode)
        //        {
        //            case DocTypeCodes.SalesDelivery:
        //            case DocTypeCodes.SalesDeliveryOnly:
        //            case DocTypeCodes.SalesReturn:
        //            case DocTypeCodes.PurchaseDelivery:
        //            case DocTypeCodes.PurchaseReturn:
        //            case DocTypeCodes.ProductionIssue:
        //            case DocTypeCodes.ProductionReceive:
        //            case DocTypeCodes.StockIssue:
        //            case DocTypeCodes.StockReceive:
        //            case DocTypeCodes.StockRevalue:
        //            case DocTypeCodes.StockTransfer:
        //                return true;
        //        }
        //        return false;
        //    }
        //}
        //protected override void OnSaved()
        //{
        //    base.OnSaved();

        //    if (!(Session is NestedUnitOfWork)
        //        && (Session.DataLayer != null)
        //            && (Session.ObjectLayer is SimpleObjectLayer)
        //                )
        //    {
        //        DevExpress.Xpo.Metadata.XPClassInfo myClass = Session.GetClassInfo(typeof(Companies));
        //        CriteriaOperator myCriteria = CriteriaOperator.Parse("1=1");
        //        SortingCollection sortProps = new SortingCollection(null);
        //        sortProps.Add(new SortProperty("Oid", DevExpress.Xpo.DB.SortingDirection.Ascending));

        //        var LDocType = Session.GetObjects(myClass, myCriteria, sortProps, 0, false, true);

        //        //XPCollection<DocTypes> LDocType = new XPCollection<DocTypes>();
        //        //LDocType.Load();

        //        bool added = false;
        //        foreach (var dtl in LDocType)
        //        {
        //            DocTypeSeries findobj = Session.FindObject<DocTypeSeries>(CriteriaOperator.Parse("DocType.Oid=? and Company.Oid=?", this.Oid, ((Companies)dtl).Oid));
        //            if (findobj == null)
        //            {
        //                added = true;
        //                int cnt = 0;
        //                if (((Companies)dtl).CompanyDoc != null)
        //                    cnt = ((Companies)dtl).CompanyDoc.Count();

        //                cnt++;
        //                DocTypeSeries obj = new DocTypeSeries(Session);
        //                obj.Company = Session.FindObject<Companies>(new BinaryOperator("Oid", ((Companies)dtl).Oid, BinaryOperatorType.Equal));
        //                obj.DocType = Session.FindObject<DocTypes>(new BinaryOperator("Oid", this.Oid, BinaryOperatorType.Equal));
        //                obj.NextDocNo = (cnt * 1000000) + 1;
        //            }
        //        }
        //        if (added)
        //            Session.CommitTransaction();
        //    }

        //}
    }
}