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
    [Appearance("NewRecord", AppearanceItemType = "Action", TargetItems = "New", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("DeleteRecord", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("LinkRecord", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkRecord", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [RuleCriteria("AppStageDocDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [DefaultProperty("ApprovalStatus")]
    [XafDisplayName("Approval Document")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClassAppStatusDoc : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClassAppStatusDoc(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            ApprovalStatus = ApprovalStatus.Not_Applicable;
            ApprovedEmailCnt = 0;
        }
        private ApprovalStatus _ApprovalStatus;
        [Index(0), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        //[Appearance("RefNo", Enabled = false, Criteria = "(not IsNew and not IsRequestorChecking) or DocPassed or Accepted")]
        [Appearance("ApprovalStatus", Enabled = false)]
        public ApprovalStatus ApprovalStatus
        {
            get { return _ApprovalStatus; }
            set
            {
                SetPropertyValue("ApprovalStatus", ref _ApprovalStatus, value);
            }
        }
        private int _CurrAppStage;
        [XafDisplayName("Current App Stage")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(9), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("CurrAppStage", Enabled = false)]
        public int CurrAppStage
        {
            get { return _CurrAppStage; }
            set
            {
                SetPropertyValue("CurrAppStage", ref _CurrAppStage, value);
            }
        }

        private Approval _CurrApproval;
        [XafDisplayName("Current Approval")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("CurrApproval", Enabled = false)]
        public Approval CurrApproval
        {
            get { return _CurrApproval; }
            set
            {
                SetPropertyValue("CurrApproval", ref _CurrApproval, value);
            }
        }
        private Employee _CurrAppointedUser;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("CurrAppointedUser", Enabled = false)]
        [Index(11)]
        public Employee CurrAppointedUser
        {
            get { return _CurrAppointedUser; }
            set
            {
                SetPropertyValue("CurrAppointedUser", ref _CurrAppointedUser, value);
            }
        }

        private int _ApprovedEmailCnt;
        [XafDisplayName("Email Count")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("EmailCnt", Enabled = false)]
        public int ApprovedEmailCnt
        {
            get { return _ApprovedEmailCnt; }
            set
            {
                SetPropertyValue("ApprovedEmailCnt", ref _ApprovedEmailCnt, value);
            }
        }

    }
}