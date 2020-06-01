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
    //[RuleCriteria("AppStageDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [DefaultProperty("Approval")]
    [XafDisplayName("Approval Stage")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClassAppStage : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClassAppStage(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            EmailCnt = 0;
        }

        private Approval _Approval;
        [XafDisplayName("Approval")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
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

        private int _EmailCnt;
        [XafDisplayName("Email Count")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(20), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("EmailCnt", Enabled = false)]
        public int EmailCnt
        {
            get { return _EmailCnt; }
            set
            {
                SetPropertyValue("EmailCnt", ref _EmailCnt, value);
            }
        }
        private DateTime? _EmailDate;
        [XafDisplayName("Email Date")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(21), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("EmailDate", Enabled = false)]
        public DateTime? EmailDate
        {
            get { return _EmailDate; }
            set
            {
                SetPropertyValue("EmailDate", ref _EmailDate, value);
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


    }
}