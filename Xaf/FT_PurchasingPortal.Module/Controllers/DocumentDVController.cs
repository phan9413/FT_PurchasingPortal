using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using FT_PurchasingPortal.Module.BusinessObjects;

namespace FT_PurchasingPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class DocumentDVController : ViewController
    {
        RecordsNavigationController recordnaviator;
        GenController genCon;
        public DocumentDVController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetViewType = ViewType.DetailView;
        }
        public bool RejectDocAction(Object e, string paramString)
        {
            if (e.GetType() == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Rejected, paramString);
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Rejected;
                selectedObject.AppStatus.ApprovalStatus = ApprovalStatus.Not_Applicable;
                selectedObject.AppStatus.CurrApproval = null;
                selectedObject.AppStatus.CurrAppStage = 0;
            }
            else if (e.GetType() == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Rejected, paramString);
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Rejected;
                selectedObject.AppStatus.ApprovalStatus = ApprovalStatus.Not_Applicable;
                selectedObject.AppStatus.CurrApproval = null;
                selectedObject.AppStatus.CurrAppStage = 0;
            }
            else if (e.GetType() == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Rejected, paramString);
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Rejected;
                selectedObject.AppStatus.ApprovalStatus = ApprovalStatus.Not_Applicable;
                selectedObject.AppStatus.CurrApproval = null;
                selectedObject.AppStatus.CurrAppStage = 0;
            }

            return true;
        }
        public bool CloseDocAction(Object e, string paramString)
        {
            if (e.GetType() == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Closed, paramString);
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Closed;
            }
            else if (e.GetType() == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Closed, paramString);
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Closed;
            }
            else if (e.GetType() == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Closed, paramString);
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Closed;
            }

            return true;
        }
        public bool CancelDocAction(Object e, string paramString)
        {
            if (e.GetType() == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Cancelled, paramString);
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Cancelled;
            }
            else if (e.GetType() == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Cancelled, paramString);
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Cancelled;
            }
            else if (e.GetType() == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Cancelled, paramString);
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Cancelled;
            }

            return true;
        }
        public bool PostDocAction(Object e)
        {
            if (e.GetType() == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Posted, "");
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Posted;
            }
            else if (e.GetType() == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Posted, "");
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Posted;
            }
            else if (e.GetType() == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)e;

                selectedObject.DocStatus.AddDocStatus(DocStatus.Posted, "");
                selectedObject.DocStatus.CurrDocStatus = DocStatus.Posted;
            }

            return true;
        }
        public bool SubmitAction(Object e)
        {
            int oid = 0;
            string doctype = "";
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            bool isReqApp = false;
            if (e.GetType() == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)e;
                oid = selectedObject.Oid;
                doctype = selectedObject.DocType.BoCode;
                isReqApp = selectedObject.DocType.IsReqApp;
            }
            else if (e.GetType() == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e;
                oid = selectedObject.Oid;
                doctype = selectedObject.DocType.BoCode;
                isReqApp = selectedObject.DocType.IsReqApp;
            }
            else if (e.GetType() == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)e;
                oid = selectedObject.Oid;
                doctype = selectedObject.DocType.BoCode;
                isReqApp = selectedObject.DocType.IsReqApp;
            }

            #region sp_SubmitDocValidation
            XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();

            int ErrCode = 0;
            string ErrText = "";

            SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_SubmitDocValidation", new OperandValue(user.UserName), new OperandValue(oid), new OperandValue(doctype));

            if (sprocData.ResultSet.Count() > 0)
            {
                if (sprocData.ResultSet[0].Rows.Count() > 0)
                {
                    foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                    {
                        if (row.Values[0] != null)
                        {
                            ErrCode = int.Parse(row.Values[0].ToString());
                            ErrText = row.Values[1].ToString();
                        }
                    }
                }
            }
            persistentObjectSpace.Session.DropIdentityMap();
            persistentObjectSpace.Dispose();
            sprocData = null;

            if (ErrCode != 0)
            {
                genCon.showMsg("Failed", "[ErrCode=" + ErrCode.ToString() + "] ErrText=" + ErrText, InformationType.Error);
                return false;
            }
            #endregion

            if (isReqApp)
            {
                #region sp_GetApproval
                persistentObjectSpace = null;
                persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();

                ErrCode = 0;
                ErrText = "";

                sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_GetApproval", new OperandValue(user.UserName), new OperandValue(oid), new OperandValue(doctype));

                if (sprocData.ResultSet.Count() > 0)
                {
                    if (sprocData.ResultSet[0].Rows.Count() > 0)
                    {
                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            if (row.Values[0] != null)
                            {
                                //ErrCode = int.Parse(row.Values[0].ToString());
                                //ErrText = row.Values[1].ToString();
                            }
                        }
                    }
                }
                persistentObjectSpace.Session.DropIdentityMap();
                persistentObjectSpace.Dispose();
                sprocData = null;

                if (ErrCode != 0)
                {
                    genCon.showMsg("Failed", "[ErrCode=" + ErrCode.ToString() + "] ErrText=" + ErrText, InformationType.Error);
                    return false;
                }
                #endregion
            }

            IObjectSpace ios = Application.CreateObjectSpace();
            int CurrentApprovalOid = 0;
            int CurrentAppStageOid = 0;
            if (e.GetType() == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = ios.GetObjectByKey<StockTransferRequest>(((StockTransferRequest)e).Oid);
                if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected)
                {
                    if (selectedObject.AppStatus.ApprovalStatus == ApprovalStatus.Not_Applicable)
                    {
                        if (isReqApp && selectedObject.DocType.IsNoAppReject)
                        {
                            genCon.showMsg("Failed", "Approval Requeired for " + selectedObject.DocType.BoName, InformationType.Error);
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Submited, "");
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Rejected, "");
                            selectedObject.DocStatus.CurrDocStatus = DocStatus.Rejected;
                        }
                        else
                        {
                            selectedObject.AssignDocNumber();
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Submited, "");
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Accepted, "");
                            selectedObject.DocStatus.CurrDocStatus = DocStatus.Accepted;
                        }
                    }
                    else if (selectedObject.AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
                    {
                        selectedObject.DocStatus.AddDocStatus(DocStatus.Submited, "");
                        selectedObject.DocStatus.CurrDocStatus = DocStatus.Submited;
                        CurrentApprovalOid = selectedObject.AppStatus.GetCurrentApprovalOid();
                        if (CurrentApprovalOid > 0)
                        {
                            selectedObject.AppStatus.CurrApproval = ios.GetObjectByKey<Approval>(CurrentApprovalOid);
                            if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User && selectedObject.AppStatus.CurrApproval.AppointedUser != null)
                            {
                                selectedObject.AppStatus.CurrAppointedUser = ios.GetObjectByKey<Employee>(selectedObject.AppStatus.CurrApproval.AppointedUser.Oid);
                            }
                        }
                        CurrentAppStageOid = selectedObject.AppStatus.GetCurrentAppStageOid();
                        if (CurrentAppStageOid > 0)
                            selectedObject.AppStatus.CurrAppStage = CurrentAppStageOid;
                    }
                }
            }
            else if (e.GetType() == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = ios.GetObjectByKey<PurchaseOrder>(((PurchaseOrder)e).Oid);
                if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected)
                {
                    if (selectedObject.AppStatus.ApprovalStatus == ApprovalStatus.Not_Applicable)
                    {
                        if (isReqApp && selectedObject.DocType.IsNoAppReject)
                        {
                            genCon.showMsg("Failed", "Approval Requeired for " + selectedObject.DocType.BoName, InformationType.Error);
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Submited, "");
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Rejected, "");
                            selectedObject.DocStatus.CurrDocStatus = DocStatus.Rejected;
                        }
                        else
                        {
                            selectedObject.AssignDocNumber();
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Submited, "");
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Accepted, "");
                            selectedObject.DocStatus.CurrDocStatus = DocStatus.Accepted;
                        }
                    }
                    else if (selectedObject.AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
                    {
                        selectedObject.DocStatus.AddDocStatus(DocStatus.Submited, "");
                        selectedObject.DocStatus.CurrDocStatus = DocStatus.Submited;
                        CurrentApprovalOid = selectedObject.AppStatus.GetCurrentApprovalOid();
                        if (CurrentApprovalOid > 0)
                        {
                            selectedObject.AppStatus.CurrApproval = ios.GetObjectByKey<Approval>(CurrentApprovalOid);
                            if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User && selectedObject.AppStatus.CurrApproval.AppointedUser != null)
                            {
                                selectedObject.AppStatus.CurrAppointedUser = ios.GetObjectByKey<Employee>(selectedObject.AppStatus.CurrApproval.AppointedUser.Oid);
                            }
                        }
                        CurrentAppStageOid = selectedObject.AppStatus.GetCurrentAppStageOid();
                        if (CurrentAppStageOid > 0)
                            selectedObject.AppStatus.CurrAppStage = CurrentAppStageOid;
                    }
                }
            }
            else if (e.GetType() == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = ios.GetObjectByKey<PurchaseRequest>(((PurchaseRequest)e).Oid);
                if (selectedObject.DocStatus.CurrDocStatus == DocStatus.Draft || selectedObject.DocStatus.CurrDocStatus == DocStatus.Rejected)
                {
                    if (selectedObject.AppStatus.ApprovalStatus == ApprovalStatus.Not_Applicable)
                    {
                        if (isReqApp && selectedObject.DocType.IsNoAppReject)
                        {
                            genCon.showMsg("Failed", "Approval Requeired for " + selectedObject.DocType.BoName, InformationType.Error);
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Submited, "");
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Rejected, "");
                            selectedObject.DocStatus.CurrDocStatus = DocStatus.Rejected;
                        }
                        else
                        {
                            selectedObject.AssignDocNumber();
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Submited, "");
                            selectedObject.DocStatus.AddDocStatus(DocStatus.Accepted, "");
                            selectedObject.DocStatus.CurrDocStatus = DocStatus.Accepted;
                        }
                    }
                    else if (selectedObject.AppStatus.ApprovalStatus == ApprovalStatus.Required_Approval)
                    {
                        selectedObject.DocStatus.AddDocStatus(DocStatus.Submited, "");
                        selectedObject.DocStatus.CurrDocStatus = DocStatus.Submited;
                        CurrentApprovalOid = selectedObject.AppStatus.GetCurrentApprovalOid();
                        if (CurrentApprovalOid > 0)
                        {
                            selectedObject.AppStatus.CurrApproval = ios.GetObjectByKey<Approval>(CurrentApprovalOid);
                            if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User && selectedObject.AppStatus.CurrApproval.AppointedUser != null)
                            {
                                selectedObject.AppStatus.CurrAppointedUser = ios.GetObjectByKey<Employee>(selectedObject.AppStatus.CurrApproval.AppointedUser.Oid);
                            }
                        }
                        CurrentAppStageOid = selectedObject.AppStatus.GetCurrentAppStageOid();
                        if (CurrentAppStageOid > 0)
                            selectedObject.AppStatus.CurrAppStage = CurrentAppStageOid;
                    }
                }
            }

            ios.CommitChanges();
            return true;
        }

        public bool ApproveAction(object e, ApprovalStatus appstatus, string appRemark)
        {
            int oid = 0;
            string doctype = "";
            bool isReqApp = false;
            int CurrentAppStageOid = 0;
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

            if (e.GetType() == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)e;
                oid = selectedObject.Oid;
                doctype = selectedObject.DocType.BoCode;
                isReqApp = selectedObject.DocType.IsReqApp;
                CurrentAppStageOid = selectedObject.AppStatus.CurrAppStage;
            }
            else if (e.GetType() == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e;
                oid = selectedObject.Oid;
                doctype = selectedObject.DocType.BoCode;
                isReqApp = selectedObject.DocType.IsReqApp;
                CurrentAppStageOid = selectedObject.AppStatus.CurrAppStage;
            }
            else if (e.GetType() == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)e;
                oid = selectedObject.Oid;
                doctype = selectedObject.DocType.BoCode;
                isReqApp = selectedObject.DocType.IsReqApp;
                CurrentAppStageOid = selectedObject.AppStatus.CurrAppStage;
            }


            int newappstatus = 0;
            if (isReqApp) 
            {
                #region sp_Approval
                XPObjectSpace persistentObjectSpace = (XPObjectSpace)Application.CreateObjectSpace();

                int ErrCode = 0;
                string ErrText = "";

                SelectedData sprocData = persistentObjectSpace.Session.ExecuteSproc("sp_Approval", new OperandValue(user.UserName), new OperandValue(oid), new OperandValue(doctype), new OperandValue((int)appstatus), new OperandValue(appRemark), new OperandValue(CurrentAppStageOid));

                if (sprocData.ResultSet.Count() > 0)
                {
                    if (sprocData.ResultSet[0].Rows.Count() > 0)
                    {
                        foreach (SelectStatementResultRow row in sprocData.ResultSet[0].Rows)
                        {
                            if (row.Values[0] != null)
                            {
                                newappstatus = int.Parse(row.Values[0].ToString());
                                //ErrCode = int.Parse(row.Values[0].ToString());
                                //ErrText = row.Values[1].ToString();
                            }
                        }
                    }
                }
                persistentObjectSpace.Session.DropIdentityMap();
                persistentObjectSpace.Dispose();
                sprocData = null;

                if (ErrCode != 0)
                {
                    genCon.showMsg("Failed", "[ErrCode=" + ErrCode.ToString() + "] ErrText=" + ErrText, InformationType.Error);
                    return false;
                }
                #endregion

            }

            int CurrentApprovalOid = 0;

            IObjectSpace ios = Application.CreateObjectSpace();

            if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            {
                StockTransferRequest selectobj = ios.GetObjectByKey<StockTransferRequest>(((StockTransferRequest)View.CurrentObject).Oid);
                CurrentApprovalOid = selectobj.AppStatus.GetCurrentApprovalOid();
                CurrentAppStageOid = selectobj.AppStatus.GetCurrentAppStageOid();
                if (appstatus == ApprovalStatus.Rejected)
                {
                    selectobj.DocStatus.AddDocStatus(DocStatus.Rejected, "[APP]" + appRemark == null ? "" : appRemark);
                }
                else
                {
                    if (CurrentApprovalOid > 0 && CurrentAppStageOid > 0)
                    {
                        if (selectobj.AppStatus.CurrApproval != null && selectobj.AppStatus.CurrApproval.Oid == CurrentApprovalOid)
                        { }
                        else
                        {
                            selectobj.AppStatus.CurrApproval = ios.GetObjectByKey<Approval>(CurrentApprovalOid);
                            if (selectobj.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User && selectobj.AppStatus.CurrApproval.AppointedUser != null)
                            {
                                selectobj.AppStatus.CurrAppointedUser = ios.GetObjectByKey<Employee>(selectobj.AppStatus.CurrApproval.AppointedUser.Oid);
                            }
                        }
                        if (selectobj.AppStatus.CurrAppStage == CurrentAppStageOid)
                        { }
                        else
                        {
                            selectobj.AppStatus.CurrAppStage = CurrentAppStageOid;
                        }
                    }
                    else
                    {
                        selectobj.AssignDocNumber();
                        selectobj.DocStatus.AddDocStatus(DocStatus.Accepted, "");
                        selectobj.DocStatus.CurrDocStatus = DocStatus.Accepted;
                        selectobj.AppStatus.CurrApproval = null;
                        selectobj.AppStatus.CurrAppStage = 0;
                    }
                }
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder selectobj = ios.GetObjectByKey<PurchaseOrder>(((PurchaseOrder)View.CurrentObject).Oid);
                CurrentApprovalOid = selectobj.AppStatus.GetCurrentApprovalOid();
                CurrentAppStageOid = selectobj.AppStatus.GetCurrentAppStageOid();
                if (appstatus == ApprovalStatus.Rejected)
                {
                    selectobj.DocStatus.AddDocStatus(DocStatus.Rejected, "[APP]" + appRemark == null ? "" : appRemark);
                }
                else
                {
                    if (CurrentApprovalOid > 0 && CurrentAppStageOid > 0)
                    {
                        if (selectobj.AppStatus.CurrApproval != null && selectobj.AppStatus.CurrApproval.Oid == CurrentApprovalOid)
                        { }
                        else
                        {
                            selectobj.AppStatus.CurrApproval = ios.GetObjectByKey<Approval>(CurrentApprovalOid);
                            if (selectobj.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User && selectobj.AppStatus.CurrApproval.AppointedUser != null)
                            {
                                selectobj.AppStatus.CurrAppointedUser = ios.GetObjectByKey<Employee>(selectobj.AppStatus.CurrApproval.AppointedUser.Oid);
                            }
                        }
                        if (selectobj.AppStatus.CurrAppStage == CurrentAppStageOid)
                        { }
                        else
                        {
                            selectobj.AppStatus.CurrAppStage = CurrentAppStageOid;
                        }
                    }
                    else
                    {
                        selectobj.AssignDocNumber();
                        selectobj.DocStatus.AddDocStatus(DocStatus.Accepted, "");
                        selectobj.DocStatus.CurrDocStatus = DocStatus.Accepted;
                        selectobj.AppStatus.CurrApproval = null;
                        selectobj.AppStatus.CurrAppStage = 0;
                    }
                }
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            {
                PurchaseRequest selectobj = ios.GetObjectByKey<PurchaseRequest>(((PurchaseRequest)View.CurrentObject).Oid);
                CurrentApprovalOid = selectobj.AppStatus.GetCurrentApprovalOid();
                CurrentAppStageOid = selectobj.AppStatus.GetCurrentAppStageOid();
                if (appstatus == ApprovalStatus.Rejected)
                {
                    selectobj.DocStatus.AddDocStatus(DocStatus.Rejected, "[APP]" + appRemark == null ? "" : appRemark);
                    selectobj.DocStatus.CurrDocStatus = DocStatus.Rejected;
                }
                else
                {
                    if (CurrentApprovalOid > 0 && CurrentAppStageOid > 0)
                    {
                        if (selectobj.AppStatus.CurrApproval != null && selectobj.AppStatus.CurrApproval.Oid == CurrentApprovalOid)
                        { }
                        else
                        {
                            selectobj.AppStatus.CurrApproval = ios.GetObjectByKey<Approval>(CurrentApprovalOid);
                            if (selectobj.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User && selectobj.AppStatus.CurrApproval.AppointedUser != null)
                            {
                                selectobj.AppStatus.CurrAppointedUser = ios.GetObjectByKey<Employee>(selectobj.AppStatus.CurrApproval.AppointedUser.Oid);
                            }
                        }
                        if (selectobj.AppStatus.CurrAppStage == CurrentAppStageOid)
                        { }
                        else
                        {
                            selectobj.AppStatus.CurrAppStage = CurrentAppStageOid;
                        }
                    }
                    else
                    {
                        selectobj.AssignDocNumber();
                        selectobj.DocStatus.AddDocStatus(DocStatus.Accepted, "");
                        selectobj.DocStatus.CurrDocStatus = DocStatus.Accepted;
                        selectobj.AppStatus.CurrApproval = null;
                        selectobj.AppStatus.CurrAppStage = 0;
                    }
                }
            }
            ios.CommitChanges();

            return true;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (View is DetailView)
            {
                ((DetailView)View).ViewEditModeChanged += GenController_ViewEditModeChanged;

                recordnaviator = Frame.GetController<RecordsNavigationController>();
                if (recordnaviator != null)
                {
                    recordnaviator.PreviousObjectAction.Executed += PreviousObjectAction_Executed;
                    recordnaviator.NextObjectAction.Executed += NextObjectAction_Executed;
                }
                resetButton();
                enableButton();
            }
        }
        public void resetButton()
        {
            this.SubmitDoc.Active.SetItemValue("Enabled", false);
            this.CloseDoc.Active.SetItemValue("Enabled", false);
            this.PostDoc.Active.SetItemValue("Enabled", false);
            this.CancelDoc.Active.SetItemValue("Enabled", false);
            this.RejectDoc.Active.SetItemValue("Enabled", false);
            this.ApprovalDoc.Active.SetItemValue("Enabled", false);
            this.ChangeAppUser.Active.SetItemValue("Enabled", false);

            this.SwitchView.Active.SetItemValue("Enabled", true);

            if (typeof(ClassDocument).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                this.SubmitDoc.Active.SetItemValue("Enabled", true);
                this.CloseDoc.Active.SetItemValue("Enabled", true);
                this.PostDoc.Active.SetItemValue("Enabled", true);
                this.CancelDoc.Active.SetItemValue("Enabled", true);
                this.RejectDoc.Active.SetItemValue("Enabled", true);
                this.ApprovalDoc.Active.SetItemValue("Enabled", true);
                this.ChangeAppUser.Active.SetItemValue("Enabled", true);

            }
            else if (typeof(ClassStockTransferDocument).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                this.SubmitDoc.Active.SetItemValue("Enabled", true);
                this.CloseDoc.Active.SetItemValue("Enabled", true);
                this.PostDoc.Active.SetItemValue("Enabled", true);
                this.CancelDoc.Active.SetItemValue("Enabled", true);
                this.RejectDoc.Active.SetItemValue("Enabled", true);
                this.ApprovalDoc.Active.SetItemValue("Enabled", true);
                this.ChangeAppUser.Active.SetItemValue("Enabled", true);

            }

        }
        private void enableButton()
        {
            this.SubmitDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            this.CloseDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
            this.PostDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
            this.CancelDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
            this.RejectDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
            this.SwitchView.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            this.ApprovalDoc.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
            this.ChangeAppUser.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
        }
        private void GenController_ViewEditModeChanged(object sender, EventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                resetButton();
                enableButton();
            }
        }
        private void PreviousObjectAction_Executed(object sender, ActionBaseEventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                resetButton();
            }
        }
        private void NextObjectAction_Executed(object sender, ActionBaseEventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                resetButton();
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GenController>();
        }
        protected override void OnDeactivated()
        {
            if (View.GetType() == typeof(DetailView))
            {
                ((DetailView)View).ViewEditModeChanged -= GenController_ViewEditModeChanged;
                if (recordnaviator != null)
                {
                    recordnaviator.PreviousObjectAction.Executed -= PreviousObjectAction_Executed;
                    recordnaviator.NextObjectAction.Executed -= NextObjectAction_Executed;
                }
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        private void SubmitDoc_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            DocStatus docstatus = DocStatus.Cancelled;
            if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)e.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;

                //if (selectedObject.AppStatus.AppStatus.Count > 0)
                //{
                //    CriteriaOperator op = CriteriaOperator.Parse("AppStageDoc.Oid=?", selectedObject.AppStatus.Oid);
                //    XPCollection<StockTransferRequestAppStatus> xpcol = (XPCollection<StockTransferRequestAppStatus>)ObjectSpace.GetObjects<StockTransferRequestAppStatus>(op);
                //    ObjectSpace.Delete(xpcol);
                //}
                //if (selectedObject.AppStatus.AppStage.Count > 0)
                //{
                //    CriteriaOperator op = CriteriaOperator.Parse("AppStageDoc.Oid=?", selectedObject.AppStatus.Oid);
                //    XPCollection<StockTransferRequestAppStage> xpcol = (XPCollection<StockTransferRequestAppStage>)ObjectSpace.GetObjects<StockTransferRequestAppStage>(op);
                //    ObjectSpace.Delete(xpcol);
                //}
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;

                //if (selectedObject.AppStatus.AppStatus.Count > 0)
                //{
                //    CriteriaOperator op = CriteriaOperator.Parse("AppStageDoc.Oid=?", selectedObject.AppStatus.Oid);
                //    XPCollection<PurchaseOrderAppStatus> xpcol = (XPCollection<PurchaseOrderAppStatus>)ObjectSpace.GetObjects<PurchaseOrderAppStatus>(op);
                //    ObjectSpace.Delete(xpcol);
                //}
                //if (selectedObject.AppStatus.AppStage.Count > 0)
                //{
                //    CriteriaOperator op = CriteriaOperator.Parse("AppStageDoc.Oid=?", selectedObject.AppStatus.Oid);
                //    XPCollection<PurchaseOrderAppStage> xpcol = (XPCollection<PurchaseOrderAppStage>)ObjectSpace.GetObjects<PurchaseOrderAppStage>(op);
                //    ObjectSpace.Delete(xpcol);
                //}
                //if (selectedObject.CardCode == null)
                //{
                //    genCon.showMsg("Failed", "Please fill in Card Code.", InformationType.Error);
                //    return;
                //}
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)e.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;

                //if (selectedObject.AppStatus.AppStatus.Count > 0)
                //{
                //    CriteriaOperator op = CriteriaOperator.Parse("AppStageDoc.Oid=?", selectedObject.AppStatus.Oid);
                //    XPCollection<PurchaseRequestAppStatus> xpcol = (XPCollection<PurchaseRequestAppStatus>)ObjectSpace.GetObjects<PurchaseRequestAppStatus>(op);
                //    ObjectSpace.Delete(xpcol);
                //}
                //if (selectedObject.AppStatus.AppStage.Count > 0)
                //{
                //    CriteriaOperator op = CriteriaOperator.Parse("AppStageDoc.Oid=?", selectedObject.AppStatus.Oid);
                //    XPCollection<PurchaseRequestAppStage> xpcol = (XPCollection<PurchaseRequestAppStage>)ObjectSpace.GetObjects<PurchaseRequestAppStage>(op);
                //    ObjectSpace.Delete(xpcol);
                //}
            }
            if (docstatus == DocStatus.Draft || docstatus == DocStatus.Rejected)
            { }
            else
            {
                genCon.showMsg("Failed", "Only Draft document allowed.", InformationType.Error);
                return;
            }

            ModificationsController controller = Frame.GetController<ModificationsController>();
            if (controller != null)
            {
                if (!controller.SaveAction.DoExecute())
                {
                    genCon.showMsg("Failed", "Process failed.", InformationType.Error);
                    return;
                }
            }

            if (!SubmitAction(e.CurrentObject)) return;

            RefreshController refreshcontroller = Frame.GetController<RefreshController>();
            if (refreshcontroller != null)
                refreshcontroller.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Submit Done.", InformationType.Success);
            ((DetailView)View).ViewEditMode = ViewEditMode.View;
            View.BreakLinksToControls();
            View.CreateControls();
            //resetButton();
        }
        private void PostDoc_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            DocStatus docstatus = DocStatus.Cancelled;
            if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)e.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)e.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)e.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            if (docstatus == DocStatus.Closed)
            { }
            else
            {
                genCon.showMsg("Failed", "Only Closed document allowed.", InformationType.Error);
                return;
            }

            if (!PostDocAction(e.CurrentObject)) return;
            View.ObjectSpace.CommitChanges();
            RefreshController refCon = Frame.GetController<RefreshController>();
            if (refCon != null)
                refCon.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Post Done.", InformationType.Success);
        }
        private void CloseDoc_Cancel(object sender, EventArgs e)
        {
            genCon.showMsg("Failed", "Process cancelled.", InformationType.Error);
        }
        private void CloseDoc_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;
            string actionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            DocStatus docstatus = DocStatus.Cancelled;
            if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            if (docstatus == DocStatus.Accepted || docstatus == DocStatus.PostedCancel)
            { }
            else
            {
                err = true;
                actionMessage = "Document invalid.";
            }

            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = err;
            ((StringParameters)dv.CurrentObject).ActionMessage = actionMessage;

            e.View = dv;
        }
        private void CloseDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr)
            {
                genCon.showMsg("Failed", "Process failed. No changes done.", InformationType.Error);
                return;
            }

            if (!CloseDocAction(e.CurrentObject, p.ParamString)) return;
            View.ObjectSpace.CommitChanges();
            RefreshController refCon = Frame.GetController<RefreshController>();
            if (refCon != null)
                refCon.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Close Done.", InformationType.Success);
        }
        private void CancelDoc_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;
            string actionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            DocStatus docstatus = DocStatus.Cancelled;
            if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            if (docstatus == DocStatus.Draft || docstatus == DocStatus.Rejected)
            { }
            else
            {
                err = true;
                actionMessage = "Document invalid.";
            }

            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = err;
            ((StringParameters)dv.CurrentObject).ActionMessage = actionMessage;

            e.View = dv;

        }
        private void CancelDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr)
            {
                genCon.showMsg("Failed", "Process failed. No changes done.", InformationType.Error);
                return;
            }

            if (!CancelDocAction(e.CurrentObject, p.ParamString)) return;
            View.ObjectSpace.CommitChanges();
            RefreshController refCon = Frame.GetController<RefreshController>();
            if (refCon != null)
                refCon.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Cancel Done.", InformationType.Success);
        }
        private void RejectDoc_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;
            string actionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";

            DocStatus docstatus = DocStatus.Cancelled;
            if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
            }
            if (docstatus == DocStatus.Submited || docstatus == DocStatus.Accepted || docstatus == DocStatus.Closed || docstatus == DocStatus.PostedCancel)
            { }
            else
            {
                err = true;
                actionMessage = "Document invalid.";
            }

            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<StringParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((StringParameters)dv.CurrentObject).IsErr = err;
            ((StringParameters)dv.CurrentObject).ActionMessage = actionMessage;

            e.View = dv;

        }
        private void RejectDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            StringParameters p = (StringParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr)
            {
                genCon.showMsg("Failed", "Process failed. No changes done.", InformationType.Error);
                return;
            }

            if (!RejectDocAction(e.CurrentObject, p.ParamString)) return;
            View.ObjectSpace.CommitChanges();
            RefreshController refCon = Frame.GetController<RefreshController>();
            if (refCon != null)
                refCon.RefreshAction.DoExecute();
            genCon.showMsg("Successful", "Reject Done.", InformationType.Success);
        }

        private void SwitchView_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (ObjectSpace.ModifiedObjects.Count == 0)
            {
                ((DetailView)View).ViewEditMode = ViewEditMode.View;
                View.BreakLinksToControls();
                View.CreateControls();
            }
            else
            {
                genCon.showMsg("Error", "Please save the document 1st.", InformationType.Info);
            }

        }

        private void ApprovalDoc_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;
            string actionMessage = "Press OK to CONFIRM the action and SAVE, else press Cancel.";
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;

            DocStatus docstatus = DocStatus.Cancelled;
            ApprovalStatus apptatus = ApprovalStatus.Rejected;
            if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
                apptatus = selectedObject.AppStatus.ApprovalStatus;

                if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.User)
                {
                    if (selectedObject.AppStatus.CurrApproval.ApproveUser.Where(x => x.SystemUser.Oid == user.Oid).Count() > 0)
                    { }
                    else
                    {
                        err = true;
                        actionMessage = "Approval not allowed by this user.";
                    }
                }
                else if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Position)
                {
                    if (user.Employee != null && user.Employee.Position != null && selectedObject.AppStatus.CurrApproval.ApprovePosition.Where(x => x.Oid == user.Employee.Position.Oid).Count() > 0)
                    { }
                    else
                    {
                        err = true;
                        actionMessage = "Approval not allowed by this position";
                    }
                }
                else if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User)
                {
                    if (selectedObject.AppStatus.CurrAppointedUser != null && selectedObject.AppStatus.CurrAppointedUser.Oid == user.Employee.Oid)
                    { }
                    else
                    {
                        err = true;
                        actionMessage = "Approval not allowed by this appointed user.";
                    }
                }
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
                apptatus = selectedObject.AppStatus.ApprovalStatus;

                if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.User)
                {
                    if (selectedObject.AppStatus.CurrApproval.ApproveUser.Where(x => x.SystemUser.Oid == user.Oid).Count() > 0)
                    { }
                    else
                    {
                        err = true;
                        actionMessage = "Approval not allowed by this user.";
                    }
                }
                else if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Position)
                {
                    if (user.Employee != null && user.Employee.Position != null && selectedObject.AppStatus.CurrApproval.ApprovePosition.Where(x => x.Oid == user.Employee.Position.Oid).Count() > 0)
                    { }
                    else
                    {
                        err = true;
                        actionMessage = "Approval not allowed by this position";
                    }
                }
                else if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User)
                {
                    if (selectedObject.AppStatus.CurrAppointedUser != null && selectedObject.AppStatus.CurrAppointedUser.Oid == user.Employee.Oid)
                    { }
                    else
                    {
                        err = true;
                        actionMessage = "Approval not allowed by this appointed user.";
                    }
                }

            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
                apptatus = selectedObject.AppStatus.ApprovalStatus;

                if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.User)
                {
                    if (selectedObject.AppStatus.CurrApproval.ApproveUser.Where(x => x.SystemUser.Oid == user.Oid).Count() > 0)
                    { }
                    else
                    {
                        err = true;
                        actionMessage = "Approval not allowed by this user.";
                    }
                }
                else if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Position)
                {
                    if (user.Employee != null && user.Employee.Position != null && selectedObject.AppStatus.CurrApproval.ApprovePosition.Where(x => x.Oid == user.Employee.Position.Oid).Count() > 0)
                    { }
                    else
                    {
                        err = true;
                        actionMessage = "Approval not allowed by this position";
                    }
                }
                else if (selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User)
                {
                    if (selectedObject.AppStatus.CurrAppointedUser != null && selectedObject.AppStatus.CurrAppointedUser.Oid == user.Employee.Oid)
                    { }
                    else
                    {
                        err = true;
                        actionMessage = "Approval not allowed by this appointed user.";
                    }
                }
            }
            if (docstatus == DocStatus.Submited || apptatus == ApprovalStatus.Required_Approval)
            { }
            else
            {
                err = true;
                actionMessage = "Document invalid.";
            }

            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<ApprovalParameters>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;
            ((ApprovalParameters)dv.CurrentObject).IsErr = err;
            ((ApprovalParameters)dv.CurrentObject).ActionMessage = actionMessage;

            e.View = dv;
        }

        private void ApprovalDoc_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            ApprovalStatus appstatus = ApprovalStatus.Required_Approval;
            int oid = 0;
            int currentappstage = 0;
            int userlastappstage = 0;
            if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)View.CurrentObject;
                oid = selectedObject.Oid;
                currentappstage = selectedObject.AppStatus.CurrAppStage;
                if (selectedObject.AppStatus.AppStatus.Where(x => x.CreateUser.Oid == user.Oid).Count() > 0)
                {
                    appstatus = selectedObject.AppStatus.AppStatus.Where(x => x.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().ApprovalStatus;
                    userlastappstage = selectedObject.AppStatus.AppStatus.Where(x => x.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStage;
                }
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)View.CurrentObject;
                oid = selectedObject.Oid;
                currentappstage = selectedObject.AppStatus.CurrAppStage;
                if (selectedObject.AppStatus.AppStatus.Where(x => x.CreateUser.Oid == user.Oid).Count() > 0)
                {
                    appstatus = selectedObject.AppStatus.AppStatus.Where(x => x.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().ApprovalStatus;
                    userlastappstage = selectedObject.AppStatus.AppStatus.Where(x => x.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStage;
                }
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)View.CurrentObject;
                oid = selectedObject.Oid;
                currentappstage = selectedObject.AppStatus.CurrAppStage;
                if (selectedObject.AppStatus.AppStatus.Where(x => x.CreateUser.Oid == user.Oid).Count() > 0)
                {
                    appstatus = selectedObject.AppStatus.AppStatus.Where(x => x.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().ApprovalStatus;
                    userlastappstage = selectedObject.AppStatus.AppStatus.Where(x => x.CreateUser.Oid == user.Oid).OrderBy(c => c.Oid).Last().AppStage;
                }
            }

            ApprovalParameters p = (ApprovalParameters)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;
            string appRemark = p.ParamString;
            if (userlastappstage == currentappstage)
            {
                if (appstatus == ApprovalStatus.Required_Approval && p.AppStatus == ApprovalAction.NA)
                {
                    genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                    return;
                }
                else if (appstatus == ApprovalStatus.Approved && p.AppStatus == ApprovalAction.Yes)
                {
                    genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                    return;
                }
                else if (appstatus == ApprovalStatus.Rejected && p.AppStatus == ApprovalAction.No)
                {
                    genCon.showMsg("Failed", "Same Approval Status is not allowed.", InformationType.Error);
                    return;
                }
            }
            if (p.AppStatus == ApprovalAction.NA)
            {
                appstatus = ApprovalStatus.Required_Approval;
            }
            if (p.AppStatus == ApprovalAction.Yes)
            {
                appstatus = ApprovalStatus.Approved;
            }
            if (p.AppStatus == ApprovalAction.No)
            {
                appstatus = ApprovalStatus.Rejected;
            }

            //string doctype = "";
            //if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            //{
            //    StockTransferRequest selectedObject = (StockTransferRequest)View.CurrentObject;
            //    doctype = selectedObject.DocType.BoCode;
            //    selectedObject.AppStatus.AddAppStatus(appstatus, selectedObject.AppStatus.CurrAppStage.Oid, appRemark);
            //}
            //else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            //{
            //    PurchaseOrder selectedObject = (PurchaseOrder)View.CurrentObject;
            //    doctype = selectedObject.DocType.BoCode;
            //    selectedObject.AppStatus.AddAppStatus(appstatus, selectedObject.AppStatus.CurrAppStage.Oid, appRemark);
            //}
            //else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            //{
            //    PurchaseRequest selectedObject = (PurchaseRequest)View.CurrentObject;
            //    doctype = selectedObject.DocType.BoCode;
            //    selectedObject.AppStatus.AddAppStatus(appstatus, selectedObject.AppStatus.CurrAppStage.Oid, appRemark);
            //}


            if (!ApproveAction(View.CurrentObject, appstatus, appRemark)) return;


            RefreshController refreshcontroller = Frame.GetController<RefreshController>();
            if (refreshcontroller != null)
                refreshcontroller.RefreshAction.DoExecute();

            genCon.showMsg("Successful", "Approval Done.", InformationType.Success);
            //((DetailView)View).ViewEditMode = ViewEditMode.View;
            //View.BreakLinksToControls();
            //View.CreateControls();
            resetButton();

        }

        private void ChangeAppUser_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            bool err = false;

            DocStatus docstatus = DocStatus.Cancelled;
            ApprovalStatus apptatus = ApprovalStatus.Rejected;
            bool isappointuser = false;
            if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
                apptatus = selectedObject.AppStatus.ApprovalStatus;
                if (selectedObject.AppStatus.CurrApproval != null && selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User)
                {
                    isappointuser = true;
                }
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
                apptatus = selectedObject.AppStatus.ApprovalStatus;
                if (selectedObject.AppStatus.CurrApproval != null && selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User)
                {
                    isappointuser = true;
                }
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)View.CurrentObject;
                docstatus = selectedObject.DocStatus.CurrDocStatus;
                apptatus = selectedObject.AppStatus.ApprovalStatus;
                if (selectedObject.AppStatus.CurrApproval != null && selectedObject.AppStatus.CurrApproval.ApprovalBy == ApprovalBy.Appointed_User)
                {
                    isappointuser = true;
                }
            }
            if (docstatus == DocStatus.Submited && apptatus == ApprovalStatus.Required_Approval && isappointuser)
            { }
            else
            {
                err = true;
            }

            if (err)
            {
                IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(Employee));
                string listViewId = Application.FindLookupListViewId(typeof(Employee));
                CollectionSourceBase collectionSource = Application.CreateCollectionSource(newObjectSpace, typeof(Employee), listViewId);
                collectionSource.Criteria.Add("filter01", CriteriaOperator.Parse("1=0"));
                e.View = Application.CreateListView(listViewId, collectionSource, true);
            }
            else
            {
                IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(Employee));
                string listViewId = Application.FindLookupListViewId(typeof(Employee));
                CollectionSourceBase collectionSource = Application.CreateCollectionSource(newObjectSpace, typeof(Employee), listViewId);
                e.View = Application.CreateListView(listViewId, collectionSource, true);
            }
        }

        private void ChangeAppUser_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (e.PopupWindowViewSelectedObjects.Count != 1)
            {
                genCon.showMsg("Failed", "Please select only 1 user.", InformationType.Error);
                return;
            }
            Employee user = null;
            foreach (Employee dtl in e.PopupWindowViewSelectedObjects)
            {
                user = dtl;
            }

            if (View.ObjectTypeInfo.Type == typeof(StockTransferRequest))
            {
                StockTransferRequest selectedObject = (StockTransferRequest)View.CurrentObject;
                selectedObject.AppStatus.CurrAppointedUser = View.ObjectSpace.GetObjectByKey<Employee>(user.Oid);
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseOrder))
            {
                PurchaseOrder selectedObject = (PurchaseOrder)View.CurrentObject;
                selectedObject.AppStatus.CurrAppointedUser = View.ObjectSpace.GetObjectByKey<Employee>(user.Oid);
            }
            else if (View.ObjectTypeInfo.Type == typeof(PurchaseRequest))
            {
                PurchaseRequest selectedObject = (PurchaseRequest)View.CurrentObject;
                selectedObject.AppStatus.CurrAppointedUser = View.ObjectSpace.GetObjectByKey<Employee>(user.Oid);
            }
            View.ObjectSpace.CommitChanges();

            RefreshController refreshcontroller = Frame.GetController<RefreshController>();
            if (refreshcontroller != null)
                refreshcontroller.RefreshAction.DoExecute();

            genCon.showMsg("Successful", "Change appointed user done.", InformationType.Success);
            //((DetailView)View).ViewEditMode = ViewEditMode.View;
            //View.BreakLinksToControls();
            //View.CreateControls();
            resetButton();
        }
    }
}
