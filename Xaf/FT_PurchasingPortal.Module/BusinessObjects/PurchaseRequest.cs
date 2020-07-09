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
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted)
    [Appearance("CloseDocRecord", AppearanceItemType = "Action", TargetItems = "CloseDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (4))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected || selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted)
    [Appearance("CancelDocRecord", AppearanceItemType = "Action", TargetItems = "CancelDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited)
    [Appearance("RejectDocRecord", AppearanceItemType = "Action", TargetItems = "RejectDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (3))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected || selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted)
    [Appearance("SaveDocRecord", AppearanceItemType = "Action", TargetItems = "Save", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected || selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted)
    [Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited && AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
    [Appearance("ApprovalDocRecord", AppearanceItemType = "Action", TargetItems = "ApprovalDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus = 3 and AppStatus.ApprovalStatus = 2)")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited && AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
    [Appearance("ChangeAppUserRecord", AppearanceItemType = "Action", TargetItems = "ChangeAppUser", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus = 3 and AppStatus.ApprovalStatus = 2)")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.PostedCancel)
    [Appearance("ReOpenDocRecord", AppearanceItemType = "Action", TargetItems = "ReOpenDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (7))")]

    [Persistent("OPRE")]
    public class PurchaseRequest : ClassDocument
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequest(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            AppStatus = new PurchaseRequestApp(Session);
            DocStatus = new PurchaseRequestDoc(Session);
            UDFs = new PurchaseRequestUDF(Session);
            DocType = Session.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", DocTypeCodes.PurchaseRequest));
            DocTypeSeries = Session.FindObject<DocTypeSeries>(CriteriaOperator.Parse("DocType.Oid=? and Company.Oid=?", DocType.Oid, Company.Oid));
            if (CreateUser != null)
            {
                this.IsViewItemPriceRole = CreateUser.CheckAccessVP(DocType.BoCode);
            }
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (PurchaseRequestDetail.Count > 0)
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
        private PurchaseRequestDoc _DocStatus;
        [XafDisplayName("Document Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("DocStatus", Enabled = false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseRequestDoc DocStatus
        {
            get { return _DocStatus; }
            set
            {
                SetPropertyValue("DocStatus", ref _DocStatus, value);
            }
        }
        private PurchaseRequestApp _AppStatus;
        [XafDisplayName("Approval Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("AppStatus", Enabled = false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseRequestApp AppStatus
        {
            get { return _AppStatus; }
            set
            {
                SetPropertyValue("AppStatus", ref _AppStatus, value);
            }
        }

        private PurchaseRequestUDF _UDFs;
        [XafDisplayName("User Define Info.")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseRequestUDF UDFs
        {
            get { return _UDFs; }
            set
            {
                SetPropertyValue("UDFs", ref _UDFs, value);
            }
        }
        [ImmediatePostData]
        [Association("PurchaseRequest-Detail")]
        [XafDisplayName("Details")]
        [DevExpress.Xpo.Aggregated]
        [Appearance("PurchaseRequestrDetail", Enabled = false, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
        //[Appearance("PurchaseRequestDetail2", Enabled = false, Criteria = "not IsCardCodeSelected")]
        public XPCollection<PurchaseRequestDetail> PurchaseRequestDetail
        {
            get { return GetCollection<PurchaseRequestDetail>("PurchaseRequestDetail"); }
        }

        [Association("PurchaseRequest-Attachment")]
        [XafDisplayName("Attachment")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<PurchaseRequestAttachment> PurchaseRequestAttachment
        {
            get { return GetCollection<PurchaseRequestAttachment>("PurchaseRequestAttachment"); }
        }

    }
    #endregion

    #region details
    [Persistent("OPRE1")]
    public class PurchaseRequestDetail : ClassDocumentDetail
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequestDetail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            UDFs = new PurchaseRequestDetailUDF(Session);
            ObjType = Session.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", DocTypeCodes.PurchaseRequest));
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
                if (this.PurchaseRequest != null)
                {
                    this.PurchaseRequest.DocB4Total -= this.LineTotal;
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
                if (this.PurchaseRequest != null)
                {
                    if (!this.PurchaseRequest.IsCopy) this.PurchaseRequest.IsCopy = true;

                    if (this.PurchaseRequest.PurchaseRequestDetail.Count > 0)
                        this.PurchaseRequest.DocB4Total = this.PurchaseRequest.PurchaseRequestDetail.Sum(pp => pp.LineTotal);
                    else
                        this.PurchaseRequest.DocB4Total = 0;
                }
            }
        }
        private PurchaseRequestDetailUDF _UDFs;
        [XafDisplayName("User Define Info.")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseRequestDetailUDF UDFs
        {
            get { return _UDFs; }
            set
            {
                SetPropertyValue("UDFs", ref _UDFs, value);
            }
        }

        private PurchaseRequest _PurchaseRequest;
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(true)]
        [Appearance("PurchaseRequest", Enabled = false)]
        [Association("PurchaseRequest-Detail")]
        public PurchaseRequest PurchaseRequest
        {
            get { return _PurchaseRequest; }
            set
            {
                SetPropertyValue("PurchaseRequest", ref _PurchaseRequest, value);
            }
        }

    }
    #endregion
}