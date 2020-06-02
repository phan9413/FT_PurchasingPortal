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

#region update log
// YKW - 20200212 - comments

#endregion

namespace FT_PurchasingPortal.Module.BusinessObjects
{
    [DefaultClassOptions, ImageName("Action_Chart_Printing_Preview")]
    [Persistent("OCRR")]
    [NavigationItem("Reports")]
    [XafDisplayName("Crystal Reports")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class CrReport : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public CrReport(Session session)
            : base(session)
        {
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
                Company = Session.FindObject<Company>(new BinaryOperator("BoCode", usr.Company.BoCode, BinaryOperatorType.Equal));
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
        private Company _Company;
        [Index(0)]
        public Company Company
        {
            get { return _Company; }
            set
            {
                SetPropertyValue("Company", ref _Company, value);
            }
        }

        private string _ReportName;
        [Index(10)]
        [RuleUniqueValue]
        public string ReportName
        {
            get { return _ReportName; }
            set { SetPropertyValue("ReportName", ref _ReportName, value); }
        }

        private string _ReportPathFile;
        [Index(11), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string ReportPathFile
        {
            get { return _ReportPathFile; }
            set { SetPropertyValue("ReportPathFile", ref _ReportPathFile, value); }
        }

        [Association("CrReport-Param")]
        public XPCollection<CrReportParam> CrReportParam
        {
            get { return GetCollection<CrReportParam>("CrReportParam"); }
        }

    }
}