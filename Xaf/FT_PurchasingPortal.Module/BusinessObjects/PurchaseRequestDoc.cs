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
    // Specify more UI options using a declarative approach (https://documentation.devexpress.com/#eXpressAppFramework/CustomDocument112701).
    [Persistent("OPRED")]
    public class PurchaseRequestDoc : ClassDocStatusDoc
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequestDoc(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsSAPPosted = false;
        }

        [Association("PRDocStatus-DocStatus")]
        [XafDisplayName("Document Status")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<PurchaseRequestDocStatus> DocumentStatus
        {
            get { return GetCollection<PurchaseRequestDocStatus>("DocumentStatus"); }
        }

        //public void assignCurrDocStatus()
        //{
        //    if (DocumentStatus.Count > 0)
        //        CurrDocStatus = DocumentStatus.OrderByDescending<PurchaseOrderDocStatus, DateTime?>(pp => pp.CreateDate).First<PurchaseOrderDocStatus>().DocStatus;

        //}
        public void AddDocStatus(DocStatus newstatus, string remarks)
        {
            PurchaseRequestDocStatus ds = new PurchaseRequestDocStatus(Session);
            ds.DocStatus = newstatus;
            ds.DocStatusRemarks = remarks;
            ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            ds.CreateDate = DateTime.Now;
            this.DocumentStatus.Add(ds);

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
                    PurchaseRequestDocStatus ds = new PurchaseRequestDocStatus(Session);
                    ds.DocStatus = DocStatus.Draft;
                    ds.DocStatusRemarks = "";
                    ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.CreateDate = DateTime.Now;
                    this.DocumentStatus.Add(ds);
                }
                else
                {
                }
            }
        }
    }
    [Persistent("OPRED1")]
    public class PurchaseRequestDocStatus : ClassDocStatus
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequestDocStatus(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private PurchaseRequestDoc _DocStatusDoc;
        [Browsable(false)]
        [Association("PRDocStatus-DocStatus")]
        public PurchaseRequestDoc DocStatusDoc
        {
            get { return _DocStatusDoc; }
            set
            {
                SetPropertyValue("DocStatusDoc", ref _DocStatusDoc, value);
            }
        }
    }
}