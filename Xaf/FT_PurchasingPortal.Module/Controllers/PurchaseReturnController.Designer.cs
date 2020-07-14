namespace FT_PurchasingPortal.Module.Controllers
{
    partial class PurchaseReturnController
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
            this.CopyFromDO = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // CopyFromDO
            // 
            this.CopyFromDO.AcceptButtonCaption = null;
            this.CopyFromDO.CancelButtonCaption = null;
            this.CopyFromDO.Caption = "Copy From DO";
            this.CopyFromDO.ConfirmationMessage = null;
            this.CopyFromDO.Id = "CopyFromDO";
            this.CopyFromDO.ToolTip = null;
            this.CopyFromDO.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CopyFromDO_CustomizePopupWindowParams);
            this.CopyFromDO.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CopyFromDO_Execute);
            // 
            // PurchaseReturnController
            // 
            this.Actions.Add(this.CopyFromDO);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CopyFromDO;
    }
}
