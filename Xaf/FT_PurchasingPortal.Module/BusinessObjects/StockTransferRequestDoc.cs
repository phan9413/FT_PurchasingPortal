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
    [Persistent("OSTFD")]
    public class StockTransferRequestDoc : ClassDocStatusDoc
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public StockTransferRequestDoc(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsSAPPosted = false;
        }

        [Association("TRDocStatus-DocStatus")]
        [XafDisplayName("Document Status")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<StockTransferRequestDocStatus> DocumentStatus
        {
            get { return GetCollection<StockTransferRequestDocStatus>("DocumentStatus"); }
        }

        //public void assignCurrDocStatus()
        //{
        //    if (DocumentStatus.Count > 0)
        //        CurrDocStatus = DocumentStatus.OrderByDescending<StockTransferRequestDocStatus, DateTime?>(pp => pp.CreateDate).First<StockTransferRequestDocStatus>().DocStatus;

        //}
        public void AddDocStatus(DocStatus newstatus, string remarks)
        {
            StockTransferRequestDocStatus ds = new StockTransferRequestDocStatus(Session);
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
                    StockTransferRequestDocStatus ds = new StockTransferRequestDocStatus(Session);
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
    [Persistent("OSTFD1")]
    public class StockTransferRequestDocStatus : ClassDocStatus
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public StockTransferRequestDocStatus(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private StockTransferRequestDoc _DocStatusDoc;
        [Browsable(false)]
        [Association("TRDocStatus-DocStatus")]
        public StockTransferRequestDoc DocStatusDoc
        {
            get { return _DocStatusDoc; }
            set
            {
                SetPropertyValue("DocStatusDoc", ref _DocStatusDoc, value);
            }
        }
    }
}