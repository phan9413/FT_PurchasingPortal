namespace FT_PurchasingPortal.Module.Controllers
{
    partial class vwSAP_ITEM_AVAILABILITYController
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
            this.CopyToSTR = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // CopyToSTR
            // 
            this.CopyToSTR.AcceptButtonCaption = null;
            this.CopyToSTR.CancelButtonCaption = null;
            this.CopyToSTR.Caption = "Copy To STR";
            this.CopyToSTR.ConfirmationMessage = null;
            this.CopyToSTR.Id = "CopyToSTR";
            this.CopyToSTR.ToolTip = null;
            this.CopyToSTR.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.CopyToSTR_CustomizePopupWindowParams);
            this.CopyToSTR.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.CopyToSTR_Execute);
            // 
            // vwSAP_ITEM_AVAILABILITYController
            // 
            this.Actions.Add(this.CopyToSTR);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction CopyToSTR;
    }
}
