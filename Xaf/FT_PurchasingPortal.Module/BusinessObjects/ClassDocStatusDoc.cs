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
    [RuleCriteria("DocumentStatusDocDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [DefaultProperty("CurrDocStatus")]
    [XafDisplayName("Document Status Document")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClassDocStatusDoc : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClassDocStatusDoc(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            CurrDocStatus = DocStatus.Draft;
            IsSAPPosted = false;
        }

        private DocStatus _CurrDocStatus;
        [XafDisplayName("Document Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("DocStatus", Enabled = false)]
        public DocStatus CurrDocStatus
        {
            get { return _CurrDocStatus; }
            set
            {
                SetPropertyValue("CurrDocStatus", ref _CurrDocStatus, value);
            }
        }
        private bool _IsSAPPosted;
        [Index(30), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("IsSAPPosted", Enabled = false)]
        [XafDisplayName("Posted")]
        public bool IsSAPPosted
        {
            get { return _IsSAPPosted; }
            set
            {
                SetPropertyValue("IsSAPPosted", ref _IsSAPPosted, value);
            }
        }
        private string _SAPPostCancelRemarks;
        [Index(31), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("SAPPostCancelRemarks", Enabled = false)]
        [XafDisplayName("Post Error Remarks")]
        public string SAPPostCancelRemarks
        {
            get { return _SAPPostCancelRemarks; }
            set
            {
                if (!IsLoading)
                {
                    if (value != null)
                        if (value.Length > 100)
                            value = value.Substring(0, 100);
                }
                SetPropertyValue("SAPPostCancelRemarks", ref _SAPPostCancelRemarks, value);
            }
        }

    }
}