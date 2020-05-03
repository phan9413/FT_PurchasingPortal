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
    [MemberDesignTimeVisibility(true)]
    [NonPersistent]
    [Appearance("SaveAndNewDocDetailRecord", AppearanceItemType = "Action", TargetItems = "SaveAndNew", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("SaveAndCloseDocDetailRecord", AppearanceItemType = "Action", TargetItems = "SaveAndClose", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("DuplicateFontColor", TargetItems = "*", FontColor = "Red", Criteria = "IsDuplicated")]
    [RuleCriteria("ClassDocumentDetailDeleteRule", DefaultContexts.Delete, "IsCanDelete", "Cannot Delete when Target Document found")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClassDocumentDetail : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClassDocumentDetail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            Baseline = 0;
            LineStatus = LineStatusEnum.Open;
            if (!GeneralValues.IsNetCore)
            {
                CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                if (CreateUser.Company != null)
                {
                    Company = Session.GetObjectByKey<Company>(CreateUser.Company.Oid);
                }
                if (CreateUser.Employee != null && CreateUser.Employee.WhsCode != null)
                {
                    WhsCode = Session.GetObjectByKey<vwWarehouses>(CreateUser.Employee.WhsCode.BoKey);
                }
            }
            IsDuplicated = false;
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            IsDuplicated = false;
        }
        protected override void OnSaved()
        {
            base.OnSaved();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                if (!GeneralValues.IsNetCore)
                {
                    SystemUsers user = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    Session.ExecuteSproc("sp_AfterDocDetailUpdated", new OperandValue(user.UserName), new OperandValue(this.Oid), new OperandValue(this.ObjType.BoCode));
                }
            }
        }
        [NonPersistent]
        [Index(999)]
        [Appearance("IsDuplicated", Enabled = false)]
        public bool IsDuplicated { get; set; }
        public bool IsCanDelete
        {
            get
            {
                if (CopyQty > 0) return false;
                if (CopyCreQty > 0) return false;

                return true; 
            }
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
        [Browsable(false)]
        [Appearance("IsViewItemPriceRole", Enabled = false)]
        [NonPersistent]
        public bool IsViewItemPriceRole { get; set; }

        private DocType _ObjType;
        [Browsable(false)]
        public DocType ObjType
        {
            get { return _ObjType; }
            set
            {
                SetPropertyValue("ObjType", ref _ObjType, value);
            }
        }

        private Company _Company;
        [Browsable(false)]
        public Company Company
        {
            get { return _Company; }
            set
            {
                SetPropertyValue("Company", ref _Company, value);
            }
        }
        private vwCurrency _DocCur;
        [Browsable(false)]
        [NoForeignKey]
        public vwCurrency DocCur
        {
            get { return _DocCur; }
            set
            {
                SetPropertyValue("DocCur", ref _DocCur, value);
            }
        }

        private int _VisOrder;
        [Index(0), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [NonCloneableAttribute()]
        public int VisOrder
        {
            get { return _VisOrder; }
            set { SetPropertyValue("VisOrder", ref _VisOrder, value); }
        }
        private vwItemMasters _ItemCode;
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [ImmediatePostData]
        [XafDisplayName("Item Code")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive and (InvntItem = '@This.ObjType.InvntItem' or SellItem = '@This.ObjType.SellItem' or PrchseItem = '@This.ObjType.PrchseItem')")]
        [NoForeignKey]
        [RuleRequiredField(DefaultContexts.Save)]
        public vwItemMasters ItemCode
        {
            get { return _ItemCode; }
            set
            {
                if (SetPropertyValue("ItemCode", ref _ItemCode, value))
                {
                    if (!IsLoading && value != null)
                    {
                        Dscription = value.ItemName;
                        if (ObjType.SellItem == "Y")
                            UnitMsr = value.SalUnitMsr;
                        else if (ObjType.PrchseItem == "Y")
                            UnitMsr = value.BuyUnitMsr;
                    }
                }
            }
        }
        private string _Dscription;
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Description")]
        [DbType("nvarchar(100)")]
        public string Dscription
        {
            get { return _Dscription; }
            set
            {
                SetPropertyValue("Dscription", ref _Dscription, value);
            }
        }
        private vwWarehouses _WhsCode;
        [Index(21), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Warehouse")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [NoForeignKey]
        [RuleRequiredField(DefaultContexts.Save)]
        public vwWarehouses WhsCode
        {
            get { return _WhsCode; }
            set
            {
                SetPropertyValue("WhsCode", ref _WhsCode, value);
            }
        }
        private vwDimension1 _OcrCode;
        [Index(22), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Dimension 1")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [NoForeignKey]
        public vwDimension1 OcrCode
        {
            get { return _OcrCode; }
            set
            {
                SetPropertyValue("OcrCode", ref _OcrCode, value);
            }
        }
        private vwDimension2 _OcrCode2;
        [Index(24), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Dimension 2")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [NoForeignKey]
        public vwDimension2 OcrCode2
        {
            get { return _OcrCode2; }
            set
            {
                SetPropertyValue("OcrCode2", ref _OcrCode2, value);
            }
        }
        private vwDimension3 _OcrCode3;
        [Index(25), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Dimension 3")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [NoForeignKey]
        public vwDimension3 OcrCode3
        {
            get { return _OcrCode3; }
            set
            {
                SetPropertyValue("OcrCode3", ref _OcrCode3, value);
            }
        }
        private vwDimension4 _OcrCode4;
        [Index(26), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Dimension 4")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [NoForeignKey]
        public vwDimension4 OcrCode4
        {
            get { return _OcrCode4; }
            set
            {
                SetPropertyValue("OcrCode4", ref _OcrCode4, value);
            }
        }
        private vwDimension5 _OcrCode5;
        [Index(27), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Dimension 5")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [NoForeignKey]
        public vwDimension5 OcrCode5
        {
            get { return _OcrCode5; }
            set
            {
                SetPropertyValue("OcrCode5", ref _OcrCode5, value);
            }
        }
        private vwProjects _PrjCode;
        [Index(28), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Project Code")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [NoForeignKey]
        public vwProjects PrjCode
        {
            get { return _PrjCode; }
            set
            {
                SetPropertyValue("PrjCode", ref _PrjCode, value);
            }
        }

        private double _Quantity;
        // hide price
        [Index(90), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        [DbType("numeric(19,6)")]
        public double Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
            }
        }
        private string _UnitMsr;
        [Index(91), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public string UnitMsr
        {
            get { return _UnitMsr; }
            set
            {
                SetPropertyValue("UnitMsr", ref _UnitMsr, value);
            }
        }
        private decimal _UnitPrice;
        // hide price
        [EditorAlias("VPDec")]
        [Appearance("dhpUnitPrice", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(92), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [Appearance("UnitPrice", Enabled = false)]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        [DbType("numeric(19,6)")]
        public decimal UnitPrice
        {
            get { return _UnitPrice; }
            set
            {
                SetPropertyValue("UnitPrice", ref _UnitPrice, value);
            }
        }
        private decimal _LineTotal;
        // hide price
        [EditorAlias("VPDec")]
        [Appearance("dhpLineTotal", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(200), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [Appearance("LineTotal", Enabled = false)]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public decimal LineTotal
        {
            get { return _LineTotal; }
            set
            {
                SetPropertyValue("LineTotal", ref _LineTotal, value);
            }
        }
        private decimal _LineTotalFC;
        // hide price
        [EditorAlias("VPDec")]
        [Appearance("dhpLineTotalFC", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(201), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [Appearance("LineTotalFC", Enabled = false)]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public decimal LineTotalFC
        {
            get { return _LineTotalFC; }
            set
            {
                SetPropertyValue("LineTotalFC", ref _LineTotalFC, value);
            }
        }
        private LineStatusEnum _LineStatus;
        [XafDisplayName("Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(202), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("LineStatus", Enabled = false)]
        public LineStatusEnum LineStatus
        {
            get { return _LineStatus; }
            set
            {
                SetPropertyValue("LineStatus", ref _LineStatus, value);
            }
        }

        private vwAccounts _AcctCode;
        [XafDisplayName("Account")]
        [Index(299), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [NoForeignKey]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        public vwAccounts AcctCode
        {
            get { return _AcctCode; }
            set
            {
                SetPropertyValue("AcctCode", ref _AcctCode, value);
            }
        }

        private SystemUsers _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [NonCloneableAttribute()]
        public SystemUsers CreateUser
        {
            get { return _CreateUser; }
            set
            {
                SetPropertyValue("CreateUser", ref _CreateUser, value);
            }
        }

        private DateTime? _CreateDate;
        [Index(301), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [NonCloneableAttribute()]
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set
            {
                SetPropertyValue("CreateDate", ref _CreateDate, value);
            }
        }

        private SystemUsers _UpdateUser;
        [XafDisplayName("Update User"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(302), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public SystemUsers UpdateUser
        {
            get { return _UpdateUser; }
            set
            {
                SetPropertyValue("UpdateUser", ref _UpdateUser, value);
            }
        }

        private DateTime? _UpdateDate;
        [Index(303), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }
        private double _CopyQty;
        [Index(310), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [DbType("numeric(19,6)")]
        public double CopyQty
        {
            get { return _CopyQty; }
            set
            {
                SetPropertyValue("CopyQty", ref _CopyQty, value);
            }
        }
        private double _CopyCreQty;
        [Index(311), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [DbType("numeric(19,6)")]
        public double CopyCreQty
        {
            get { return _CopyCreQty; }
            set
            {
                SetPropertyValue("CopyCreQty", ref _CopyCreQty, value);
            }
        }
        private double _OpenQty;
        [Index(312), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [DbType("numeric(19,6)")]
        public double OpenQty
        {
            get { return _OpenQty; }
            set
            {
                SetPropertyValue("OpenQty", ref _OpenQty, value);
            }
        }
        private double _OpenCreQty;
        [Index(313), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [DbType("numeric(19,6)")]
        public double OpenCreQty
        {
            get { return _OpenCreQty; }
            set
            {
                SetPropertyValue("OpenCreQty", ref _OpenCreQty, value);
            }
        }
        private DocType _BaseType;
        [Index(320), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public DocType BaseType  
        {
            get { return _BaseType; }
            set
            {
                SetPropertyValue("BaseType", ref _BaseType, value);
            }
        }
        private int _Baseline;
        [Index(321), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public int Baseline
        {
            get { return _Baseline; }
            set
            {
                SetPropertyValue("Baseline", ref _Baseline, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                if (Session.IsNewObject(this))
                {
                    CreateDate = DateTime.Now;
                }
                else
                {
                    if (!GeneralValues.IsNetCore)
                        UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    UpdateDate = DateTime.Now;
                }
            }
        }
    }
}