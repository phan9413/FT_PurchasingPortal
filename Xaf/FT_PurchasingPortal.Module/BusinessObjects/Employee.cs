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
    [DefaultClassOptions]
    [Persistent("OEMP")]
    //[ImageName("BO_Contact")]
    [DefaultProperty("FullName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    //[Appearance("NewRecord", AppearanceItemType = "Action", TargetItems = "New", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsPosted")]
    //[Appearance("DeleteRecord", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("LinkRecord", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("UnlinkRecord", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[RuleCriteria("FTModule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    public class Employee : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Employee(Session session)
            : base(session)
        {
            session.IsObjectModifiedOnNonPersistentPropertyChange = true;
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsUpdater = false;
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            IsUpdater = false;
        }
        protected override void OnSaving()
        {
            if (!IsUpdater)
            {
                if (string.IsNullOrEmpty(FullName))
                {
                    throw new Exception("Please fill in Employee Full Name.");
                }
            }
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
            }

            if (!string.IsNullOrEmpty(TypePassword))
                SAPPassword = TypePassword;

        }
        [Browsable(false)]
        [NonPersistent]
        public bool IsUpdater
        {
            get; set;
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
        private string _SAPUserID;
        [XafDisplayName("SAP User ID"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(0)]
        public string SAPUserID
        {
            get { return _SAPUserID; }
            set
            {
                SetPropertyValue("SAPUserID", ref _SAPUserID, value);
            }
        }
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Browsable(false)]
        public string SAPPassword
        {
            get; set;
        }
        private string _TypePassword;
        [XafDisplayName("SAP User Password")]
        [NonPersistent]
        [Index(1)]
        public string TypePassword
        {
            get { return _TypePassword; }
            set
            {
                SetPropertyValue("TypePassword", ref _TypePassword, value);
            }
        }

        private vwWarehouses _WhsCode;
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [NoForeignKey]
        [Index(9)]
        public vwWarehouses WhsCode
        {
            get
            { return _WhsCode; }
            set
            {
                SetPropertyValue("WhsCode", ref _WhsCode, value);
            }
        }

        private string _FullName;
        [XafDisplayName("Full Name")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(10)]
        public string FullName
        {
            get { return _FullName; }
            set
            {
                SetPropertyValue("FullName", ref _FullName, value);
            }
        }
        private string _UserEmail;
        [XafDisplayName("Email")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(20)]
        public string UserEmail
        {
            get { return _UserEmail; }
            set
            {
                SetPropertyValue("UserEmail", ref _UserEmail, value);
            }
        }
        private string _ICNo;
        [XafDisplayName("IC No")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(21)]
        public string ICNo
        {
            get { return _ICNo; }
            set
            {
                SetPropertyValue("ICNo", ref _ICNo, value);
            }
        }
        private Departments _Department;
        [XafDisplayName("Department")]
        [DataSourceCriteria("Company.BoCode = '@This.SystemUser.Company.BoCode' and IsActive")]
        [NoForeignKey]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(24)]
        public Departments Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }
        private Divisions _Division;
        [XafDisplayName("Division")]
        [DataSourceCriteria("Company.BoCode = '@This.SystemUser.Company.BoCode' and IsActive")]
        [NoForeignKey]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(25)]
        public Divisions Division
        {
            get { return _Division; }
            set
            {
                SetPropertyValue("Division", ref _Division, value);
            }
        }

        private Positions _Position;
        [XafDisplayName("Position")]
        [DataSourceCriteria("IsActive")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Association("Positions-Employee")]
        [Index(30)]
        public Positions Position
        {
            get { return _Position; }
            set
            {
                SetPropertyValue("Position", ref _Position, value);
            }
        }

        private SystemUsers _SystemUser;
        //[XafDisplayName("System User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("SystemUser", Enabled = false)]
        public SystemUsers SystemUser
        {
            get { return _SystemUser; }
            set
            {
                SetPropertyValue("SystemUser", ref _SystemUser, value);
            }
        }
        [Browsable(false)]
        [Association("ApprovalApproveUser")]
        [XafDisplayName("Approve User")]
        [Appearance("MyNotification", Enabled = false)]
        public XPCollection<Approval> ApproveApproval
        {
            get { return GetCollection<Approval>("ApproveApproval"); }
        }

        [Browsable(false)]
        [Association("Employee-MyNotification")]
        [Appearance("MyNotification", Enabled = false)]
        public XPCollection<MyNotification> MyNotification
        {
            get { return GetCollection<MyNotification>("MyNotification"); }
        }
    }
}