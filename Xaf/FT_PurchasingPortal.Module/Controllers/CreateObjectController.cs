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
    public partial class CreateObjectController : ViewController
    {
        public CreateObjectController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (View is ListView && !View.IsRoot)
            {
                NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
                if (controller != null)
                {
                    //controller.NewObjectAction.Execute += NewObjectAction_Execute;
                    controller.ObjectCreated += Controller_ObjectCreated;
                }
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            if (View is ListView && !View.IsRoot)
            {
                NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
                if (controller != null)
                {
                    //controller.NewObjectAction.Execute -= NewObjectAction_Execute;
                    controller.ObjectCreated -= Controller_ObjectCreated;
                }
            }
            base.OnDeactivated();
        }
        private void Controller_ObjectCreated(object sender, ObjectCreatedEventArgs e)
        {
            ListView lv = ((ListView)View);
            if (lv.CollectionSource is PropertyCollectionSource)
            {
                PropertyCollectionSource collectionSource = (PropertyCollectionSource)lv.CollectionSource;
                if (collectionSource.MasterObject != null)
                {
                    int minvalue = -1;
                    int comparevalue = 0;
                    int maxvisorder = 1;
                    int comparevisorder = 0;
                    if (collectionSource.MasterObjectType == typeof(StockTransferRequest) && e.CreatedObject.GetType() == typeof(StockTransferRequestDetail))
                    {
                        StockTransferRequest masterobject = (StockTransferRequest)collectionSource.MasterObject;
                        StockTransferRequestDetail currentobject = (StockTransferRequestDetail)e.CreatedObject;
                        if (masterobject.StockTransferRequestDetail.Count > 0)
                        {
                            comparevalue = masterobject.StockTransferRequestDetail.Min(pp => pp.Oid);
                            comparevisorder = masterobject.StockTransferRequestDetail.Max(pp => pp.VisOrder);
                        }
                        if (comparevalue <= minvalue) minvalue = comparevalue - 1;
                        currentobject.Oid = minvalue;

                        if (comparevisorder >= maxvisorder) maxvisorder = comparevisorder + 1;
                        currentobject.VisOrder = maxvisorder;

                    }
                    else if (collectionSource.MasterObjectType == typeof(PurchaseOrder))
                    {
                        PurchaseOrder masterobject = (PurchaseOrder)collectionSource.MasterObject;
                        PurchaseOrderDetail currentobject = (PurchaseOrderDetail)e.CreatedObject;
                        if (masterobject.PurchaseOrderDetail.Count > 0)
                        {
                            comparevalue = masterobject.PurchaseOrderDetail.Min(pp => pp.Oid);
                            comparevisorder = masterobject.PurchaseOrderDetail.Max(pp => pp.VisOrder);
                        }
                        if (comparevalue <= minvalue) minvalue = comparevalue - 1;
                        currentobject.Oid = minvalue;

                        if (comparevisorder >= maxvisorder) maxvisorder = comparevisorder + 1;
                        currentobject.VisOrder = maxvisorder;
                        currentobject.DocCur = currentobject.Session.GetObjectByKey<vwCurrency>(masterobject.DocCur.BoKey);
                    }
                    else if (collectionSource.MasterObjectType == typeof(PurchaseRequest))
                    {
                        PurchaseRequest masterobject = (PurchaseRequest)collectionSource.MasterObject;
                        PurchaseRequestDetail currentobject = (PurchaseRequestDetail)e.CreatedObject;
                        if (masterobject.PurchaseRequestDetail.Count > 0)
                        {
                            comparevalue = masterobject.PurchaseRequestDetail.Min(pp => pp.Oid);
                            comparevisorder = masterobject.PurchaseRequestDetail.Max(pp => pp.VisOrder);
                        }
                        if (comparevalue <= minvalue) minvalue = comparevalue - 1;
                        currentobject.Oid = minvalue;

                        if (comparevisorder >= maxvisorder) maxvisorder = comparevisorder + 1;
                        currentobject.VisOrder = maxvisorder;
                        if (masterobject.DocCur != null)
                            currentobject.DocCur = currentobject.Session.GetObjectByKey<vwCurrency>(masterobject.DocCur.BoKey);
                    }
                }
            }
        }
    }
}
