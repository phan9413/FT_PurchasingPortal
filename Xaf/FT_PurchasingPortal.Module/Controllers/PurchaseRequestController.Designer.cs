namespace FT_PurchasingPortal.Module.Controllers
{
    partial class PurchaseRequestController
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
            this.CopyToPO = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // CopyToPO
            // 
            this.CopyToPO.Caption = "Copy To PO";
            this.CopyToPO.ConfirmationMessage = null;
            this.CopyToPO.Id = "CopyToPO";
            this.CopyToPO.ToolTip = null;
            this.CopyToPO.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.CopyToPO_Execute);
            // 
            // PurchaseRequestController
            // 
            this.Actions.Add(this.CopyToPO);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction CopyToPO;
    }
}
