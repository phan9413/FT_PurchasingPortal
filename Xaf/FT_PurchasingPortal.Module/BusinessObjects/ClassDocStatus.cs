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
    [RuleCriteria("DocumentStatusDeleteRule", DefaultContexts.Delete, "1=0", "Cannot Delete.")]
    [DefaultProperty("StatusInfo")]
    [XafDisplayName("Document Status")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    public class ClassDocStatus : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public ClassDocStatus(Session session)
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
        private DocStatus _DocStatus;
        [XafDisplayName("Document Status")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [Appearance("DocStatus", Enabled = false)]
        public DocStatus DocStatus
        {
            get { return _DocStatus; }
            set
            {
                SetPropertyValue("DocStatus", ref _DocStatus, value);
            }
        }

        private string _DocStatusRemarks;
        [XafDisplayName("Remarks")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(11), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public string DocStatusRemarks
        {
            get { return _DocStatusRemarks; }
            set
            {
                SetPropertyValue("DocStatusRemarks", ref _DocStatusRemarks, value);
            }
        }

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
        [Index(301), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set
            {
                SetPropertyValue("CreateDate", ref _CreateDate, value);
            }
        }

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
            }
        }

    }
}