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

namespace FT_PurchasingPortal.Module.BusinessObjects
{
    [DomainComponent]
    [NonPersistent]
    public class CopyToAction : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public CopyToAction(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            ParamAction = CopyToEnum.CopyZeroQty;
        }
        // Add this property as the key member in the CustomizeTypesInfo event
        [XafDisplayName("Action")]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("ParamAction", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        public CopyToEnum ParamAction { get; set; }

        //[XafDisplayName("Important")]
        [Appearance("ActionMessage", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        [Appearance("ActionMessage2", Enabled = false, FontColor = "Red")]
        public string ActionMessage { get; set; }

        [Browsable(false)]
        public bool IsErr { get; set; }

    }




    [DomainComponent]
    [NonPersistent]
    public class DateParameters : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public DateParameters(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            ParamDate = DateTime.Today;
        }
        // Add this property as the key member in the CustomizeTypesInfo event
        [XafDisplayName("Date")]
        //[RuleRequiredField(DefaultContexts.Save)]
        [Appearance("DateString", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        public DateTime? ParamDate { get; set; }

        //[XafDisplayName("Important")]
        //[Appearance("ActionMessage", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        [Appearance("ActionMessage2", Enabled = false, FontColor = "Red")]
        public string ActionMessage { get; set; }

        [Browsable(false)]
        public bool IsErr { get; set; }

    }

    [DomainComponent]
    [NonPersistent]
    public class BooleanParameters : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public BooleanParameters(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        // Add this property as the key member in the CustomizeTypesInfo event
        [XafDisplayName("Order with Equipment?")]
        [Appearance("ParamBoolean", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        [Appearance("ParamBoolean2", Enabled = false, Criteria = "IsDisable")]
        public bool ParamBoolean { get; set; }

        [XafDisplayName("Important")]
        //[Appearance("ActionMessage", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        [Appearance("ActionMessage2", Enabled = false, FontColor = "Red")]
        public string ActionMessage { get; set; }

        [Browsable(false)]
        public bool IsDisable { get; set; }

        [Browsable(false)]
        public bool IsErr { get; set; }
    }


    [DomainComponent]
    [NonPersistent]
    public class StringParameters : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public StringParameters(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        // Add this property as the key member in the CustomizeTypesInfo event
        [XafDisplayName("Remarks")]
        [Appearance("ParamString", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        public string ParamString { get; set; }

        //[XafDisplayName("Important")]
        //[Appearance("ActionMessage", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        [Appearance("ActionMessage2", Enabled = false, FontColor = "Red")]
        public string ActionMessage { get; set; }

        [Browsable(false)]
        public bool IsErr { get; set; }

    }

    [DomainComponent]
    [NonPersistent]
    public class BudgetParameters : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public BudgetParameters(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            ParamYear = DateTime.Today.Year;
            IsYearly = true;
            IsMonthly = false;
            Amount = 0;
        }
        // Add this property as the key member in the CustomizeTypesInfo event
        private int _ParamYear;
        [XafDisplayName("Year")]
        [Appearance("ParamYear", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        public int ParamYear
        {
            get { return _ParamYear; }
            set
            {
                SetPropertyValue("ParamYear", ref _ParamYear, value);
            }
        }

        private decimal _Amount;
        [XafDisplayName("Equally Amount")]
        [Appearance("Amount", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        public decimal Amount
        {
            get { return _Amount; }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
            }
        }
        private bool _IsYearly;
        [ImmediatePostData]
        [XafDisplayName("By Year")]
        [Appearance("IsYearly", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        public bool IsYearly
        {
            get { return _IsYearly; }
            set
            {
                if (SetPropertyValue("IsYearly", ref _IsYearly, value) && !IsLoading)
                {
                    if (_IsYearly)
                        SetPropertyValue("IsMonthly", ref _IsMonthly, false);
                    else
                        SetPropertyValue("IsMonthly", ref _IsMonthly, true);
                }
            }
        }

        private bool _IsMonthly;
        [ImmediatePostData]
        [XafDisplayName("By Monthly")]
        [Appearance("IsMonthly", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        public bool IsMonthly
        {
            get { return _IsMonthly; }
            set
            {
                if (SetPropertyValue("IsMonthly", ref _IsMonthly, value) && !IsLoading)
                {
                    if (_IsMonthly)
                        SetPropertyValue("IsYearly", ref _IsYearly, false);
                    else
                        SetPropertyValue("IsYearly", ref _IsYearly, true);
                }
            }
        }

        //[Appearance("ActionMessage", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "Not IsErr")]
        [Appearance("ActionMessage2", Enabled = false, FontColor = "Red")]
        public string ActionMessage { get; set; }

        [Browsable(false)]
        public bool IsErr { get; set; }
    }

    [DomainComponent]
    [NonPersistent]
    public class ApprovalParameters : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ApprovalParameters(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        // Add this property as the key member in the CustomizeTypesInfo event
        [XafDisplayName("Approval Status")]
        [Appearance("Approved", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        public ApprovalAction AppStatus { get; set; }

        [XafDisplayName("Remarks")]
        [Appearance("ParamString", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        public string ParamString { get; set; }

        //[XafDisplayName("Important")]
        //[Appearance("ActionMessage", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsErr")]
        [Appearance("ActionMessage2", Enabled = false, FontColor = "Red")]
        public string ActionMessage { get; set; }

        [Browsable(false)]
        public bool IsErr { get; set; }

    }

}