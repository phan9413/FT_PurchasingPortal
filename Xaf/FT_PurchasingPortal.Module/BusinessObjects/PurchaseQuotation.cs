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
    /// <summary>
    /// 20200707 remove post feature in ClassDocument for sync PO purpose
    /// </summary>
    #region header
    [DefaultClassOptions]
    [NavigationItem("Purchasing")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected)
    [Appearance("SubmitDocRecord", AppearanceItemType = "Action", TargetItems = "SubmitDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2))")]
    //1 = 1
    [Appearance("PostDocRecord", AppearanceItemType = "Action", TargetItems = "PostDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "1 = 1")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted )
    [Appearance("CloseDocRecord", AppearanceItemType = "Action", TargetItems = "CloseDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (4))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected || selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted)
    [Appearance("CancelDocRecord", AppearanceItemType = "Action", TargetItems = "CancelDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited)
    [Appearance("RejectDocRecord", AppearanceItemType = "Action", TargetItems = "RejectDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (3))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected || selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted)
    [Appearance("SaveDocRecord", AppearanceItemType = "Action", TargetItems = "Save", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
    //[Appearance("SaveDocRecord2", AppearanceItemType = "Action", TargetItems = "Save", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsCardCodeSelected")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected || selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted)
    [Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited && AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
    [Appearance("ApprovalDocRecord", AppearanceItemType = "Action", TargetItems = "ApprovalDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus = 3 and AppStatus.ApprovalStatus = 2)")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited && AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
    [Appearance("ChangeAppUserRecord", AppearanceItemType = "Action", TargetItems = "ChangeAppUser", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus = 3 and AppStatus.ApprovalStatus = 2)")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.PostedCancel)
    [Appearance("ReOpenDocRecord", AppearanceItemType = "Action", TargetItems = "ReOpenDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (7))")]

    [Persistent("OPRU")]
    public class PurchaseQuotation : ClassDocument
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseQuotation(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            AppStatus = new PurchaseQuotationApp(Session);
            DocStatus = new PurchaseQuotationDoc(Session);
            UDFs = new PurchaseQuotationUDF(Session);
            DocType = Session.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", DocTypeCodes.PurchaseQuotation));
            DocTypeSeries = Session.FindObject<DocTypeSeries>(CriteriaOperator.Parse("DocType.Oid=? and Company.Oid=?", DocType.Oid, Company.Oid));
            if (CreateUser != null)
            {
                this.IsViewItemPriceRole = CreateUser.CheckAccessVP(DocType.BoCode);
            }
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (PurchaseQuotationDetail.Count > 0)
                IsCopy = true;

            if (!GeneralValues.IsNetCore)
            {
                if (!string.IsNullOrEmpty(SecuritySystem.CurrentUserId.ToString()))
                {
                    SystemUsers user = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    if (user != null)
                    {
                        this.IsViewItemPriceRole = CreateUser.CheckAccessVP(DocType.BoCode);
                    }
                }
            }
        }
        private PurchaseQuotationDoc _DocStatus;
        [XafDisplayName("Document Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("DocStatus", Enabled = false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseQuotationDoc DocStatus
        {
            get { return _DocStatus; }
            set
            {
                SetPropertyValue("DocStatus", ref _DocStatus, value);
            }
        }
        private PurchaseQuotationApp _AppStatus;
        [XafDisplayName("Approval Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("AppStatus", Enabled = false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseQuotationApp AppStatus
        {
            get { return _AppStatus; }
            set
            {
                SetPropertyValue("AppStatus", ref _AppStatus, value);
            }
        }

        private PurchaseQuotationUDF _UDFs;
        [XafDisplayName("User Define Info.")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseQuotationUDF UDFs
        {
            get { return _UDFs; }
            set
            {
                SetPropertyValue("UDFs", ref _UDFs, value);
            }
        }
        [ImmediatePostData]
        [Association("PurchaseQuotation-Detail")]
        [XafDisplayName("Details")]
        [DevExpress.Xpo.Aggregated]
        [Appearance("PurchaseQuotationDetail", Enabled = false, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4, 7))")]
        [Appearance("PurchaseQuotationDetail2", Enabled = false, Criteria = "not IsCardCodeSelected")]
        public XPCollection<PurchaseQuotationDetail> PurchaseQuotationDetail
        {
            get { return GetCollection<PurchaseQuotationDetail>("PurchaseQuotationDetail"); }
        }

        [Association("PurchaseQuotation-Attachment")]
        [XafDisplayName("Attachment")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<PurchaseQuotationAttachment> PurchaseQuotationAttachment
        {
            get { return GetCollection<PurchaseQuotationAttachment>("PurchaseQuotationAttachment"); }
        }

    }
    #endregion

    #region details
    [Persistent("OPRU1")]
    public class PurchaseQuotationDetail : ClassDocumentDetail
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseQuotationDetail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            UDFs = new PurchaseQuotationDetailUDF(Session);
            ObjType = Session.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", DocTypeCodes.PurchaseQuotation));
            if (CreateUser != null)
            {
                this.IsViewItemPriceRole = CreateUser.CheckAccessVP(ObjType.BoCode);
            }
            if (!string.IsNullOrEmpty(Company.InputTax))
                TaxCode = Session.FindObject<vwTaxes>(CriteriaOperator.Parse("CompanyCode=? and Code=?", Company.BoCode, Company.InputTax));
            if (CreateUser.Employee.WhsCode != null)
                WhsCode = Session.FindObject<vwWarehouses>(CriteriaOperator.Parse("CompanyCode=? and WhsCode=?", Company.BoCode, CreateUser.Employee.WhsCode.WhsCode));
            else if (!string.IsNullOrEmpty(Company.WhsCode))
                WhsCode = Session.FindObject<vwWarehouses>(CriteriaOperator.Parse("CompanyCode=? and WhsCode=?", Company.BoCode, Company.WhsCode));
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();

            if (!GeneralValues.IsNetCore)
            {
                if (!string.IsNullOrEmpty(SecuritySystem.CurrentUserId.ToString()))
                {
                    SystemUsers user = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    if (user != null)
                    {
                        this.IsViewItemPriceRole = CreateUser.CheckAccessVP(ObjType.BoCode);
                    }
                }
            }
        }
        protected override void OnDeleting()
        {
            base.OnDeleting();
            /// Aggregate
            //if (!(Session is NestedUnitOfWork)
            //   && (Session.DataLayer != null)
            //       && (Session.ObjectLayer is DevExpress.ExpressApp.Security.ClientServer.SecuredSessionObjectLayer)
            //           )
            {
                if (this.PurchaseQuotation != null)
                {
                    this.PurchaseQuotation.DocB4Total -= this.LineTotal;
                }
            }
        }
        protected override void OnSaving()
        {
            base.OnSaving();
            /// Aggregate
            if ((Session is NestedUnitOfWork)
               && (Session.DataLayer != null)
                   && (Session.ObjectLayer is SessionObjectLayer)
                       )
            {
                if (this.PurchaseQuotation != null)
                {
                    if (this.PurchaseQuotation.PurchaseQuotationDetail.Count > 0)
                        this.PurchaseQuotation.DocB4Total = this.PurchaseQuotation.PurchaseQuotationDetail.Sum(pp => pp.LineTotal);
                    else
                        this.PurchaseQuotation.DocB4Total = 0;
                }
            }
        }
        private PurchaseQuotationDetailUDF _UDFs;
        [XafDisplayName("User Define Info.")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseQuotationDetailUDF UDFs
        {
            get { return _UDFs; }
            set
            {
                SetPropertyValue("UDFs", ref _UDFs, value);
            }
        }

        private PurchaseQuotation _PurchaseQuotation;
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(true)]
        [Appearance("PurchaseQuotation", Enabled = false)]
        [Association("PurchaseQuotation-Detail")]
        public PurchaseQuotation PurchaseQuotation
        {
            get { return _PurchaseQuotation; }
            set
            {
                SetPropertyValue("PurchaseQuotation", ref _PurchaseQuotation, value);
            }
        }

    }
    #endregion
}