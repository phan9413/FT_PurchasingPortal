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
    /// 20200707 force PostToDocument = document
    /// </summary>
    [DefaultClassOptions]
    [Persistent("ODTS")]
    [NavigationItem("Setup")]
    [XafDisplayName("Document Series")]
    [ImageName("ModelEditor_Action_Modules")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Appearance("HideNew", AppearanceItemType.Action, "True", TargetItems = "New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideLink", AppearanceItemType.Action, "True", TargetItems = "Link", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideUnlink", AppearanceItemType.Action, "True", TargetItems = "Unlink", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideResetViewSetting", AppearanceItemType.Action, "True", TargetItems = "ResetViewSettings", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideExport", AppearanceItemType.Action, "True", TargetItems = "Export", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    //[Appearance("HideRefresh", AppearanceItemType.Action, "True", TargetItems = "Refresh", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [RuleCriteria("DocTypeSeriesDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class DocTypeSeries : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public DocTypeSeries(Session session)
            : base(session)
        {
            PostToDocument = PostToDocument.Draft;
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
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
                Company = Session.GetObjectByKey<Company>(usr.Company.Oid);
            }
            PostToDocument = PostToDocument.Document;

            DocTypeSeriesDoc doc = new DocTypeSeriesDoc(Session);
            doc.NextDocNo = 1000001;
            Prefix = "";
            this.DetailBO.Add(doc);
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
        private string _Prefix;
        [XafDisplayName("Prefix")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(2)]
        public string Prefix
        {
            get { return _Prefix; }
            set
            {
                SetPropertyValue("Prefix", ref _Prefix, value);
            }
        }
        private DocType _DocType;
        [XafDisplayName("Document Type")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("DocType", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(10)]
        public virtual DocType DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private PostToDocument _PostToDocument;
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Index(20)]
        public PostToDocument PostToDocument
        {
            get { return _PostToDocument; }
            set
            {
                SetPropertyValue("PostToDocument", ref _PostToDocument, value);
            }
        }
        private int _SAPSeries;
        [Index(21)]
        public int SAPSeries
        {
            get { return _SAPSeries; }
            set
            {
                SetPropertyValue("SAPSeries", ref _SAPSeries, value);
            }
        }

        private Company _Company;
        [XafDisplayName("Company")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Appearance("Company", Enabled = false)]
        [Index(0)]
        public Company Company
        {
            get { return _Company; }
            set
            {
                SetPropertyValue("Company", ref _Company, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Association("DocSeries")]
        [XafDisplayName("Document Number")]
        public XPCollection<DocTypeSeriesDoc> DetailBO
        {
            get { return GetCollection<DocTypeSeriesDoc>("DetailBO"); }
        }

    }
}