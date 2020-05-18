using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using FT_PurchasingPortal.Module.BusinessObjects;

namespace FT_PurchasingPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class GenController : ViewController
    {
        public GenController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        public void openNewView(IObjectSpace os, object target, ViewEditMode viewmode)
        {
            ShowViewParameters svp = new ShowViewParameters();
            DetailView dv = Application.CreateDetailView(os, target);
            dv.ViewEditMode = viewmode;
            dv.IsRoot = true;
            svp.CreatedView = dv;

            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));

        }
        public void showMsg(string caption, string msg, InformationType msgtype)
        {
            MessageOptions options = new MessageOptions();
            options.Duration = 3000;
            //options.Message = string.Format("{0} task(s) have been successfully updated!", e.SelectedObjects.Count);
            options.Message = string.Format("{0}", msg);
            options.Type = msgtype;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = caption;
            options.Win.Type = WinMessageType.Flyout;
            Application.ShowViewStrategy.ShowMessage(options);

        }
        public bool GetCurrentUserViewPriceStatus()
        {
            bool rtn = false;
            IObjectSpace ios = Application.CreateObjectSpace();
            SystemUsers user = null;
            if (!string.IsNullOrEmpty(SecuritySystem.CurrentUserId.ToString()))
            {
                user = ios.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            }
            if (user != null)
            {
                string temp = View.ObjectTypeInfo.Type.ToString();
                string[] classname = temp.Split('.');
                temp = classname[classname.Length - 1];
                //temp = temp.Replace("Ajiya.Module.BusinessObjects.", "");
                temp = temp.Replace("Detail", "");
                rtn = user.CheckAccessVP(temp);
                //if (user.Roles.Where(p => p.Name == "C_" + temp + GeneralValues.viewpricestring).Count() > 0)
                //    rtn = true;

                //if (!rtn)
                //    rtn = user.Roles.Where(p => p.IsAdministrative).Count() > 0 ? true : false;

            }
            ios.Dispose();
            return rtn;
        }
    }
}
