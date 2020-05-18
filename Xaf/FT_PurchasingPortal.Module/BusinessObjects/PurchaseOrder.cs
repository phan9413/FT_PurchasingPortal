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
    #region header
    [DefaultClassOptions]
    [NavigationItem("Purchasing")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected)
    [Appearance("SubmitDocRecord", AppearanceItemType = "Action", TargetItems = "SubmitDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Closed)
    [Appearance("PostDocRecord", AppearanceItemType = "Action", TargetItems = "PostDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (5))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted || selectedObject.DocStatus.CurrDocStatus == DocStatus.PostedCancel)
    [Appearance("CloseDocRecord", AppearanceItemType = "Action", TargetItems = "CloseDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (4, 7))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected)
    [Appearance("CancelDocRecord", AppearanceItemType = "Action", TargetItems = "CancelDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited || selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted || selectedObject.DocStatus.CurrDocStatus == DocStatus.Closed || selectedObject.DocStatus.CurrDocStatus == DocStatus.PostedCancel)
    [Appearance("RejectDocRecord", AppearanceItemType = "Action", TargetItems = "RejectDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (3, 4, 5, 7))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected || selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted)
    [Appearance("SaveDocRecord", AppearanceItemType = "Action", TargetItems = "Save", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
    [Appearance("SaveDocRecord2", AppearanceItemType = "Action", TargetItems = "Save", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsCardCodeSelected")]

    [Persistent("OPRQ")]
    public class PurchaseOrder : ClassDocument
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseOrder(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            AppStatus = new PurchaseOrderApp(Session);
            DocStatus = new PurchaseOrderDoc(Session);
            UDFs = new PurchaseOrderUDF(Session);
            DocType = Session.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", DocTypeCodes.PurchaseOrder));
            DocTypeSeries = Session.FindObject<DocTypeSeries>(CriteriaOperator.Parse("DocType.Oid=? and Company.Oid=?", DocType.Oid, Company.Oid));
            if (CreateUser != null)
            {
                this.IsViewItemPriceRole = CreateUser.CheckAccessVP(DocType.BoCode);
            }
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (DocStatus.CurrDocStatus != BusinessObjects.DocStatus.Draft)
                IsCopy = true;

            if (!GeneralValues.IsNetCore)
            {
                SystemUsers user = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                if (user != null)
                {
                    this.IsViewItemPriceRole = CreateUser.CheckAccessVP(DocType.BoCode);
                }
            }
        }
        private PurchaseOrderDoc _DocStatus;
        [XafDisplayName("Document Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("DocStatus", Enabled = false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseOrderDoc DocStatus
        {
            get { return _DocStatus; }
            set
            {
                SetPropertyValue("DocStatus", ref _DocStatus, value);
            }
        }
        private PurchaseOrderApp _AppStatus;
        [XafDisplayName("Approval Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("AppStatus", Enabled = false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseOrderApp AppStatus
        {
            get { return _AppStatus; }
            set
            {
                SetPropertyValue("AppStatus", ref _AppStatus, value);
            }
        }

        private PurchaseOrderUDF _UDFs;
        [XafDisplayName("User Define Info.")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseOrderUDF UDFs
        {
            get { return _UDFs; }
            set
            {
                SetPropertyValue("UDFs", ref _UDFs, value);
            }
        }
        [Association("PurchaseOrder-Detail")]
        [XafDisplayName("Details")]
        [DevExpress.Xpo.Aggregated]
        [Appearance("PurchaseOrderDetail", Enabled = false, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
        [Appearance("PurchaseOrderDetail2", Enabled = false, Criteria = "not IsCardCodeSelected")]
        public XPCollection<PurchaseOrderDetail> PurchaseOrderDetail
        {
            get { return GetCollection<PurchaseOrderDetail>("PurchaseOrderDetail"); }
        }

        [Association("PurchaseOrder-Attachment")]
        [XafDisplayName("Attachment")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<PurchaseOrderAttachment> PurchaseOrderAttachment
        {
            get { return GetCollection<PurchaseOrderAttachment>("PurchaseOrderAttachment"); }
        }

    }
    #endregion

    #region details
    [Persistent("OPRQ1")]
    public class PurchaseOrderDetail : ClassDocumentDetail
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseOrderDetail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            UDFs = new PurchaseOrderDetailUDF(Session);
            ObjType = Session.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", DocTypeCodes.PurchaseOrder));
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
                SystemUsers user = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                if (user != null)
                {
                    this.IsViewItemPriceRole = CreateUser.CheckAccessVP(ObjType.BoCode);
                }
            }
        }
        private PurchaseOrderDetailUDF _UDFs;
        [XafDisplayName("User Define Info.")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public PurchaseOrderDetailUDF UDFs
        {
            get { return _UDFs; }
            set
            {
                SetPropertyValue("UDFs", ref _UDFs, value);
            }
        }

        private PurchaseOrder _PurchaseOrder;
        [Browsable(false)]
        [Association("PurchaseOrder-Detail")]
        public PurchaseOrder PurchaseOrder
        {
            get { return _PurchaseOrder; }
            set
            {
                SetPropertyValue("PurchaseOrder", ref _PurchaseOrder, value);
            }
        }

    }
    #endregion
}