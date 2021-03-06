﻿using System;
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
    //[Appearance("DuplicateFontColor", TargetItems = "*", FontColor = "Red", Criteria = "IsDuplicated")]
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
            }
            else
            {
                CreateUser = Session.FindObject<SystemUsers>(CriteriaOperator.Parse("UserName=?", GeneralValues.NetCoreUserName));
            }
            if (CreateUser != null)
            {
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
            Quantity = 1;
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
                else
                {
                    Session.ExecuteSproc("sp_AfterDocDetailUpdated", new OperandValue(GeneralValues.NetCoreUserName), new OperandValue(this.Oid), new OperandValue(this.ObjType.BoCode));
                }
            }
        }

        [Browsable(false)]
        [NonPersistent]
        [Index(999)]
        [Appearance("IsDuplicated", Enabled = false)]
        public bool IsDuplicated { get; set; }

        [Browsable(false)]
        public bool IsCanDelete
        {
            get
            {
                if (CopyQty > 0) return false;
                if (CopyCreQty > 0) return false;

                return true; 
            }
        }

        /// <summary>
        /// for Webapi
        /// </summary>
        [Browsable(false)]
        public bool IsBeingDelete { get; set; }

        [Browsable(false)]
        public bool IsBatchNumberValid
        {
            get
            {
                if (ItemCode != null)
                {
                    if (ItemCode.ManBtchNum || ItemCode.ManSerNum)
                    {
                        if (!string.IsNullOrWhiteSpace(BatchNumber))
                            return true;
                        else
                            return false;
                    }
                }
                return true;
            }
        }
        [Browsable(false)]
        public bool IsSerialValid
        {
            get
            {
                if (ItemCode != null)
                {
                    if (ItemCode.ManSerNum)
                    {
                        if (Quantity == 1)
                            return true;
                        else
                            return false;
                    }
                }
                return true;
            }
        }
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
        [Appearance("ItemCode", Enabled = false, Criteria = "not IsNew or Baseline > 0")]
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
                        {
                            UnitMsr = value.BuyUnitMsr;
                            if (value.LastPurCur != null && DocCur.CurrCode == value.LastPurCur)
                                UnitPrice = value.LastPurPrc;
                            else
                                UnitPrice = 0;
                        }
                        BatchNumber = "";
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
        private string _ItemDetails;
        [Index(12), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Item Details")]
        [DbType("nvarchar(max)")]
        public string ItemDetails
        {
            get { return _ItemDetails; }
            set
            {
                SetPropertyValue("ItemDetails", ref _ItemDetails, value);
            }
        }
        private vwBusinessPartners _LineVendor;
        [ImmediatePostData]
        [Index(13), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Vendor")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive and CardType = '@This.ObjType.CardType'")]
        [Appearance("LineVendor", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not ObjType.IsReq")]
        [NoForeignKey]
        //[RuleRequiredField(DefaultContexts.Save)]
        public vwBusinessPartners LineVendor
        {
            get { return _LineVendor; }
            set
            {
                if (SetPropertyValue("LineVendor", ref _LineVendor, value))
                {
                    if (!IsLoading)
                    {
                        if (value != null)
                        {
                            DocCur = Session.FindObject<vwCurrency>(CriteriaOperator.Parse("BoKey=?", value.Currency));
                            if (ItemCode != null)
                            {
                                if (DocCur.CurrCode == ItemCode.LastPurCur)
                                    UnitPrice = ItemCode.LastPurPrc;
                                else
                                    UnitPrice = 0;
                            }
                        }
                        else
                        {
                            DocCur = Session.FindObject<vwCurrency>(CriteriaOperator.Parse("CompanyCode=? and CurrCode=?", Company.BoCode, Company.LocalCurreny));
                            if (ItemCode != null)
                            {
                                if (DocCur.CurrCode == ItemCode.LastPurCur)
                                    UnitPrice = ItemCode.LastPurPrc;
                                else
                                    UnitPrice = 0;
                            }
                        }

                    }
                }
            }
        }
        private vwCurrency _DocCur;
        [Index(14), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Currency")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [Appearance("DocCur", Enabled = false, Criteria = "not (LineVendor is null)")]
        [Appearance("DocCur2", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not ObjType.IsReq")]
        [NoForeignKey]
        public vwCurrency DocCur
        {
            get { return _DocCur; }
            set
            {
                SetPropertyValue("DocCur", ref _DocCur, value);
            }
        }

        private vwWarehouses _WhsCode;
        [ImmediatePostData]
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
                if (SetPropertyValue("WhsCode", ref _WhsCode, value))
                {
                    if (!IsLoading && _WhsCode != null)
                    {
                        BinCode = Session.FindObject<vwWarehouseBins>(CriteriaOperator.Parse("WhsCode=? and BinAbsEntry>0", _WhsCode.WhsCode));
                        if (BinCode == null)
                            BinCode = Session.FindObject<vwWarehouseBins>(CriteriaOperator.Parse("WhsCode=? and BinAbsEntry=0", _WhsCode.WhsCode));
                    }
                }
            }
        }
        private vwWarehouseBins _BinCode;
        [Index(22), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Bin")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and WhsCode = '@This.WhsCode.WhsCode' and IsActive")]
        [NoForeignKey]
        [RuleRequiredField(DefaultContexts.Save)]
        public vwWarehouseBins BinCode
        {
            get { return _BinCode; }
            set
            {
                SetPropertyValue("BinCode", ref _BinCode, value);
            }
        }
        private string _BatchNumber;
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Batch/Serial Code")]
        [Appearance("BatchNumber", Enabled = false, Criteria = "not(ItemCode.ManBtchNum or ItemCode.ManSerNum)")]
        [DbType("nvarchar(100)")]
        public string BatchNumber
        {
            get { return _BatchNumber; }
            set
            {
                SetPropertyValue("BatchNumber", ref _BatchNumber, value);
            }
        }
        private vwDimension1 _OcrCode;
        [Index(31), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        [Index(32), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        [Index(33), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        [Index(34), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        [Index(35), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        [Index(36), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        [ImmediatePostData]
        [Index(90), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        [DbType("numeric(19,6)")]
        public double Quantity
        {
            get { return _Quantity; }
            set
            {
                if (SetPropertyValue("Quantity", ref _Quantity, value))
                {
                    if (!IsLoading)
                    {
                        AssignLineTotal();
                    }
                }
            }
        }
        private string _UnitMsr;
        [Index(91), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public string UnitMsr
        {
            get { return _UnitMsr; }
            set
            {
                SetPropertyValue("UnitMsr", ref _UnitMsr, value);
            }
        }
        private double _UnitPrice;
        [ImmediatePostData]
        // hide price
        [EditorAlias("VPDou")]
        [Appearance("dhpUnitPrice", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(92), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        [ModelDefault("DisplayFormat", "{0:n4}")]
        [DbType("numeric(19,6)")]
        public double UnitPrice
        {
            get { return _UnitPrice; }
            set
            {
                if (SetPropertyValue("UnitPrice", ref _UnitPrice, value))
                {
                    if (!IsLoading)
                    {
                        AssignLineTotal();
                    }
                }
            }
        }
        private vwTaxes _TaxCode;
        [ImmediatePostData]
        [Index(100), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Tax Code")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive and Category = '@This.ObjType.TaxCategory'")]
        [NoForeignKey]
        public vwTaxes TaxCode
        {
            get { return _TaxCode; }
            set
            {
                if (SetPropertyValue("TaxCode", ref _TaxCode, value))
                {
                    if (!IsLoading)
                    {
                        AssignTaxAmt();
                    }
                }

            }
        }
        private double _TaxPerc;
        // hide price
        [XafDisplayName("Tax %")]
        [EditorAlias("VPDou")]
        [Appearance("dhpTaxPerc", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(101), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(true)]
        [Appearance("TaxPerc", Enabled = false)]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public double TaxPerc
        {
            get { return _TaxPerc; }
            set
            {
                SetPropertyValue("TaxPerc", ref _TaxPerc, value);

            }
        }
        private decimal _TaxAmt;
        // hide price
        [XafDisplayName("Tax Amount")]
        [EditorAlias("VPDec")]
        [Appearance("dhpTaxAmt", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(102), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(true)]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public decimal TaxAmt
        {
            get { return _TaxAmt; }
            set
            {
                if (SetPropertyValue("TaxAmt", ref _TaxAmt, value))
                {
                    if (!IsLoading)
                    {
                        AssignTaxAmt();
                    }
                }

            }
        }
        private decimal _DiscountAmt;
        [ImmediatePostData]
        // hide price
        [XafDisplayName("Discount")]
        [EditorAlias("VPDec")]
        [Appearance("dhpDiscount", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(198), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public decimal DiscountAmt
        {
            get { return _DiscountAmt; }
            set
            {
                if (SetPropertyValue("DiscountAmt", ref _DiscountAmt, value))
                {
                    if (!IsLoading)
                    {
                        AssignLineTotal();
                    }
                }

            }
        }
        private vwExpenses _FreightCharge;
        [ImmediatePostData]
        [Index(199), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Freight Charge")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [NoForeignKey]
        public vwExpenses FreightCharge
        {
            get { return _FreightCharge; }
            set
            {
                if (SetPropertyValue("FreightCharge", ref _FreightCharge, value))
                {
                    if (!IsLoading)
                    {
                        if (value == null)
                            FreightAmt = 0;

                    }
                }
            }
        }
        private decimal _FreightAmt;
        [ImmediatePostData]
        // hide price
        [XafDisplayName("Freight Amount")]
        [EditorAlias("VPDec")]
        [Appearance("dhpFreightAmt", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(200), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        [Appearance("FreightAmt", Enabled = false, Criteria = "FreightCharge is null")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public decimal FreightAmt
        {
            get { return _FreightAmt; }
            set
            {
                if (SetPropertyValue("FreightAmt", ref _FreightAmt, value))
                {
                    if (!IsLoading)
                    {
                        AssignLineTotal();
                    }
                }

            }
        }

        private decimal _LineTotal;
        // hide price
        [EditorAlias("VPDec")]
        [Appearance("dhpLineTotal", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(201), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
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
        //private decimal _LineTotalFC;
        //// hide price
        //[EditorAlias("VPDec")]
        //[Appearance("dhpLineTotalFC", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        //[Index(201), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        //[Appearance("LineTotalFC", Enabled = false)]
        //[ModelDefault("DisplayFormat", "{0:n2}")]
        //[DbType("numeric(19,6)")]
        //public decimal LineTotalFC
        //{
        //    get { return _LineTotalFC; }
        //    set
        //    {
        //        SetPropertyValue("LineTotalFC", ref _LineTotalFC, value);
        //    }
        //}
        private vwAccounts _AcctCode;
        [XafDisplayName("Account")]
        [Index(210), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
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
        private LineStatusEnum _LineStatus;
        [XafDisplayName("Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(220), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("LineStatus", Enabled = false)]
        public LineStatusEnum LineStatus
        {
            get { return _LineStatus; }
            set
            {
                SetPropertyValue("LineStatus", ref _LineStatus, value);
            }
        }
        [NonCloneable]
        private SystemUsers _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("CreateUser", Enabled = false)]
        public SystemUsers CreateUser
        {
            get { return _CreateUser; }
            set
            {
                SetPropertyValue("CreateUser", ref _CreateUser, value);
            }
        }
        [NonCloneable]
        private DateTime? _CreateDate;
        [Index(301), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("CreateDate", Enabled = false)]
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set
            {
                SetPropertyValue("CreateDate", ref _CreateDate, value);
            }
        }
        [NonCloneable]
        private SystemUsers _UpdateUser;
        [XafDisplayName("Update User"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(302), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("UpdateUser", Enabled = false)]
        public SystemUsers UpdateUser
        {
            get { return _UpdateUser; }
            set
            {
                SetPropertyValue("UpdateUser", ref _UpdateUser, value);
            }
        }
        [NonCloneable]
        private DateTime? _UpdateDate;
        [Index(303), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("UpdateDate", Enabled = false)]
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }
        [NonCloneable]
        private double _CopyQty;
        [Index(310), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [DbType("numeric(19,6)")]
        [Appearance("CopyQty", Enabled = false)]
        public double CopyQty
        {
            get { return _CopyQty; }
            set
            {
                SetPropertyValue("CopyQty", ref _CopyQty, value);
            }
        }
        [NonCloneable]
        private double _CopyCreQty;
        [Index(311), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [DbType("numeric(19,6)")]
        [Appearance("CopyCreQty", Enabled = false)]
        public double CopyCreQty
        {
            get { return _CopyCreQty; }
            set
            {
                SetPropertyValue("CopyCreQty", ref _CopyCreQty, value);
            }
        }
        [NonCloneable]
        private double _OpenQty;
        [Index(312), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [DbType("numeric(19,6)")]
        [Appearance("OpenQty", Enabled = false)]
        public double OpenQty
        {
            get { return _OpenQty; }
            set
            {
                SetPropertyValue("OpenQty", ref _OpenQty, value);
            }
        }
        [NonCloneable]
        private double _OpenCreQty;
        [Index(313), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [DbType("numeric(19,6)")]
        [Appearance("OpenCreQty", Enabled = false)]
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
        [Appearance("BaseType", Enabled = false)]
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
        [Appearance("Baseline", Enabled = false)]
        public int Baseline
        {
            get { return _Baseline; }
            set
            {
                SetPropertyValue("Baseline", ref _Baseline, value);
            }
        }
        private string _BaseDocNo;
        [Index(322), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("BaseDocNo", Enabled = false)]
        public string BaseDocNo
        {
            get { return _BaseDocNo; }
            set
            {
                SetPropertyValue("BaseDocNo", ref _BaseDocNo, value);
            }
        }
        #region sap ref
        private string _SAPObjType;
        [Index(330), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("SAPObjType", Enabled = false)]
        public string SAPObjType
        {
            get { return _SAPObjType; }
            set
            {
                SetPropertyValue("SAPObjType", ref _SAPObjType, value);
            }
        }
        private int _SAPDocEntry;
        [Index(331), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("SAPDocEntry", Enabled = false)]
        public int SAPDocEntry
        {
            get { return _SAPDocEntry; }
            set
            {
                SetPropertyValue("SAPDocEntry", ref _SAPDocEntry, value);
            }
        }
        private int _SAPLineNum;
        [Index(332), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("SAPLineNum", Enabled = false)]
        public int SAPLineNum
        {
            get { return _SAPLineNum; }
            set
            {
                SetPropertyValue("SAPDocEntry", ref _SAPLineNum, value);
            }
        }
        private string _SAPBaseType;
        [Index(340), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("SAPBaseType", Enabled = false)]
        public string SAPBaseType
        {
            get { return _SAPBaseType; }
            set
            {
                SetPropertyValue("SAPBaseType", ref _SAPBaseType, value);
            }
        }
        private int _SAPBaseEntry;
        [Index(341), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("SAPBaseEntry", Enabled = false)]
        public int SAPBaseEntry
        {
            get { return _SAPBaseEntry; }
            set
            {
                SetPropertyValue("SAPDocEntry", ref _SAPBaseEntry, value);
            }
        }
        private int _SAPBaseLine;
        [Index(342), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("SAPBaseLine", Enabled = false)]
        public int SAPBaseLine
        {
            get { return _SAPBaseLine; }
            set
            {
                SetPropertyValue("SAPBaseLine", ref _SAPBaseLine, value);
            }
        }
        #endregion
        private int _MasterOid;
        [Index(400), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("MasterOid", Enabled = false)]
        public int MasterOid
        {
            get { return _MasterOid; }
            set
            {
                SetPropertyValue("MasterOid", ref _MasterOid, value);
            }
        }
        private SystemUsers _DeleteBy;
        [Index(401), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("DeleteBy", Enabled = false)]
        public SystemUsers DeleteBy
        {
            get { return _DeleteBy; }
            set
            {
                SetPropertyValue("DeleteBy", ref _DeleteBy, value);
            }
        }
        private DateTime? _DeleteDate;
        [Index(402), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("DeleteDate", Enabled = false)]
        public DateTime? DeleteDate
        {
            get { return _DeleteDate; }
            set
            {
                SetPropertyValue("DeleteDate", ref _DeleteDate, value);
            }
        }
        private int _VerNo;
        //[Browsable(false)]
        [Appearance("VerNo", Enabled = false)]
        public int VerNo
        {
            get { return _VerNo; }
            set
            {
                SetPropertyValue("VerNo", ref _VerNo, value);
            }
        }
        private int _PostVerNo;
        //[Browsable(false)]
        [Appearance("PostVerNo", Enabled = false)]
        public int PostVerNo
        {
            get { return _PostVerNo; }
            set
            {
                SetPropertyValue("PostVerNo", ref _PostVerNo, value);
            }
        }
        public void AssignTaxAmt()
        {
            TaxAmt = Math.Round(Convert.ToDecimal(Quantity * UnitPrice) * Convert.ToDecimal(TaxPerc) / 100, 2, MidpointRounding.AwayFromZero);
            AssignLineTotal();
        }
        public void AssignLineTotal()
        {
            LineTotal = Math.Round(Convert.ToDecimal(Quantity * UnitPrice) - DiscountAmt + FreightAmt + TaxAmt, 2, MidpointRounding.AwayFromZero);
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        protected override void OnSaving()
        {
            if (ItemCode != null)
            {
                if (ItemCode.ManSerNum)
                    if (Quantity != 1)
                    {
                        throw new Exception(string.Format("Quantity must equal 1, because [{0}] is managed by serial", ItemCode.ItemCode));
                    }
            }
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                IsBeingDelete = false;
                if (Session.IsNewObject(this))
                {
                    CreateDate = DateTime.Now;
                }
                else
                {
                    if (!GeneralValues.IsNetCore)
                        UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    else
                        UpdateUser = Session.FindObject<SystemUsers>(CriteriaOperator.Parse("UserName=?", GeneralValues.NetCoreUserName));
                    UpdateDate = DateTime.Now;
                }
                VerNo++;
            }
        }
    }
}