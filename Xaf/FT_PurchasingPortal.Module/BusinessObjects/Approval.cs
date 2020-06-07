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
using System.Drawing;

#region update log
// YKW - 20200212 - comments

#endregion

namespace FT_PurchasingPortal.Module.BusinessObjects
{
    [DefaultClassOptions]
    [Persistent("OAPP")]
    //[ImageName("BO_Contact")]
    [NavigationItem("Approval")]
    [DefaultProperty("BoName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Appearance("NewRecord", AppearanceItemType = "Action", TargetItems = "New", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("DeleteRecord", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("LinkRecord", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("UnlinkRecord", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [RuleCriteria("ApprovalsDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [RuleCriteria("ApprovalsSaveRule", DefaultContexts.Save, "IsDocAmountValid", "Doc Amount is invalid.")]
    [RuleCriteria("ApprovalsSaveRule2", DefaultContexts.Save, "IsBudgetValid", "Budget is invalid.")]
    //[RuleCriteria("ApprovalsSaveRule3", DefaultContexts.Save, "IsTriggerUserValid", "Email User is invalid.")]
    [RuleCriteria("ApprovalsSaveRule4", DefaultContexts.Save, "IsApprovalUserValid", "Approval User is invalid.")]

    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Approval : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Approval(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).

            BoCode = "";
            BoName = "";
            IsOverride = false;
            IsActive = false;
            IsReject = false;
            ApprovalCnt = 1;

            DocAmount = 0;
            SystemUsers usr = null;
            if (!GeneralValues.IsNetCore)
            {
                usr = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            }
            else
            {
                usr = Session.FindObject<SystemUsers>(CriteriaOperator.Parse("UserName=?", GeneralValues.NetCoreUserName));
            }
            if (usr != null && usr.Company != null)
            {
                Company = Session.FindObject<Company>(new BinaryOperator("BoCode", usr.Company.BoCode, BinaryOperatorType.Equal));
                AppType = Company.AppType;
                ApprovalBy = Company.ApprovalBy;
                DocCur = Session.FindObject<vwCurrency>(CriteriaOperator.Parse("CompanyCode=? and CurrCode=?", Company.BoCode, Company.LocalCurreny));
            }
        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            //DevExpress.BarCodes.BarCode barCode = new DevExpress.BarCodes.BarCode();
            //barCode.Symbology = DevExpress.BarCodes.Symbology.QRCode;
            //barCode.CodeText = "http://www.devexpress.com";
            //barCode.BackColor = Color.White;
            //barCode.ForeColor = Color.Black;
            //barCode.RotationAngle = 0;
            //barCode.CodeBinaryData = Encoding.Default.GetBytes(barCode.CodeText);
            //barCode.Options.QRCode.CompactionMode = DevExpress.BarCodes.QRCodeCompactionMode.Byte;
            //barCode.Options.QRCode.ErrorLevel = DevExpress.BarCodes.QRCodeErrorLevel.Q;
            //barCode.Options.QRCode.ShowCodeText = false;
            //barCode.DpiX = 72;
            //barCode.DpiY = 72;
            //barCode.Module = 2f;

            //Bitmap bmp = barCode.BarCodeImage;
            //ImageConverter converter = new ImageConverter();
            //PhotoData = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
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

        private string _BoCode;
        [XafDisplayName("Code"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleUniqueValue]
        [Appearance("BoCode", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(0)]
        public string BoCode
        {
            get { return _BoCode; }
            set
            {                
                SetPropertyValue("BoCode", ref _BoCode, value);
            }
        }

        private string _BoName;
        [XafDisplayName("Name"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(1)]
        public string BoName
        {
            get { return _BoName; }
            set
            {
                SetPropertyValue("BoName", ref _BoName, value);
            }
        }

        [PersistentAlias("concat(BoCode, '::', BoName)")]
        [Index(2), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public string BoFullName
        {
            get { return EvaluateAlias("BoFullName").ToString(); }
        }

        private DocType _DocType;
        [XafDisplayName("Document Type"), ToolTip("Select Document")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Appearance("DocType", Enabled = false, Criteria = "not IsNew")]
        [DataSourceCriteria("IsReqApp")]
        [Index(3)]
        public DocType DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private ApprovalType _AppType;
        [ImmediatePostData]
        [XafDisplayName("Approval Type"), ToolTip("Select Type")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("AppType", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(4)]
        public ApprovalType AppType
        {
            get { return _AppType; }
            set
            {
                if (SetPropertyValue("AppType", ref _AppType, value))
                {
                    if (!IsLoading)
                    {
                        SetPropertyValue("DocAmount", ref _DocAmount, 0);
                    }
                }
            }
        }
        private int _ApprovalCnt;
        [XafDisplayName("Number of Approval"), ToolTip("Enter Number")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [Appearance("ApprovalCnt", Enabled = false, Criteria = "IsAppointed_User")]
        [Index(5)]
        public int ApprovalCnt
        {
            get { return _ApprovalCnt; }
            set
            {
                SetPropertyValue("ApprovalCnt", ref _ApprovalCnt, value);
            }
        }

        private string _ApprovalLevel;
        [XafDisplayName("Approval Level")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        [RuleUniqueValue]
        [Index(6)]
        public string ApprovalLevel
        {
            get { return _ApprovalLevel; }
            set
            {
                SetPropertyValue("ApprovalLevel", ref _ApprovalLevel, value);
            }
        }
        private vwCurrency _DocCur;
        [Index(7), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [NoForeignKey]
        [DataSourceCriteria("CompanyCode = '@This.Company.BoCode' and IsActive")]
        [XafDisplayName("Currency Code")]
        [RuleRequiredField(DefaultContexts.Save)]
        //[Appearance("DocCur", Enabled = false, Criteria = "not IsAllowedDocAmount")]
        public vwCurrency DocCur
        {
            get { return _DocCur; }
            set
            {
                SetPropertyValue("DocCur", ref _DocCur, value);
            }
        }
        private decimal _DocAmount;
        [XafDisplayName("Document Amount"), ToolTip("Enter Number")]
        [Appearance("DocAmount", Enabled = false, Criteria = "not IsAllowedDocAmount")]
        [Index(8)]
        public decimal DocAmount
        {
            get { return _DocAmount; }
            set
            {
                SetPropertyValue("DocAmount", ref _DocAmount, value);
            }
        }
        [Browsable(false)]
        public bool IsAllowedDocAmount
        {
            get
            {
                if (AppType == ApprovalType.Document)
                    return true;
                return false;
            }
        }

        private bool _IsActive;
        [XafDisplayName("Active")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(10)]
        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                SetPropertyValue("IsActive", ref _IsActive, value);
            }
        }
        private bool _IsOverride;
        [XafDisplayName("Override Following Level?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(20)]
        public bool IsOverride
        {
            get { return _IsOverride; }
            set
            {
                SetPropertyValue("IsOverride", ref _IsOverride, value);
            }
        }

        private bool _IsReject;
        [XafDisplayName("Reject?")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(30)]
        public bool IsReject
        {
            get { return _IsReject; }
            set
            {
                SetPropertyValue("IsReject", ref _IsReject, value);
            }
        }

        private ApprovalBy _ApprovalBy;
        [ImmediatePostData]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(50)]
        public ApprovalBy ApprovalBy
        {
            get { return _ApprovalBy; }
            set
            {
                if (SetPropertyValue("ApprovalBy", ref _ApprovalBy, value))
                {
                    if (!IsLoading)
                    {
                        //SetPropertyValue("Department", ref _Department, null);
                        SetPropertyValue("AppointedUser", ref _AppointedUser, null);
                        SetPropertyValue("Division", ref _Division, null);
                        SetPropertyValue("ApprovalCnt", ref _ApprovalCnt, 1);
                    }
                }
            }
        }
        private Employee _AppointedUser;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("AppointedUser", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsAppointed_User")]
        [Index(51)]
        public Employee AppointedUser
        {
            get { return _AppointedUser; }
            set
            {
                SetPropertyValue("AppointedUser", ref _AppointedUser, value);
            }
        }

        private Divisions _Division;
        [DataSourceCriteria("Company.BoCode = '@This.Company.BoCode' and IsActive")]
        [XafDisplayName("Approval Pos. by Division.?")]
        [NoForeignKey]
        [Appearance("Division", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsPosition")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(52)]
        public Divisions Division
        {
            get { return _Division; }
            set
            {
                SetPropertyValue("Division", ref _Division, value);
            }
        }

        //[Size(SizeAttribute.Unlimited), ImageEditor]
        //public Byte[] PhotoData { get; set; }


        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Browsable(false)]
        public bool IsBudget
        {
            get
            {
                if (AppType == ApprovalType.Budget)
                    return true;
                return false;
            }
        }

        [Browsable(false)]
        public bool IsUser
        {
            get
            {
                if (ApprovalBy == ApprovalBy.User)
                    return true;
                return false;
            }
        }
        [Browsable(false)]
        public bool IsPosition
        {
            get
            {
                if (ApprovalBy == ApprovalBy.Position)
                    return true;
                return false;
            }
        }
        [Browsable(false)]
        public bool IsAppointed_User
        {
            get
            {
                if (ApprovalBy == ApprovalBy.Appointed_User)
                    return true;
                return false;
            }
        }

        [Browsable(false)]
        public bool IsDocAmountValid
        {
            get
            {
                if (AppType == ApprovalType.Budget && DocAmount == 0)
                {
                    if (ApprovalBy == ApprovalBy.Appointed_User && AppointedUser != null)
                        return true;
                    else
                    {
                        return true;
                    }
                }
                if (AppType == ApprovalType.Document && DocAmount != 0)
                {
                    if (ApprovalBy == ApprovalBy.Appointed_User && AppointedUser != null)
                        return true;
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        [Browsable(false)]
        public bool IsBudgetValid
        {
            get
            {
                if (BudgetMaster.Where(pp => pp.Company.Oid != Company.Oid).Count() > 0)
                    return false;
                return true;
            }
        }
        //[Browsable(false)]
        //public bool IsTriggerUserValid
        //{
        //    get
        //    {
        //        if (TriggerUser.Where(pp => pp.Company.Oid != Company.Oid).Count() > 0)
        //            return false;
        //        return true;
        //    }
        //}
        [Browsable(false)]
        public bool IsApprovalUserValid
        {
            get
            {
                if (ApproveUser.Where(pp => pp.SystemUser.Company.Oid != Company.Oid).Count() > 0)
                    return false;
                return true;
            }
        }

        //[Association("ApprovalTriggerUser")]
        //[XafDisplayName("Email User")]
        //[Appearance("TriggerUser", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsUser or IsTriggerCreator")]
        //public XPCollection<SystemUsers> TriggerUser
        //{
        //    get { return GetCollection<SystemUsers>("TriggerUser"); }
        //}
        [Association("ApprovalApproveUser")]
        [XafDisplayName("Approval User")]
        [Appearance("ApprovalUser", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsUser")]
        public XPCollection<Employee> ApproveUser
        {
            get { return GetCollection<Employee>("ApproveUser"); }
        }
        //[Association("ApprovalTriggerPositions")]
        //[XafDisplayName("Email Position")]
        //[Appearance("TriggerPosition", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsPosition or IsTriggerCreator")]
        //public XPCollection<Positions> TriggerPosition
        //{
        //    get { return GetCollection<Positions>("TriggerPosition"); }
        //}
        [Association("ApprovalApprovePositions")]
        [XafDisplayName("Approval Position")]
        [Appearance("ApprovalPosition", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsPosition")]
        public XPCollection<Positions> ApprovePosition
        {
            get { return GetCollection<Positions>("ApprovePosition"); }
        }

        [Association("ApprovalBudgetMasters")]
        [XafDisplayName("Budget Master")]
        [Appearance("BudgetMaster", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "not IsBudget")]
        public XPCollection<BudgetMaster> BudgetMaster
        {
            get { return GetCollection<BudgetMaster>("BudgetMaster"); }
        }
    }
}