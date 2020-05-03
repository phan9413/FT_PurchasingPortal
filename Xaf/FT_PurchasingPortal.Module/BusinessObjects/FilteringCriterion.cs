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
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.ConditionalAppearance;

#region update log
// YKW - 20200212 - comments

#endregion

namespace FT_PurchasingPortal.Module.BusinessObjects
{
    [DefaultClassOptions, ImageName("Action_Filter")]
    [Persistent("OFTC")]
    [XafDisplayName("Root ListView Filtering Criterion")]
    [XafDefaultProperty("Description")]
    [RuleCriteria("FilteringCriterionSaveRule", DefaultContexts.Save, "IsDescriptionValid", "Description is not valid.")]
    public class FilteringCriterion : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public FilteringCriterion(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            Description = "-";
        }
        [Browsable(false)]
        public bool IsDescriptionValid
        {
            get
            {
                bool rtn = true;
                if (string.IsNullOrEmpty(Description))
                    rtn = false;
                else
                    if (Description.ToUpper() == "DEFAULT")
                        rtn = false;

                return rtn;
            }
        }
        [RuleRequiredField(DefaultContexts.Save)]
        [ToolTip("Default as -")]
        public string Description
        {
            get { return GetPropertyValue<string>("Description"); }
            set { SetPropertyValue<string>("Description", value); }
        }
        [ValueConverter(typeof(TypeToStringConverter)), ImmediatePostData]
        [RuleRequiredField(DefaultContexts.Save)]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type ObjectType
        {
            get { return GetPropertyValue<Type>("ObjectType"); }
            set
            {
                SetPropertyValue<Type>("ObjectType", value);
                Criterion = String.Empty;
            }
        }
        [CriteriaOptions("ObjectType"), Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
        public string Criterion
        {
            get { return GetPropertyValue<string>("Criterion"); }
            set { SetPropertyValue<string>("Criterion", value); }
        }
        public string Remarks
        {
            get { return GetPropertyValue<string>("Remarks"); }
            set { SetPropertyValue<string>("Remarks", value); }
        }

        [Association("Filter-Role")]
        public XPCollection<FilteringCriterionRole> Roles
        {
            get { return GetCollection<FilteringCriterionRole>("Roles"); }
        }
    }


    [Appearance("LinkRecord", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkRecord", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Persistent("OFTC1")]
    public class FilteringCriterionRole : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public FilteringCriterionRole(Session session)
            : base(session)
        {
        }

        [Browsable(false)]
        [Association("Filter-Role")]
        public FilteringCriterion Filtering
        { get; set; }

        public DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyRole FilterRole
        { get; set; }

        public string Description
        {
            get { return GetPropertyValue<string>("Description"); }
            set { SetPropertyValue<string>("Description", value); }
        }

    }


}