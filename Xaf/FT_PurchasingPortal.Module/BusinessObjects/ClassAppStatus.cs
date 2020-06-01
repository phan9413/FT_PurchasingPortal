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
    //[RuleCriteria("AppStageStatusDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [DefaultProperty("StatusInfo")]
    [XafDisplayName("Approval Stage Status")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClassAppStatus : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClassAppStatus(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        [NonPersistent]
        public string StatusInfo
        {
            get
            {
                string temp = "";
                if (CreateUser != null)
                {
                    temp = string.Format("{0:yyyy/MM/dd HH:mm:ss}", CreateDate);
                    temp += " [" + CreateUser.UserName + "]";
                }
                return temp;
            }
        }
        private Approval _Approval;
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("Approval", Enabled = false)]
        public Approval Approval
        {
            get { return _Approval; }
            set
            {
                SetPropertyValue("Approval", ref _Approval, value);
            }
        }
        private int _AppStage;
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("AppStage", Enabled = false)]
        public int AppStage
        {
            get { return _AppStage; }
            set
            {
                SetPropertyValue("AppStage", ref _AppStage, value);
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

        private ApprovalStatus _ApprovalStatus;
        [XafDisplayName("Approval Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("ApprovalStatus", Enabled = false)]
        public ApprovalStatus ApprovalStatus
        {
            get { return _ApprovalStatus; }
            set
            {
                SetPropertyValue("ApprovalStatus", ref _ApprovalStatus, value);
            }
        }
        private string _ApprovalRemarks;
        [XafDisplayName("Approval Remarks")]
        [Appearance("ApprovalRemarks", Enabled = false)]
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public string ApprovalRemarks
        {
            get { return _ApprovalRemarks; }
            set
            {
                SetPropertyValue("ApprovalRemarks", ref _ApprovalRemarks, value);
            }
        }

        //private ClaimTrxs _ClaimTrx;
        //[Association("ClaimTrxs-ClaimTrxAppStages")]
        //[Index(99), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        //[Appearance("ClaimTrx", Enabled = false)]
        //public ClaimTrxs ClaimTrx
        //{
        //    get { return _ClaimTrx; }
        //    set { SetPropertyValue("ClaimTrx", ref _ClaimTrx, value); }
        //}


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
                    if (!GeneralValues.IsNetCore)
                        CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    else
                        CreateUser = Session.FindObject<SystemUsers>(CriteriaOperator.Parse("UserName=?", GeneralValues.NetCoreUserName));
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