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
    //[DefaultClassOptions]
    [Persistent("OSAC")]
    //[ImageName("BO_Contact")]
    [DefaultProperty("BoFullName")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    [Appearance("NewRecord", AppearanceItemType = "Action", TargetItems = "New", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    //[Appearance("EditRecord", AppearanceItemType = "Action", TargetItems = "SwitchToEditMode;Edit", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Criteria = "IsPosted")]
    [Appearance("DeleteRecord", AppearanceItemType = "Action", TargetItems = "Delete", Context = "Any", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("LinkRecord", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkRecord", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    public class FTSAPConn : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public FTSAPConn(Session session)
            : base(session)
        {
            session.IsObjectModifiedOnNonPersistentPropertyChange = true;
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
            }

            if (!string.IsNullOrEmpty(TypePassword))
                B1DbPassword = TypePassword;

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
        private bool _B1Post;
        [Index(0)]
        public bool B1Post
        {
            get { return _B1Post; }
            set
            {
                SetPropertyValue("B1Post", ref _B1Post, value);
            }
        }

        [Index(10)]
        public string BoFullName { get { return "Connection"; } }

        private string _B1Server;
        [Index(11)]
        public string B1Server
        {
            get { return _B1Server; }
            set
            {
                SetPropertyValue("B1Server", ref _B1Server, value);
            }
        }


        private string _B1CompanyDB;
        [Index(12)]
        public string B1CompanyDB
        {
            get { return _B1CompanyDB; }
            set
            {
                SetPropertyValue("B1CompanyDB", ref _B1CompanyDB, value);
            }
        }


        private string _B1License;
        [Index(13)]
        public string B1License
        {
            get { return _B1License; }
            set
            {
                SetPropertyValue("B1License", ref _B1License, value);
            }
        }


        private string _B1Language;
        [Index(14)]
        public string B1Language
        {
            get { return _B1Language; }
            set
            {
                SetPropertyValue("B1Language", ref _B1Language, value);
            }
        }


        private string _B1DbServerType;
        [Index(15)]
        public string B1DbServerType
        {
            get { return _B1DbServerType; }
            set
            {
                SetPropertyValue("B1DbServerType", ref _B1DbServerType, value);
            }
        }


        private string _B1DbUserName;
        [Index(16)]
        public string B1DbUserName
        {
            get { return _B1DbUserName; }
            set
            {
                SetPropertyValue("B1DbUserName", ref _B1DbUserName, value);
            }
        }


        [Browsable(false)]
        public string B1DbPassword { get; set; }

        private string _TypePassword;
        [XafDisplayName("DB Password")]
        [NonPersistent]
        [Index(18)]
        public string TypePassword
        {
            get { return _TypePassword; }
            set
            {
                SetPropertyValue("TypePassword", ref _TypePassword, value);
            }
        }


        private string _B1AttachmentPath;
        [Index(19)]
        public string B1AttachmentPath
        {
            get { return _B1AttachmentPath; }
            set
            {
                SetPropertyValue("B1AttachmentPath", ref _B1AttachmentPath, value);
            }
        }

    }
}