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
    #region headr
    [Persistent("OPRUA")]
    public class PurchaseQuotationApp : ClassAppStatusDoc
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseQuotationApp(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        [Association("PUAppStageDoc-AppStage")]
        [XafDisplayName("Approval Stages")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<PurchaseQuotationAppStage> AppStage
        {
            get { return GetCollection<PurchaseQuotationAppStage>("AppStage"); }
        }

        [Association("PUAppStageDoc-AppStatus")]
        [XafDisplayName("Approval Status")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<PurchaseQuotationAppStatus> AppStatus
        {
            get { return GetCollection<PurchaseQuotationAppStatus>("AppStatus"); }
        }
        //public void AddAppStatus(ApprovalStatus newstatus, int approvalOid, int appStageOid, string remarks)
        //{
        //    PurchaseQuotationAppStatus ds = new PurchaseQuotationAppStatus(Session);
        //    ds.Approval = Session.GetObjectByKey<Approval>(approvalOid); ;
        //    ds.AppStage = appStageOid;
        //    ds.ApprovalStatus = newstatus;
        //    ds.ApprovalRemarks = remarks;
        //    ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
        //    ds.CreateDate = DateTime.Now;
        //    this.AppStatus.Add(ds);

        //}

        public int GetCurrentApprovalOid()
        {
            int rtn = 0;
            int appcount = 0;
            if (AppStage != null && AppStage.Count > 0)
            {
                foreach (PurchaseQuotationAppStage dtl in AppStage.OrderBy(p => p.Oid))
                {
                    if (AppStatus != null && AppStatus.Count > 0)
                    {
                        appcount = AppStatus.Where(pp => pp.AppStage == dtl.Oid && pp.ApprovalStatus == ApprovalStatus.Approved).Count();
                        if (appcount == null) appcount = 0;
                        if (dtl.Approval.ApprovalCnt > appcount)
                        {
                            rtn = dtl.Approval.Oid;
                            break;
                        }
                    }
                    else
                    {
                        if (dtl.Approval.ApprovalCnt > 0)
                        {
                            rtn = dtl.Approval.Oid;
                            break;
                        }
                    }
                }
            }

            return rtn;
        }
        public int GetCurrentAppStageOid()
        {
            int rtn = 0;
            int appcount = 0;
            if (AppStage != null && AppStage.Count > 0)
            {
                foreach (PurchaseQuotationAppStage dtl in AppStage.OrderBy(p => p.Oid))
                {
                    if (AppStatus != null && AppStatus.Count > 0)
                    {
                        appcount = AppStatus.Where(pp => pp.AppStage == dtl.Oid && pp.ApprovalStatus == ApprovalStatus.Approved).Count();
                        if (appcount == null) appcount = 0;
                        if (dtl.Approval.ApprovalCnt > appcount)
                        {
                            rtn = dtl.Oid;
                            break;
                        }
                    }
                    else
                    {
                        if (dtl.Approval.ApprovalCnt > 0)
                        {
                            rtn = dtl.Oid;
                            break;
                        }
                    }
                }
            }

            return rtn;
        }

    }
    #endregion

    #region headr Approval Stage

    [Persistent("OPRUA1")]
    public class PurchaseQuotationAppStage : ClassAppStage
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseQuotationAppStage(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private PurchaseQuotationApp _AppStageDoc;
        [Browsable(false)]
        [Association("PUAppStageDoc-AppStage")]
        public PurchaseQuotationApp AppStageDoc
        {
            get { return _AppStageDoc; }
            set
            {
                SetPropertyValue("AppStageDoc", ref _AppStageDoc, value);
            }
        }
    }
    #endregion

    #region headr Approval Status
    [Persistent("OPRUA2")]
    public class PurchaseQuotationAppStatus : ClassAppStatus
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseQuotationAppStatus(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private PurchaseQuotationApp _AppStageDoc;
        [Browsable(false)]
        [Association("PUAppStageDoc-AppStatus")]
        public PurchaseQuotationApp AppStageDoc
        {
            get { return _AppStageDoc; }
            set
            {
                SetPropertyValue("AppStageDoc", ref _AppStageDoc, value);
            }
        }

    }
    #endregion
}