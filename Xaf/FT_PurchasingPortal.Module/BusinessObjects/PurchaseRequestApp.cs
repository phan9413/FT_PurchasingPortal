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
    [Persistent("OPREA")]
    public class PurchaseRequestApp : ClassAppStatusDoc
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequestApp(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        [Association("PRAppStageDoc-AppStage")]
        [XafDisplayName("Approval Stages")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<PurchaseRequestAppStage> AppStage
        {
            get { return GetCollection<PurchaseRequestAppStage>("AppStage"); }
        }

        [Association("PRAppStageDoc-AppStatus")]
        [XafDisplayName("Approval Status")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<PurchaseRequestAppStatus> AppStatus
        {
            get { return GetCollection<PurchaseRequestAppStatus>("AppStatus"); }
        }
        //public void AddAppStatus(ApprovalStatus newstatus, int approvalOid, int appStageOid, string remarks)
        //{
        //    PurchaseRequestAppStatus ds = new PurchaseRequestAppStatus(Session);
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
                foreach (PurchaseRequestAppStage dtl in AppStage.OrderBy(p => p.Oid))
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
                foreach (PurchaseRequestAppStage dtl in AppStage.OrderBy(p => p.Oid))
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

    [Persistent("OPREA1")]
    public class PurchaseRequestAppStage : ClassAppStage
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequestAppStage(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private PurchaseRequestApp _AppStageDoc;
        [Browsable(false)]
        [Association("PRAppStageDoc-AppStage")]
        public PurchaseRequestApp AppStageDoc
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
    [Persistent("OPREA2")]
    public class PurchaseRequestAppStatus : ClassAppStatus
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequestAppStatus(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private PurchaseRequestApp _AppStageDoc;
        [Browsable(false)]
        [Association("PRAppStageDoc-AppStatus")]
        public PurchaseRequestApp AppStageDoc
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