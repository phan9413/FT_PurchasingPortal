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
    [NavigationItem("Purchase Return")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected)
    [Appearance("SubmitDocRecord", AppearanceItemType = "Action", TargetItems = "SubmitDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2))")]
    //1 = 1
    [Appearance("PostDocRecord", AppearanceItemType = "Action", TargetItems = "PostDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "1 = 1")]
    //1 = 1
    [Appearance("CloseDocRecord", AppearanceItemType = "Action", TargetItems = "CloseDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "1 = 1")]
    //1 = 1
    [Appearance("CancelDocRecord", AppearanceItemType = "Action", TargetItems = "CancelDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "1 = 1")]
    //1 = 1
    [Appearance("RejectDocRecord", AppearanceItemType = "Action", TargetItems = "RejectDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "1 = 1")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected || selectedObject.DocStatus.CurrDocStatus == DocStatus.PostedCancel)
    [Appearance("SaveDocRecord", AppearanceItemType = "Action", TargetItems = "Save", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 7))")]
    //if (selectedObject.DocStatus.IsSAPPosted)
    [Appearance("SaveDocRecord2", AppearanceItemType = "Action", TargetItems = "Save", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "DocStatus.IsSAPPosted")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected || selectedObject.DocStatus.CurrDocStatus == DocStatus.PostedCancel)
    [Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 7))")]
    //if (selectedObject.DocStatus.IsSAPPosted)
    [Appearance("EditRecord2", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "DocStatus.IsSAPPosted")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited && AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
    [Appearance("ApprovalDocRecord", AppearanceItemType = "Action", TargetItems = "ApprovalDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus = 3 and AppStatus.ApprovalStatus = 2)")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited && AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
    [Appearance("ChangeAppUserRecord", AppearanceItemType = "Action", TargetItems = "ChangeAppUser", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus = 3 and AppStatus.ApprovalStatus = 2)")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.PostedCancel)
    [Appearance("ReOpenDocRecord", AppearanceItemType = "Action", TargetItems = "ReOpenDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (7))")]

    [Persistent("OPRR")]
    public class PurchaseReturn : ClassDocument
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseReturn(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            AppStatus = new PurchaseReturnApp(Session);
            DocStatus = new PurchaseReturnDoc(Session);
            UDFs = new PurchaseReturnUDF(Session);
            DocType = Session.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", DocTypeCodes.PurchaseReturn));
            DocTypeSeries = Session.FindObject<DocTypeSeries>(CriteriaOperator.Parse("DocType.Oid=? and Company.Oid=?", DocType.Oid, Company.Oid));
            
            if (CreateUser != null)
            {
                this.IsViewItemPriceRole = CreateUser.CheckAccessVP(DocType.BoCode);
            }
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (PurchaseReturnDetail.Count > 0)
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
        private PurchaseReturnDoc _DocStatus;
        [XafDisplayName("Document Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("DocStatus", Enabled = false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseReturnDoc DocStatus
        {
            get { return _DocStatus; }
            set
            {
                SetPropertyValue("DocStatus", ref _DocStatus, value);
            }
        }
        private PurchaseReturnApp _AppStatus;
        [XafDisplayName("Approval Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("AppStatus", Enabled = false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseReturnApp AppStatus
        {
            get { return _AppStatus; }
            set
            {
                SetPropertyValue("AppStatus", ref _AppStatus, value);
            }
        }

        private PurchaseReturnUDF _UDFs;
        [XafDisplayName("User Define Info.")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseReturnUDF UDFs
        {
            get { return _UDFs; }
            set
            {
                SetPropertyValue("UDFs", ref _UDFs, value);
            }
        }
        [ImmediatePostData]
        [Association("PurchaseReturn-Detail")]
        [XafDisplayName("Details")]
        [DevExpress.Xpo.Aggregated]
        [Appearance("PurchaseReturnDetail", Enabled = false, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4, 7))")]
        [Appearance("PurchaseReturnDetail2", Enabled = false, Criteria = "not IsCardCodeSelected")]
        public XPCollection<PurchaseReturnDetail> PurchaseReturnDetail
        {
            get { return GetCollection<PurchaseReturnDetail>("PurchaseReturnDetail"); }
        }

        [Association("PurchaseReturn-Attachment")]
        [XafDisplayName("Attachment")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<PurchaseReturnAttachment> PurchaseReturnAttachment
        {
            get { return GetCollection<PurchaseReturnAttachment>("PurchaseReturnAttachment"); }
        }

    }
    #endregion

    #region details
    [Persistent("OPRR1")]
    [RuleCriteria("PurchaseReturnDetailSaveRule", DefaultContexts.Save, "IsBatchNumberValid", "Batch Number is not valid")]
    [RuleCriteria("PurchaseReturnDetailSaveRule2", DefaultContexts.Save, "IsSerialValid", "Quantity must be 1 when managed by serial")]
    public class PurchaseReturnDetail : ClassDocumentDetail
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseReturnDetail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            UDFs = new PurchaseReturnDetailUDF(Session);
            ObjType = Session.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", DocTypeCodes.PurchaseReturn));
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
                if (this.PurchaseReturn != null)
                {
                    this.PurchaseReturn.DocB4Total -= this.LineTotal;
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
                if (this.PurchaseReturn != null)
                {
                    if (this.PurchaseReturn.PurchaseReturnDetail.Count > 0)
                        this.PurchaseReturn.DocB4Total = this.PurchaseReturn.PurchaseReturnDetail.Sum(pp => pp.LineTotal);
                    else
                        this.PurchaseReturn.DocB4Total = 0;
                }
            }
        }

        private PurchaseReturnDetailUDF _UDFs;
        [XafDisplayName("User Define Info.")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseReturnDetailUDF UDFs
        {
            get { return _UDFs; }
            set
            {
                SetPropertyValue("UDFs", ref _UDFs, value);
            }
        }

        private PurchaseReturn _PurchaseReturn;
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(true)]
        [Appearance("PurchaseReturn", Enabled = false)]
        [Association("PurchaseReturn-Detail")]
        public PurchaseReturn PurchaseReturn
        {
            get { return _PurchaseReturn; }
            set
            {
                SetPropertyValue("PurchaseReturn", ref _PurchaseReturn, value);
            }
        }

    }
    #endregion
}