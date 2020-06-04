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
    [Appearance("SaveAndNewDocRecord", AppearanceItemType = "Action", TargetItems = "SaveAndNew", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("SaveAndCloseDocRecord", AppearanceItemType = "Action", TargetItems = "SaveAndClose", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("ClassDocumentDeleteRecord", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClassDocument : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClassDocument(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            DocNum = 0;
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
                DocOwner = Session.FindObject<Employee>(new BinaryOperator("SystemUser.Oid", CreateUser.Oid, BinaryOperatorType.Equal));
            }
            //if (CreateUser.Employee != null)
            //{
            //    DocOwner = Session.GetObjectByKey<Employee>(CreateUser.Employee.Oid);
            //}
            DocDate = DateTime.Today;
            DocDueDate = DateTime.Today;
            TaxDate = DateTime.Today;
            ReqDate = DateTime.Today;
            IsCopy = false;
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
                    Session.ExecuteSproc("sp_AfterDocUpdated", new OperandValue(user.UserName), new OperandValue(this.Oid), new OperandValue(this.DocType.BoCode));
                }
                else
                {
                    Session.ExecuteSproc("sp_AfterDocUpdated", new OperandValue(GeneralValues.NetCoreUserName), new OperandValue(this.Oid), new OperandValue(this.DocType.BoCode));
                }
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

        private Company _Company;
        [Index(0), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Company", Enabled = false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public Company Company
        {
            get { return _Company; }
            set
            {
                SetPropertyValue("Company", ref _Company, value);
            }
        }

        private DocType _DocType;
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("DocType", Enabled = false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public DocType DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }
        private DocTypeSeries _DocTypeSeries;
        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [DataSourceCriteria("Company.BoCode = '@This.Company.BoCode' and DocType.BoCode = '@This.DocType.BoCode'")]
        [Appearance("DocTypeSeries", Enabled = false, Criteria = "not IsNew")]
        //[Appearance("DocTypeSeries", Enabled = false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public DocTypeSeries DocTypeSeries
        {
            get { return _DocTypeSeries; }
            set
            {
                SetPropertyValue("DocTypeSeries", ref _DocTypeSeries, value);
            }
        }
        private int _DocNum;
        [Browsable(false)]
        public int DocNum
        {
            get { return _DocNum; }
            set
            {
                SetPropertyValue("DocNum", ref _DocNum, value);
            }
        }
        private string _DocNo;
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("DocNo", Enabled = false)]
        [XafDisplayName("Document Number")]
        public string DocNo
        {
            get { return _DocNo; }
            set
            {
                SetPropertyValue("DocNo", ref _DocNo, value);
            }
        }
        private DateTime _DocDate;
        [Index(4), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Posting Date")]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime DocDate
        {
            get { return _DocDate; }
            set
            {
                SetPropertyValue("DocDate", ref _DocDate, value);
            }
        }
        private DateTime _DocDueDate;
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Due Date")]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime DocDueDate
        {
            get { return _DocDueDate; }
            set
            {
                SetPropertyValue("DocDueDate", ref _DocDueDate, value);
            }
        }
        private DateTime _TaxDate;
        [Index(6), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Doc Date")]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime TaxDate
        {
            get { return _TaxDate; }
            set
            {
                SetPropertyValue("TaxDate", ref _TaxDate, value);
            }
        }
        private DateTime _ReqDate;
        [Index(7), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Required Date")]
        [RuleRequiredField(DefaultContexts.Save)]
        public DateTime ReqDate
        {
            get { return _ReqDate; }
            set
            {
                SetPropertyValue("ReqDate", ref _ReqDate, value);
            }
        }

        private vwBusinessPartners _CardCode;
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [ImmediatePostData]
        [XafDisplayName("Business Partner")]
        [Appearance("CardCode", Enabled = false, Criteria = "IsCopy")]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive and CardType = '@This.DocType.CardType'")]
        [NoForeignKey]
        //[RuleRequiredField(DefaultContexts.Save)]
        public vwBusinessPartners CardCode
        {
            get { return _CardCode; }
            set
            {
                if (SetPropertyValue("CardCode", ref _CardCode, value))
                {
                    if (!IsLoading && value != null)
                    {
                        CardName = value.CardName;
                        CncttCode = null;
                        ShipToCode = null;
                        BillToCode = null;
                        DocCur = Session.FindObject<vwCurrency>(CriteriaOperator.Parse("CurrCode=?", value.Currency));
                    }
                }
            }
        }

        private string _CardName;
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Name")]
        [DbType("nvarchar(100)")]
        public string CardName
        {
            get { return _CardName; }
            set
            {
                SetPropertyValue("CardName", ref _CardName, value);
            }
        }
        private vwContactPersons _CncttCode;
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and CardCode = '@This.CardCode.CardCode'")]
        [XafDisplayName("Contact Person")]
        [NoForeignKey]
        public vwContactPersons CncttCode
        {
            get { return _CncttCode; }
            set
            {
                SetPropertyValue("CncttCode", ref _CncttCode, value);
            }
        }
        private string _NumAtCard;
        [Index(21), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Ref No")]
        public string NumAtCard
        {
            get { return _NumAtCard; }
            set
            {
                SetPropertyValue("NumAtCard", ref _NumAtCard, value);
            }
        }
        private vwCurrency _DocCur;
        [Index(22), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [NoForeignKey]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [XafDisplayName("Currency Code")]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("DocCur", Enabled = false)]
        public vwCurrency DocCur
        {
            get { return _DocCur; }
            set
            {
                SetPropertyValue("DocCur", ref _DocCur, value);
            }
        }
        private vwShipToAddress _ShipToCode;
        [Index(30), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [ImmediatePostData]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and CardCode = '@This.CardCode.CardCode'")]
        [XafDisplayName("Ship to")]
        [NoForeignKey]
        public vwShipToAddress ShipToCode
        {
            get { return _ShipToCode; }
            set
            {
                if (SetPropertyValue("ShipToCode", ref _ShipToCode, value))
                {
                    if (!IsLoading && value != null)
                    {
                        Address2 = value.Address;
                    }
                }
            }
        }
        private string _Address2;
        [Index(31), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Ship to Address")]
        [DbType("nvarchar(254)")]
        public string Address2
        {
            get { return _Address2; }
            set
            {
                SetPropertyValue("Address2", ref _Address2, value);
            }
        }
        private vwBillToAddress _BillToCode;
        [Index(32), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [ImmediatePostData]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and CardCode = '@This.CardCode.CardCode'")]
        [XafDisplayName("Ship to")]
        [NoForeignKey]
        public vwBillToAddress BillToCode
        {
            get { return _BillToCode; }
            set
            {
                if (SetPropertyValue("BillToCode", ref _BillToCode, value))
                {
                    if (!IsLoading && value != null)
                    {
                        Address = value.Address;
                    }
                }
            }
        }
        private string _Address;
        [Index(33), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Bill to Address")]
        [DbType("nvarchar(254)")]
        public string Address
        {
            get { return _Address; }
            set
            {
                SetPropertyValue("Address", ref _Address, value);
            }
        }
        private vwSalesPersons _SlpCode;
        [Index(50), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode'")]
        [XafDisplayName("Sales Person")]
        [NoForeignKey]
        public vwSalesPersons SlpCode
        {
            get { return _SlpCode; }
            set
            {
                SetPropertyValue("SlpCode", ref _SlpCode, value);
            }
        }
        private string _JrnMemo;
        [Index(51), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Journal Memo")]
        [DbType("nvarchar(50)")]
        public string JrnMemo
        {
            get { return _JrnMemo; }
            set
            {
                SetPropertyValue("JrnMemo", ref _JrnMemo, value);
            }
        }
        private string _Comments;
        [Index(53), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [XafDisplayName("Remarks")]
        [DbType("nvarchar(254)")]
        public string Comments
        {
            get { return _Comments; }
            set
            {
                SetPropertyValue("Comments", ref _Comments, value);
            }
        }

        private decimal _DocB4Total;
        [ImmediatePostData]
        [XafDisplayName("Total Before Disc.")]
        // hide price
        [EditorAlias("VPDec")]
        [Appearance("dhpDocB4Total", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(200), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [Appearance("DocB4Total", Enabled = false)]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public decimal DocB4Total
        {
            get { return _DocB4Total; }
            set
            {
                if (SetPropertyValue("DocB4Total", ref _DocB4Total, value))
                {
                    if (!IsLoading)
                    {
                        AssignDocTotal(DocB4Total);
                    }
                }

            }
        }

        private Double _DiscountPerc;
        [ImmediatePostData]
        // hide price
        [XafDisplayName("Discount %")]
        [EditorAlias("VPDou")]
        [Appearance("dhpDiscountPerc", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(201), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [ModelDefault("DisplayFormat", "{0:n3}")]
        [DbType("numeric(19,6)")]
        public Double DiscountPerc
        {
            get { return _DiscountPerc; }
            set
            {
                if (SetPropertyValue("DiscountPerc", ref _DiscountPerc, value))
                {
                    if (!IsLoading)
                    {
                        AssignDocTotal(DocB4Total);
                    }
                }

            }
        }
        private decimal _DiscountAmt;
        [XafDisplayName("Discount Amount")]
        // hide price
        [EditorAlias("VPDec")]
        [Appearance("dhpDiscountAmt", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(202), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [Appearance("DiscountAmt", Enabled = false)]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public decimal DiscountAmt
        {
            get { return _DiscountAmt; }
            set
            {
                SetPropertyValue("DiscountAmt", ref _DiscountAmt, value);
            }
        }
        private decimal _DiscountAdj;
        [XafDisplayName("Discount Adj.")]
        // hide price
        [ImmediatePostData]
        [EditorAlias("VPDec")]
        [Appearance("dhpDiscountAdj", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(203), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public decimal DiscountAdj
        {
            get { return _DiscountAdj; }
            set
            {
                if (SetPropertyValue("DiscountAdj", ref _DiscountAdj, value))
                {
                    if (!IsLoading)
                    {
                        AssignDocTotal(DocB4Total);
                    }
                }
            }
        }

        private decimal _Rounding;
        // hide price
        [ImmediatePostData]
        [EditorAlias("VPDec")]
        [Appearance("dhpRounding", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(210), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public decimal Rounding
        {
            get { return _Rounding; }
            set
            {
                if (SetPropertyValue("Rounding", ref _Rounding, value))
                {
                    if (!IsLoading)
                    {
                        AssignDocTotal(DocB4Total);
                    }
                }
            }
        }

        private decimal _DocTotal;
        // hide price
        [XafDisplayName("Total After Disc.")]
        [EditorAlias("VPDec")]
        [Appearance("dhpDocTotal", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        [Index(211), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        [Appearance("DocTotal", Enabled = false)]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [DbType("numeric(19,6)")]
        public decimal DocTotal
        {
            get { return _DocTotal; }
            set
            {
                SetPropertyValue("DocTotal", ref _DocTotal, value);
            }
        }
        //private decimal _DocTotalFC;
        //// hide price
        //[EditorAlias("VPDec")]
        //[Appearance("dhpDocTotalFC", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsViewItemPriceRole")]
        //[Index(201), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        //[Appearance("DocTotalFC", Enabled = false)]
        //[ModelDefault("DisplayFormat", "{0:n2}")]
        //[DbType("numeric(19,6)")]
        //public decimal DocTotalFC
        //{
        //    get { return _DocTotalFC; }
        //    set
        //    {
        //        SetPropertyValue("DocTotalFC", ref _DocTotalFC, value);
        //    }
        //}
        private Employee _DocOwner;
        [XafDisplayName("Document Owner")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[Appearance("DocOwner", Enabled = false)]
        [Index(299), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public Employee DocOwner
        {
            get { return _DocOwner; }
            set
            {
                SetPropertyValue("DocOwner", ref _DocOwner, value);
            }
        }

        private SystemUsers _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
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

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }
        [Browsable(false)]
        public bool IsCardCodeSelected
        {
            get
            {
                if (CardCode != null) return true;
                return false;
            }
        }

        [Browsable(false), NonPersistent]
        public bool IsDocUserCheck { get; set; }

        [Browsable(false), NonPersistent]
        public bool IsDocSuperUserCheck { get; set; }

        [Browsable(false), NonPersistent]
        public bool IsDocCloseUserCheck { get; set; }

        [Browsable(false), NonPersistent]
        public bool IsDocRejectUserCheck { get; set; }

        [Browsable(false), NonPersistent]
        public bool IsDocApprovalUserCheck { get; set; }

        [Browsable(false), NonPersistent]
        public bool IsCopy { get; set; }

        public void AssignDocNumber()
        {
            if (DocNum > 0) return;
            DocTypeSeriesDoc doc = Session.FindObject<DocTypeSeriesDoc>(CriteriaOperator.Parse("DocTypeSeries.Company.Oid=? and DocTypeSeries.Oid=?", Company.Oid, DocTypeSeries.Oid));
            if (doc == null)
            {
                //doc = new DocTypeSeriesDoc(Session);
                //doc.DocTypeSeries = doc.Session.GetObjectByKey<DocTypeSeries>(DocTypeSeries.Oid);
                throw new Exception("Series next number not found");
            }
            DocNum = doc.NextDocNo;
            DocNo = doc.DocTypeSeries.Prefix == null ? DocNum.ToString() : doc.DocTypeSeries.Prefix.Trim() + DocNum.ToString();
            doc.NextDocNo++;
        }
        public void AssignDocTotal(decimal docb4total)
        {
            DocB4Total = Math.Round(docb4total, 2, MidpointRounding.AwayFromZero);
            decimal discountamt = Math.Round(DocB4Total * Convert.ToDecimal(DiscountPerc) / 100, 2, MidpointRounding.AwayFromZero);
            DiscountAmt = discountamt;
            DocTotal = DocB4Total - DiscountAmt - DiscountAdj + Rounding;
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
                    else
                        UpdateUser = Session.FindObject<SystemUsers>(CriteriaOperator.Parse("UserName=?", GeneralValues.NetCoreUserName));
                    UpdateDate = DateTime.Now;
                }
            }
        }
        
    }
}