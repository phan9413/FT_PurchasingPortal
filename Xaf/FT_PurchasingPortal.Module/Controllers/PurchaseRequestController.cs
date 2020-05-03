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
    public partial class PurchaseRequestController : ViewController
    {
        RecordsNavigationController recordnaviator;
        GenController genCon;
        CopyController copyCon;
        public PurchaseRequestController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(PurchaseRequest);
            TargetViewType = ViewType.DetailView;

        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (View is DetailView)
            {
                ((DetailView)View).ViewEditModeChanged += GenController_ViewEditModeChanged;

                recordnaviator = Frame.GetController<RecordsNavigationController>();
                if (recordnaviator != null)
                {
                    recordnaviator.PreviousObjectAction.Executed += PreviousObjectAction_Executed;
                    recordnaviator.NextObjectAction.Executed += NextObjectAction_Executed;
                }
                resetButton();
                enableButton();
            }
        }
        private void GenController_ViewEditModeChanged(object sender, EventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                resetButton();
                enableButton();
            }
        }
        private void PreviousObjectAction_Executed(object sender, ActionBaseEventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                resetButton();
            }
        }
        private void NextObjectAction_Executed(object sender, ActionBaseEventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                resetButton();
            }
        }
        public void resetButton()
        {
            this.CopyToPO.Active.SetItemValue("Enabled", false);
            PurchaseRequest selectobject = (PurchaseRequest)View.CurrentObject;

            switch (selectobject.DocStatus.CurrDocStatus)
            {
                case DocStatus.Draft:
                case DocStatus.Submited:
                case DocStatus.Cancelled:
                    break;
                default:
                    this.CopyToPO.Active.SetItemValue("Enabled", true);
                    break;
            }
        }
        private void enableButton()
        {
            this.CopyToPO.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            genCon = Frame.GetController<GenController>();
            copyCon = Frame.GetController<CopyController>();
        }
        protected override void OnDeactivated()
        {
            if (View.GetType() == typeof(DetailView))
            {
                ((DetailView)View).ViewEditModeChanged -= GenController_ViewEditModeChanged;
                if (recordnaviator != null)
                {
                    recordnaviator.PreviousObjectAction.Executed -= PreviousObjectAction_Executed;
                    recordnaviator.NextObjectAction.Executed -= NextObjectAction_Executed;
                }
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }


        private void CopyToPO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            PurchaseRequest sObject = (PurchaseRequest)View.CurrentObject;
            IObjectSpace ios = Application.CreateObjectSpace();
            PurchaseOrder tObject = ios.CreateObject<PurchaseOrder>();

            if (copyCon.CopyToDocument(sObject, tObject, ios, (DetailView)View))
            {
                if (tObject.CardCode != null)
                    tObject.IsCopy = true;

                genCon.showMsg("Operation Done", "New Sales Delivery copied. Please save it.", InformationType.Success);
                genCon.openNewView(ios, tObject, ViewEditMode.Edit);
                return;
            }

            genCon.showMsg("Operation Done", "No Open Item for copied.", InformationType.Info);
        }
    }
}
