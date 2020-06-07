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
    [Persistent("OCMP")]
    //[ImageName("BO_Contact")]
    [NavigationItem("Setup")]
    [XafDisplayName("Company")]
    [DefaultProperty("BoName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    [Appearance("NewRecord", AppearanceItemType = "Action", TargetItems = "New", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("DeleteRecord", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("LinkRecord", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkRecord", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [RuleCriteria("CompanyDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    //[RuleCriteria("CompanySaveRule", DefaultContexts.Save, "1=0", "Cannot Save.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class Company : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Company(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).

            BoCode = "";
            BoName = "";
            ApprovalBy = ApprovalBy.User;
            AppType = ApprovalType.Document;

            //DevExpress.Xpo.Metadata.XPClassInfo myClass = Session.GetClassInfo(typeof(DocTypes));
            //CriteriaOperator myCriteria = new BinaryOperator("IsActive", true);
            //SortingCollection sortProps = new SortingCollection(null);
            //sortProps.Add(new SortProperty("BoCode", DevExpress.Xpo.DB.SortingDirection.Ascending));

            //var LDocType = Session.GetObjects(myClass, myCriteria, sortProps, 0, false, true);

            ////XPCollection<DocTypes> LDocType = new XPCollection<DocTypes>();
            ////LDocType.Load();

            //int cnt = 0;
            //foreach (var dtl in LDocType)
            //{
            //    cnt++;
            //    CompanyDocs obj = new CompanyDocs(Session);
            //    obj.DocType = Session.FindObject<DocTypes>(new BinaryOperator("Oid", ((DocTypes)dtl).Oid, BinaryOperatorType.Equal));
            //    obj.NextDocNo = (cnt * 1000000) + 1;
            //    this.CompanyDoc.Add(obj);
            //}
            FTModule = new FTModule(Session);
            FTSAPConn = new FTSAPConn(Session);
            FTCrystalReportConn = new FTCrystalReportConn(Session);

            WhsCode = GeneralValues.defwh;
            LocalCurreny = GeneralValues.deflocalcurrency;
            InputTax = GeneralValues.definputtax;
            OutputTax = GeneralValues.defoutputtax;
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        //OR
                        //&& !(Session.ObjectLayer is DevExpress.ExpressApp.Security.ClientServer.SecuredSessionObjectLayer)
                        && Session.IsNewObject(this))
            {
                //DateTime DocTime = DateTime.Now;
                //OutletDocs outletdoc = Session.FindObject<OutletDocs>(CriteriaOperator.Parse("Outlet.Oid=? and DocType=?", Outlet.Oid, DocumentType.SalesOrder));
                //SequentialNumber = outletdoc.NextSeq;
                //Prefix = outletdoc.Prefix;

                //outletdoc.NextSeq++;

                //outletdoc.Save();

            }
        }

        protected override void OnSaved()
        {
            base.OnSaved();
            this.Reload();
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

        //private bool _IsPassAccept;
        //[XafDisplayName("Pass = Accept?")]
        ////[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        ////[RuleRequiredField(DefaultContexts.Save)]
        //[Index(3)]
        //public bool IsPassAccept
        //{
        //    get { return _IsPassAccept; }
        //    set
        //    {
        //        SetPropertyValue("IsPassAccept", ref _IsPassAccept, value);
        //    }
        //}


        private ApprovalBy _ApprovalBy;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Default Approval By")]
        [Index(10)]
        public ApprovalBy ApprovalBy
        {
            get { return _ApprovalBy; }
            set
            {
                SetPropertyValue("ApprovalBy", ref _ApprovalBy, value);
            }
        }

        private ApprovalType _AppType;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Default Approval Type")]
        [Index(11)]
        public ApprovalType AppType
        {
            get { return _AppType; }
            set
            {
                SetPropertyValue("AppType", ref _AppType, value);
            }
        }

        private string _LocalCurreny;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [XafDisplayName("Local Currency")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(20)]
        public string LocalCurreny
        {
            get { return _LocalCurreny; }
            set
            {
                SetPropertyValue("LocalCurreny", ref _LocalCurreny, value);
            }
        }


        private string _WhsCode;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [XafDisplayName("Default Warehouse")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(21)]
        public string WhsCode
        {
            get { return _WhsCode; }
            set
            {
                SetPropertyValue("WhsCode", ref _WhsCode, value);
            }
        }


        private string _InputTax;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [XafDisplayName("Default Input Tax")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(22)]
        public string InputTax
        {
            get { return _InputTax; }
            set
            {
                SetPropertyValue("InputTax", ref _InputTax, value);
            }
        }


        private string _OutputTax;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [XafDisplayName("Default Output Tax")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(23)]
        public string OutputTax
        {
            get { return _OutputTax; }
            set
            {
                SetPropertyValue("OutputTax", ref _OutputTax, value);
            }
        }



        private int _DepartmentToDimension;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [XafDisplayName("Department to Dimension"), ToolTip("0 value = do not post")]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(24)]
        public int DepartmentToDimension
        {
            get { return _DepartmentToDimension; }
            set
            {
                SetPropertyValue("DepartmentToDimension", ref _DepartmentToDimension, value);
            }
        }


        private int _DivisionToDimension;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [XafDisplayName("Division to Dimension"), ToolTip("0 value = do not post")]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(25)]
        public int DivisionToDimension
        {
            get { return _DivisionToDimension; }
            set
            {
                SetPropertyValue("DivisionToDimension", ref _DivisionToDimension, value);
            }
        }


        private FTModule _FTModule;
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [Index(100)]
        public FTModule FTModule
        {
            get { return _FTModule; }
            set
            {
                SetPropertyValue("FTModule", ref _FTModule, value);
            }
        }


        private FTSAPConn _FTSAPConn;
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [Index(101)]
        public FTSAPConn FTSAPConn
        {
            get { return _FTSAPConn; }
            set
            {
                SetPropertyValue("FTSAPConn", ref _FTSAPConn, value);
            }
        }


        private FTCrystalReportConn _FTCrystalReportConn;
        [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never)]
        [Index(102)]
        public FTCrystalReportConn FTCrystalReportConn
        {
            get { return _FTCrystalReportConn; }
            set
            {
                SetPropertyValue("FTCrystalReportConn", ref _FTCrystalReportConn, value);
            }
        }


        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }


    }


}