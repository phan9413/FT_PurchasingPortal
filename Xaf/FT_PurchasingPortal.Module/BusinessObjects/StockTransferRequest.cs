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
    [NavigationItem("Inventory")]
    [Persistent("OSTF")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected)
    [Appearance("SubmitDocRecord", AppearanceItemType = "Action", TargetItems = "SubmitDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Closed)
    [Appearance("PostDocRecord", AppearanceItemType = "Action", TargetItems = "PostDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (5))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted || selectedObject.DocStatus.CurrDocStatus == DocStatus.PostedCancel)
    [Appearance("CloseDocRecord", AppearanceItemType = "Action", TargetItems = "CloseDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (4, 7))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected)
    [Appearance("CancelDocRecord", AppearanceItemType = "Action", TargetItems = "CancelDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited || selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted)
    [Appearance("RejectDocRecord", AppearanceItemType = "Action", TargetItems = "RejectDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (3, 4))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected || selectedObject.DocStatus.CurrDocStatus == DocStatus.Accepted)
    [Appearance("SaveDocRecord", AppearanceItemType = "Action", TargetItems = "Save", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected)
    [Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (0, 2))")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited && AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
    [Appearance("ApprovalDocRecord", AppearanceItemType = "Action", TargetItems = "ApprovalDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus = 3 and AppStatus.ApprovalStatus = 2)")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Submited && AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
    [Appearance("ChangeAppUserRecord", AppearanceItemType = "Action", TargetItems = "ChangeAppUser", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus = 3 and AppStatus.ApprovalStatus = 2)")]
    //if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Closed)
    [Appearance("ReOpenDocRecord", AppearanceItemType = "Action", TargetItems = "ReOpenDoc", Context = "DetailView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not (DocStatus.CurrDocStatus in (5))")]

    [RuleCriteria("StockTransferRequestSaveRule", DefaultContexts.Save, "IsWarehouseValid", "Warehouses are not valid.")]
    public class StockTransferRequest : ClassStockTransferDocument
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public StockTransferRequest(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            AppStatus = new StockTransferRequestApp(Session);
            DocStatus = new StockTransferRequestDoc(Session);
            UDFs = new StockTransferRequestUDF(Session);
            DocType = Session.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", DocTypeCodes.StockTransferRequest));
            DocTypeSeries = Session.FindObject<DocTypeSeries>(CriteriaOperator.Parse("DocType.Oid=? and Company.Oid=?", DocType.Oid, Company.Oid));
            if (CreateUser != null)
            {
                this.IsViewItemPriceRole = CreateUser.CheckAccessVP(DocType.BoCode);
            }
            if (CreateUser.Employee.WhsCode != null)
            {
                Filler = Session.FindObject<vwWarehouses>(CriteriaOperator.Parse("CompanyCode=? and WhsCode=?", Company.BoCode, CreateUser.Employee.WhsCode.WhsCode));
                ToWhsCode = Session.FindObject<vwWarehouses>(CriteriaOperator.Parse("CompanyCode=? and WhsCode=?", Company.BoCode, CreateUser.Employee.WhsCode.WhsCode));
            }
            else if(!string.IsNullOrEmpty(Company.WhsCode))
            {
                Filler = Session.FindObject<vwWarehouses>(CriteriaOperator.Parse("CompanyCode=? and WhsCode=?", Company.BoCode, Company.WhsCode));
                ToWhsCode = Session.FindObject<vwWarehouses>(CriteriaOperator.Parse("CompanyCode=? and WhsCode=?", Company.BoCode, Company.WhsCode));
            }
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
                        this.IsViewItemPriceRole = CreateUser.CheckAccessVP(DocType.BoCode);
                    }
                }
            }
        }
        [Browsable(false)]
        public bool IsWarehouseValid
        {
            get
            {
                bool rtn = false;
                if (Filler is null || ToWhsCode is null)
                { }
                else if (Filler != ToWhsCode)
                    rtn = true;
                return rtn;
            }
        }
        private StockTransferRequestDoc _DocStatus;
        [XafDisplayName("Document Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(1), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("DocStatus", Enabled = false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public StockTransferRequestDoc DocStatus
        {
            get { return _DocStatus; }
            set
            {
                SetPropertyValue("DocStatus", ref _DocStatus, value);
            }
        }
        private StockTransferRequestApp _AppStatus;
        [XafDisplayName("Approval Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(2), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("AppStatus", Enabled = false)]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public StockTransferRequestApp AppStatus
        {
            get { return _AppStatus; }
            set
            {
                SetPropertyValue("AppStatus", ref _AppStatus, value);
            }
        }

        private StockTransferRequestUDF _UDFs;
        [XafDisplayName("User Define Info.")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public StockTransferRequestUDF UDFs
        {
            get { return _UDFs; }
            set
            {
                SetPropertyValue("UDFs", ref _UDFs, value);
            }
        }
        [ImmediatePostData]
        [Association("StockTransferRequest-Detail")]
        [XafDisplayName("Details")]
        [DevExpress.Xpo.Aggregated]
        [Appearance("StockTransferRequestDetail", Enabled = false, Criteria = "not (DocStatus.CurrDocStatus in (0, 2, 4))")]
        public XPCollection<StockTransferRequestDetail> StockTransferRequestDetail
        {
            get { return GetCollection<StockTransferRequestDetail>("StockTransferRequestDetail"); }
        }


    }
    #endregion

    #region details
    [Persistent("OSTF1")]
    [RuleCriteria("StockTransferRequestDetailSaveRule", DefaultContexts.Save, "IsWarehouseValid", "Warehouses are not valid.")]
    public class StockTransferRequestDetail : ClassStockTransferDocumentDetail
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public StockTransferRequestDetail(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            UDFs = new StockTransferRequestDetailUDF(Session);
            ObjType = Session.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", DocTypeCodes.StockTransferRequest));
            if (CreateUser != null)
            {
                this.IsViewItemPriceRole = CreateUser.CheckAccessVP(ObjType.BoCode);
            }
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
        [Browsable(false)]
        public bool IsWarehouseValid
        {
            get
            {
                bool rtn = false;
                if (FromWhsCod is null && WhsCode is null)
                {
                    rtn = true;
                }
                else if (FromWhsCod is null || WhsCode is null)
                {
                    rtn = true;
                }
                else
                {
                    if (FromWhsCod != WhsCode)
                        rtn = true;
                }
                return rtn;
            }
        }
        private StockTransferRequestDetailUDF _UDFs;
        [XafDisplayName("User Define Info.")]
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        public StockTransferRequestDetailUDF UDFs
        {
            get { return _UDFs; }
            set
            {
                SetPropertyValue("UDFs", ref _UDFs, value);
            }
        }

        private StockTransferRequest _StockTransferRequest;
        [VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(true)]
        [Appearance("StockTransferRequest", Enabled = false)]
        [Association("StockTransferRequest-Detail")]
        public StockTransferRequest StockTransferRequest
        {
            get { return _StockTransferRequest; }
            set
            {
                SetPropertyValue("StockTransferRequest", ref _StockTransferRequest, value);
            }
        }

    }
    #endregion
}