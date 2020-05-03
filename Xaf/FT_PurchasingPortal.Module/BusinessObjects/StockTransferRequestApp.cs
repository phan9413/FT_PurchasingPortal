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
    [Persistent("OSTFA")]
    public class StockTransferRequestApp : ClassAppStatusDoc
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public StockTransferRequestApp(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }
        [Association("STAppStageDoc-AppStage")]
        [XafDisplayName("Approval Stages")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<StockTransferRequestAppStage> AppStage
        {
            get { return GetCollection<StockTransferRequestAppStage>("AppStage"); }
        }

        [Association("STAppStageDoc-AppStatus")]
        [XafDisplayName("Approval Status")]
        [DevExpress.Xpo.Aggregated]
        public XPCollection<StockTransferRequestAppStatus> AppStatus
        {
            get { return GetCollection<StockTransferRequestAppStatus>("AppStatus"); }
        }
        //public void AddAppStatus(ApprovalStatus newstatus, int approvalOid, int appStageOid, string remarks)
        //{
        //    StockTransferRequestAppStatus ds = new StockTransferRequestAppStatus(Session);
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
                foreach (StockTransferRequestAppStage dtl in AppStage.OrderBy(p => p.Oid))
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
                foreach (StockTransferRequestAppStage dtl in AppStage.OrderBy(p => p.Oid))
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

    [Persistent("OSTFA1")]
    public class StockTransferRequestAppStage : ClassAppStage
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public StockTransferRequestAppStage(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private StockTransferRequestApp _AppStageDoc;
        [Browsable(false)]
        [Association("STAppStageDoc-AppStage")]
        public StockTransferRequestApp AppStageDoc
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
    [Persistent("OSTFA2")]
    public class StockTransferRequestAppStatus : ClassAppStatus
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public StockTransferRequestAppStatus(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        private StockTransferRequestApp _AppStageDoc;
        [Browsable(false)]
        [Association("STAppStageDoc-AppStatus")]
        public StockTransferRequestApp AppStageDoc
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