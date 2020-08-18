namespace FT_PurchasingPortal.Module.Controllers
{
    partial class PurchaseQuotationController
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
            this.PQCopyFromPR = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // PQCopyFromPR
            // 
            this.PQCopyFromPR.AcceptButtonCaption = null;
            this.PQCopyFromPR.CancelButtonCaption = null;
            this.PQCopyFromPR.Caption = "PQCopy From PR";
            this.PQCopyFromPR.ConfirmationMessage = null;
            this.PQCopyFromPR.Id = "PQCopyFromPR";
            this.PQCopyFromPR.ToolTip = null;
            this.PQCopyFromPR.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.PQCopyFromPR_CustomizePopupWindowParams);
            this.PQCopyFromPR.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.PQCopyFromPR_Execute);
            // 
            // PurchaseQuotationController
            // 
            this.Actions.Add(this.PQCopyFromPR);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction PQCopyFromPR;
    }
}
