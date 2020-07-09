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
    public partial class PurchaseDeliveryController : ViewController
    {
        RecordsNavigationController recordnaviator;
        GenController genCon;
        CopyController copyCon;
        public PurchaseDeliveryController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(PurchaseDelivery);
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
            this.CopyFromPO.Active.SetItemValue("Enabled", false);
            PurchaseDelivery selectobject = (PurchaseDelivery)View.CurrentObject;

            if (selectobject.DocStatus.CurrDocStatus == DocStatus.Draft)
            {
                this.CopyFromPO.Active.SetItemValue("Enabled", true);
            }
            else
            {
                switch (selectobject.DocStatus.CurrDocStatus)
                {
                    case DocStatus.Cancelled:
                    case DocStatus.Closed:
                    case DocStatus.Posted:
                    case DocStatus.PostedCancel:
                        break;
                    default:
                        this.CopyFromPO.Active.SetItemValue("Enabled", true);
                        break;
                }
            }

        }
        private void enableButton()
        {
            this.CopyFromPO.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
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

        private void CopyFromPO_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            PurchaseDelivery masterobject = (PurchaseDelivery)View.CurrentObject;

            string ObjType = masterobject.DocType.BoCode;
            string BusinessPartner = masterobject.CardCode == null ? "" : masterobject.CardCode.CardCode;

            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(PurchaseOrderDetail));
            string listViewId = Application.FindLookupListViewId(typeof(PurchaseOrderDetail));
            CollectionSourceBase collectionSource = Application.CreateCollectionSource(newObjectSpace, typeof(PurchaseOrderDetail), listViewId);
            if (BusinessPartner == "")
                collectionSource.Criteria["filter01"] = CriteriaOperator.Parse("1=0");
            else
                collectionSource.Criteria["Filter01"] = CriteriaOperator.Parse("PostVerNo = VerNo and OpenQty > CopyQty and PurchaseOrder is not null and PurchaseOrder.DocStatus.CurrDocStatus in (?, ?, ?) and LineStatus in (?) and (PurchaseOrder.CardCode=?)", DocStatus.Accepted, DocStatus.Closed, DocStatus.Posted, LineStatusEnum.Open, BusinessPartner);

            e.View = Application.CreateListView(listViewId, collectionSource, true);
            //e.View = Application.CreateListView(typeof(PurchaseRequestDetail), true);
        }

        private void CopyFromPO_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseDelivery masterobject = (PurchaseDelivery)View.CurrentObject;
            IObjectSpace ios = View is DetailView ? ObjectSpace : Application.CreateObjectSpace();

            if (copyCon.CopyFromDocument(masterobject, (ListView)e.PopupWindow.View, ios))
            {
                masterobject.IsCopy = true;
                genCon.showMsg("Operation Done", "Item Copied.", InformationType.Success);
                return;
            }
        }
    }
}
