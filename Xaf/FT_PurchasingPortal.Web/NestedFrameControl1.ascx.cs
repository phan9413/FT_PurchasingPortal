using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web;
namespace FT_PurchasingPortal.Web
{
    public partial class NestedFrameControl1 : NestedFrameControlBase, ISupportActionsToolbarVisibility
    {
        private bool toolBarVisibility = true;

        private void UpdateToolbarVisibility()
        {
            if (ToolBar != null)
            {
                ToolBar.Visible = toolBarVisibility;
            }
            if (ObjectsCreation != null)
            {
                ObjectsCreation.Visible = toolBarVisibility;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            UpdateToolbarVisibility();
        }
        protected override ContextActionsMenu CreateContextMenu()
        {
            return new ContextActionsMenu(this, "Edit", "RecordEdit", "ListView");
        }

        public override void SetStatus(ICollection<string> statusMessages)
        {
        }
        public override IActionContainer DefaultContainer
        {
            get { return ToolBar != null ? ToolBar.FindActionContainerById("View") : null; }
        }
        public override object ViewSiteControl
        {
            get { return viewSiteControl; }
        }
        public override void BeginUpdate()
        {
            base.BeginUpdate();
            if (ObjectsCreation != null)
            {
                ObjectsCreation.BeginUpdate();
            }
            if (ToolBar != null)
            {
                ToolBar.BeginUpdate();
            }
        }
        public override void EndUpdate()
        {
            if (ObjectsCreation != null)
            {
                ObjectsCreation.EndUpdate();
            }
            if (ToolBar != null)
            {
                ToolBar.EndUpdate();
            }
            base.EndUpdate();
        }
        void ISupportActionsToolbarVisibility.SetVisible(bool isVisible)
        {
            toolBarVisibility = isVisible;
            UpdateToolbarVisibility();
        }
    }
}
