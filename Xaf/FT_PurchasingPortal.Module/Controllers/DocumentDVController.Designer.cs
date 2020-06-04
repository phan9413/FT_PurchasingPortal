namespace FT_PurchasingPortal.Module.Controllers
{
    partial class DocumentDVController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.CloseDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.ReOpenDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.SubmitDoc = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.PostDoc = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.CancelDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.RejectDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.SwitchView = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.ApprovalDoc = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.ChangeAppUser = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // CloseDoc
            // 
            this.CloseDoc.AcceptButtonCaption = null;
            this.CloseDoc.CancelButtonCaption = null;
            this.CloseDoc.Caption = "Close Doc";
            this.CloseDoc.ConfirmationMessage = null;
            this.CloseDoc.Id = "CloseDoc";
            this.CloseDoc.ToolTip = null;
            this.CloseDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CloseDoc_CustomizePopupWindowParams);
            this.CloseDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CloseDoc_Execute);
            this.CloseDoc.Cancel += new System.EventHandler(this.CloseDoc_Cancel);
            // 
            // ReOpenDoc
            // 
            this.ReOpenDoc.AcceptButtonCaption = null;
            this.ReOpenDoc.CancelButtonCaption = null;
            this.ReOpenDoc.Caption = "Re-open";
            this.ReOpenDoc.ConfirmationMessage = null;
            this.ReOpenDoc.Id = "ReOpenDoc";
            this.ReOpenDoc.ToolTip = null;
            this.ReOpenDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ReOpenDoc_CustomizePopupWindowParams);
            this.ReOpenDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.ReOpenDoc_Execute);
            // 
            // SubmitDoc
            // 
            this.SubmitDoc.Caption = "Submit Doc";
            this.SubmitDoc.ConfirmationMessage = null;
            this.SubmitDoc.Id = "SubmitDoc";
            this.SubmitDoc.ToolTip = null;
            this.SubmitDoc.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.SubmitDoc_Execute);
            // 
            // PostDoc
            // 
            this.PostDoc.Caption = "Post Doc";
            this.PostDoc.ConfirmationMessage = null;
            this.PostDoc.Id = "PostDoc";
            this.PostDoc.ToolTip = null;
            this.PostDoc.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.PostDoc_Execute);
            // 
            // CancelDoc
            // 
            this.CancelDoc.AcceptButtonCaption = null;
            this.CancelDoc.CancelButtonCaption = null;
            this.CancelDoc.Caption = "Cancel Doc";
            this.CancelDoc.ConfirmationMessage = null;
            this.CancelDoc.Id = "CancelDoc";
            this.CancelDoc.ToolTip = null;
            this.CancelDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CancelDoc_CustomizePopupWindowParams);
            this.CancelDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CancelDoc_Execute);
            this.CancelDoc.Cancel += new System.EventHandler(this.CloseDoc_Cancel);
            // 
            // RejectDoc
            // 
            this.RejectDoc.AcceptButtonCaption = null;
            this.RejectDoc.CancelButtonCaption = null;
            this.RejectDoc.Caption = "Reject Doc";
            this.RejectDoc.ConfirmationMessage = null;
            this.RejectDoc.Id = "RejectDoc";
            this.RejectDoc.ToolTip = null;
            this.RejectDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.RejectDoc_CustomizePopupWindowParams);
            this.RejectDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.RejectDoc_Execute);
            this.RejectDoc.Cancel += new System.EventHandler(this.CloseDoc_Cancel);
            // 
            // SwitchView
            // 
            this.SwitchView.Caption = "Switch View";
            this.SwitchView.ConfirmationMessage = null;
            this.SwitchView.Id = "SwitchView";
            this.SwitchView.ToolTip = null;
            this.SwitchView.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.SwitchView_Execute);
            // 
            // ApprovalDoc
            // 
            this.ApprovalDoc.AcceptButtonCaption = null;
            this.ApprovalDoc.CancelButtonCaption = null;
            this.ApprovalDoc.Caption = "Approval";
            this.ApprovalDoc.ConfirmationMessage = null;
            this.ApprovalDoc.Id = "ApprovalDoc";
            this.ApprovalDoc.ToolTip = null;
            this.ApprovalDoc.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ApprovalDoc_CustomizePopupWindowParams);
            this.ApprovalDoc.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.ApprovalDoc_Execute);
            this.ApprovalDoc.Cancel += new System.EventHandler(this.CloseDoc_Cancel);
            // 
            // ChangeAppUser
            // 
            this.ChangeAppUser.AcceptButtonCaption = null;
            this.ChangeAppUser.CancelButtonCaption = null;
            this.ChangeAppUser.Caption = "Change App User";
            this.ChangeAppUser.ConfirmationMessage = null;
            this.ChangeAppUser.Id = "ChangeAppUser";
            this.ChangeAppUser.ToolTip = null;
            this.ChangeAppUser.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ChangeAppUser_CustomizePopupWindowParams);
            this.ChangeAppUser.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.ChangeAppUser_Execute);
            this.ChangeAppUser.Cancel += new System.EventHandler(this.CloseDoc_Cancel);
            // 
            // DocumentDVController
            // 
            this.Actions.Add(this.CloseDoc);
            this.Actions.Add(this.ReOpenDoc);
            this.Actions.Add(this.SubmitDoc);
            this.Actions.Add(this.PostDoc);
            this.Actions.Add(this.CancelDoc);
            this.Actions.Add(this.RejectDoc);
            this.Actions.Add(this.SwitchView);
            this.Actions.Add(this.ApprovalDoc);
            this.Actions.Add(this.ChangeAppUser);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CloseDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ReOpenDoc;
        private DevExpress.ExpressApp.Actions.SimpleAction SubmitDoc;
        private DevExpress.ExpressApp.Actions.SimpleAction PostDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CancelDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction RejectDoc;
        private DevExpress.ExpressApp.Actions.SimpleAction SwitchView;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ApprovalDoc;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction ChangeAppUser;
    }
}
