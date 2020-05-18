namespace FT_PurchasingPortal.Module.Controllers
{
    partial class CreateObjectController
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
            this.DuplicateDetail = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // DuplicateDetail
            // 
            this.DuplicateDetail.Caption = "Duplicate";
            this.DuplicateDetail.ConfirmationMessage = null;
            this.DuplicateDetail.Id = "DuplicateDetail";
            this.DuplicateDetail.ToolTip = null;
            this.DuplicateDetail.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.DuplicateDetail_Execute);
            // 
            // CreateObjectController
            // 
            this.Actions.Add(this.DuplicateDetail);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction DuplicateDetail;
    }
}
