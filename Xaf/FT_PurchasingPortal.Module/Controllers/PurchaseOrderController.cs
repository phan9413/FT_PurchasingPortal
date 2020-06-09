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
    public partial class PurchaseOrderController : ViewController
    {
        RecordsNavigationController recordnaviator;
        GenController genCon;
        CopyController copyCon;
        public PurchaseOrderController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(PurchaseOrder);
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
            this.CopyFromPR.Active.SetItemValue("Enabled", false);
            this.CopyToDO.Active.SetItemValue("Enabled", false);
            PurchaseOrder selectobject = (PurchaseOrder)View.CurrentObject;
            SystemUsers user = ObjectSpace.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);

            if (selectobject.DocStatus.CurrDocStatus == DocStatus.Draft)
            {
                this.CopyFromPR.Active.SetItemValue("Enabled", true);
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
                        this.CopyFromPR.Active.SetItemValue("Enabled", true);
                        break;
                }
                switch (selectobject.DocStatus.CurrDocStatus)
                {
                    case DocStatus.Draft:
                    case DocStatus.Submited:
                    case DocStatus.Cancelled:
                        break;
                    default:
                        if (user.Roles.Where(pp => pp.Name == DocTypeCodes.PurchaseDelivery).Count() > 0)
                        {
                            this.CopyToDO.Active.SetItemValue("Enabled", true);
                        }
                        break;
                }
            }

        }
        private void enableButton()
        {
            this.CopyFromPR.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
            this.CopyToDO.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.View);
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

        private void CopyFromPR_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            PurchaseOrder masterobject = (PurchaseOrder)View.CurrentObject;

            string ObjType = masterobject.DocType.BoCode;
            string BusinessPartner = masterobject.CardCode == null ? "" : masterobject.CardCode.BoKey;
            string doccur = masterobject.DocCur == null ? "" : masterobject.DocCur.BoKey;

            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(PurchaseRequestDetail));
            string listViewId = Application.FindLookupListViewId(typeof(PurchaseRequestDetail));
            CollectionSourceBase collectionSource = Application.CreateCollectionSource(newObjectSpace, typeof(PurchaseRequestDetail), listViewId);
            if (BusinessPartner == "")
                collectionSource.Criteria["filter01"] = CriteriaOperator.Parse("1=0");
            else
                collectionSource.Criteria["Filter01"] = CriteriaOperator.Parse("OpenQty > CopyQty and PurchaseRequest is not null and PurchaseRequest.DocStatus.CurrDocStatus in (?, ?, ?) and LineStatus in (?) and DocCur.BoKey=? and (LineVendor=? or LineVendor is null)", DocStatus.Accepted, DocStatus.Closed, DocStatus.Posted, LineStatusEnum.Open, doccur, BusinessPartner);

            e.View = Application.CreateListView(listViewId, collectionSource, true);
            //e.View = Application.CreateListView(typeof(PurchaseRequestDetail), true);
        }

        private void CopyFromPR_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseOrder masterobject = (PurchaseOrder)View.CurrentObject;
            IObjectSpace ios = View is DetailView ? ObjectSpace : Application.CreateObjectSpace();

            if (copyCon.CopyFromDocument(masterobject, (ListView)e.PopupWindow.View, ios))
            {
                masterobject.IsCopy = true;
                genCon.showMsg("Operation Done", "Item Copied.", InformationType.Success);
                return;
            }
        }

        private void CopyToDO_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            PurchaseOrder sObject = (PurchaseOrder)View.CurrentObject;
            IObjectSpace ios = Application.CreateObjectSpace();
            PurchaseDelivery tObject = ios.CreateObject<PurchaseDelivery>();

            if (copyCon.CopyToDocument(sObject, tObject, ios, (DetailView)View))
            {
                if (tObject.CardCode != null)
                    tObject.IsCopy = true;

                genCon.showMsg("Operation Done", "New Purchase Delivery copied. Please save it.", InformationType.Success);
                genCon.openNewView(ios, tObject, ViewEditMode.Edit);
                return;
            }

            genCon.showMsg("Operation Done", "No Open Item for copied.", InformationType.Info);
        }
    }
}
