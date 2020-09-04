using System;
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
namespace FT_PurchasingPortal.Web
{
    public partial class FindDialogTemplateContent1 : TemplateContent, IXafPopupWindowControlContainer, ILookupPopupFrameTemplate
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Page.ClientScript.RegisterClientScriptResource(typeof(WebWindow), "DevExpress.ExpressApp.Web.Resources.JScripts.PopupTemplate.js");
            if (WebWindow.CurrentRequestWindow != null)
            {
                WebWindow.CurrentRequestWindow.PagePreRender += new EventHandler(CurrentRequestWindow_PagePreRender);
            }
        }
        private void CurrentRequestWindow_PagePreRender(object sender, EventArgs e)
        {
            WebWindow window = (WebWindow)sender;
            window.RegisterStartupScript("Init", "Init();");
        }
        protected override void OnUnload(EventArgs e)
        {
            if (WebWindow.CurrentRequestWindow != null)
            {
                WebWindow.CurrentRequestWindow.PagePreRender -= new EventHandler(CurrentRequestWindow_PagePreRender);
            }
            base.OnUnload(e);
        }
        public ICollection<IActionContainer> GetContainers()
        {
            return new IActionContainer[] { };
        }
        public void SetView(View view)
        {
        }
        public override void SetStatus(ICollection<string> statusMessages)
        {
        }
        public override IActionContainer DefaultContainer
        {
            get { return null; }
        }
        public override object ViewSiteControl
        {
            get { return VSC; }
        }
        public XafPopupWindowControl XafPopupWindowControl
        {
            get { return PopupWindowControl; }
        }
        public bool IsSearchEnabled
        {
            get { return SAC.Visible; }
            set { SAC.Visible = value; }
        }
        public void SetStartSearchString(string searchString) { }
        public void FocusFindEditor() { }
        public override void BeginUpdate()
        {
            base.BeginUpdate();
            SAC.BeginUpdate();
            OC.BeginUpdate();
            PopupActions.BeginUpdate();
        }
        public override void EndUpdate()
        {
            SAC.EndUpdate();
            OC.EndUpdate();
            PopupActions.EndUpdate();
            base.EndUpdate();
        }
    }
}
