namespace FT_PurchasingPortal.Module.Controllers
{
    partial class PurchaseDeliveryController
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
            this.CopyFromPO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // CopyFromPO
            // 
            this.CopyFromPO.AcceptButtonCaption = null;
            this.CopyFromPO.CancelButtonCaption = null;
            this.CopyFromPO.Caption = "Copy From PO";
            this.CopyFromPO.ConfirmationMessage = null;
            this.CopyFromPO.Id = "CopyFromPO";
            this.CopyFromPO.ToolTip = null;
            this.CopyFromPO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CopyFromPO_CustomizePopupWindowParams);
            this.CopyFromPO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CopyFromPO_Execute);
            // 
            // PurchaseDeliveryController
            // 
            this.Actions.Add(this.CopyFromPO);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CopyFromPO;
    }
}
