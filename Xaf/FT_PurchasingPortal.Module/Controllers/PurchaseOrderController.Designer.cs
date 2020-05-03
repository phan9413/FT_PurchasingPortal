namespace FT_PurchasingPortal.Module.Controllers
{
    partial class PurchaseOrderController
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
            this.CopyFromPR = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // CopyFromPR
            // 
            this.CopyFromPR.AcceptButtonCaption = null;
            this.CopyFromPR.CancelButtonCaption = null;
            this.CopyFromPR.Caption = "Copy From PR";
            this.CopyFromPR.ConfirmationMessage = null;
            this.CopyFromPR.Id = "CopyFromPR";
            this.CopyFromPR.ToolTip = null;
            this.CopyFromPR.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CopyFromPR_CustomizePopupWindowParams);
            this.CopyFromPR.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CopyFromPR_Execute);
            // 
            // PurchaseOrderController
            // 
            this.Actions.Add(this.CopyFromPR);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CopyFromPR;
    }
}
