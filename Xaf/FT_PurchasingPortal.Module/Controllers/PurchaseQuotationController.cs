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
    public partial class PurchaseQuotationController : ViewController
    {
        GenController genCon;
        CopyController copyCon;
        public PurchaseQuotationController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            genCon = Frame.GetController<GenController>();
            copyCon = Frame.GetController<CopyController>();
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
        private void PQCopyFromPR_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            PurchaseRequest masterobject = (PurchaseRequest)View.CurrentObject;

            string ObjType = masterobject.DocType.BoCode;
            string BusinessPartner = masterobject.CardCode == null ? "" : masterobject.CardCode.BoKey;
            string doccur = masterobject.DocCur == null ? "" : masterobject.DocCur.BoKey;

            IObjectSpace newObjectSpace = Application.CreateObjectSpace(typeof(PurchaseRequestDetail));
            string listViewId = Application.FindLookupListViewId(typeof(PurchaseRequestDetail));
            CollectionSourceBase collectionSource = Application.CreateCollectionSource(newObjectSpace, typeof(PurchaseRequestDetail), listViewId);
            if (BusinessPartner == "")
                collectionSource.Criteria["filter01"] = CriteriaOperator.Parse("1=0");
            else
                collectionSource.Criteria["Filter01"] = CriteriaOperator.Parse("OpenQty > CopyQty and PurchaseRequest is not null and PurchaseRequest.DocStatus.CurrDocStatus in (?) and LineStatus in (?) and DocCur.BoKey=? and (LineVendor=? or LineVendor is null)", DocStatus.Accepted, LineStatusEnum.Open, doccur, BusinessPartner);

            e.View = Application.CreateListView(listViewId, collectionSource, true);
            //e.View = Application.CreateListView(typeof(PurchaseRequestDetail), true);

        }
        private void PQCopyFromPR_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseQuotation masterobject = (PurchaseQuotation)View.CurrentObject;
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
