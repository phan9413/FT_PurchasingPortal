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
    [Persistent("OPRQD")]
    public class PurchaseOrderDoc : ClassDocStatusDoc
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseOrderDoc(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            IsSAPPosted = false;
        }

        [Association("PODocStatus-DocStatus")]
        [XafDisplayName("Document Status")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<PurchaseOrderDocStatus> DocumentStatus
        {
            get { return GetCollection<PurchaseOrderDocStatus>("DocumentStatus"); }
        }

        //public void assignCurrDocStatus()
        //{
        //    if (DocumentStatus.Count > 0)
        //        CurrDocStatus = DocumentStatus.OrderByDescending<PurchaseOrderDocStatus, DateTime?>(pp => pp.CreateDate).First<PurchaseOrderDocStatus>().DocStatus;

        //}
        public void AddDocStatus(DocStatus newstatus, string remarks)
        {
            PurchaseOrderDocStatus ds = new PurchaseOrderDocStatus(Session);
            ds.DocStatus = newstatus;
            ds.DocStatusRemarks = remarks;
            if (!GeneralValues.IsNetCore)
                ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            else
                ds.CreateUser = Session.FindObject<SystemUsers>(CriteriaOperator.Parse("UserName=?", GeneralValues.NetCoreUserName));
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
                    PurchaseOrderDocStatus ds = new PurchaseOrderDocStatus(Session);
                    ds.DocStatus = DocStatus.Draft;
                    ds.DocStatusRemarks = "";
                    if (!GeneralValues.IsNetCore)
                        ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    else
                        ds.CreateUser = Session.FindObject<SystemUsers>(CriteriaOperator.Parse("UserName=?", GeneralValues.NetCoreUserName));
                    ds.CreateDate = DateTime.Now;
                    this.DocumentStatus.Add(ds);
                }
                else
                {
                }
            }
        }
    }
    [Persistent("OPRQD1")]
    public class PurchaseOrderDocStatus : ClassDocStatus
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseOrderDocStatus(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private PurchaseOrderDoc _DocStatusDoc;
        [Browsable(false)]
        [Association("PODocStatus-DocStatus")]
        public PurchaseOrderDoc DocStatusDoc
        {
            get { return _DocStatusDoc; }
            set
            {
                SetPropertyValue("DocStatusDoc", ref _DocStatusDoc, value);
            }
        }
    }
}