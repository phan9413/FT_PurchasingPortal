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
        ClassDocumentDetail sDtl;
        ClassStockTransferDocumentDetail sDtlS;
        GenController genCon;
        CopyController copyCon;
        public CreateObjectController()
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

            this.DuplicateDetail.Active.SetItemValue("Enabled", false);
            
            if (View is ListView && !View.IsRoot)
            {
                NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
                if (controller != null)
                {
                    //controller.NewObjectAction.Execute += NewObjectAction_Execute;
                    controller.ObjectCreated += Controller_ObjectCreated;
                }

                if (typeof(ClassStockTransferDocumentDetail).IsAssignableFrom(View.ObjectTypeInfo.Type))
                    this.DuplicateDetail.Active.SetItemValue("Enabled", true);
                if (typeof(ClassDocumentDetail).IsAssignableFrom(View.ObjectTypeInfo.Type))
                    this.DuplicateDetail.Active.SetItemValue("Enabled", true);
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
                    int minvalue = 0;
                    int maxvisorder = 0;

                    if (collectionSource.MasterObjectType == typeof(StockTransferRequest) && e.CreatedObject.GetType() == typeof(StockTransferRequestDetail))
                    {
                        StockTransferRequest masterobject = (StockTransferRequest)collectionSource.MasterObject;
                        StockTransferRequestDetail currentobject = (StockTransferRequestDetail)e.CreatedObject;

                        copyCon.GetStockDetailClassVisOrder(masterobject, ref minvalue, ref maxvisorder);

                        currentobject.Oid = minvalue;
                        currentobject.VisOrder = maxvisorder;

                        if (sDtlS != null)
                        {
                            copyCon.copyClassStockTransferDocumentDetail(sDtlS, currentobject, e.ObjectSpace);
                        }
                    }
                    else if (collectionSource.MasterObjectType == typeof(PurchaseOrder))
                    {
                        PurchaseOrder masterobject = (PurchaseOrder)collectionSource.MasterObject;
                        PurchaseOrderDetail currentobject = (PurchaseOrderDetail)e.CreatedObject;

                        copyCon.GetDetailClassVisOrder(masterobject, ref minvalue, ref maxvisorder);

                        currentobject.Oid = minvalue;
                        currentobject.VisOrder = maxvisorder;
                        currentobject.DocCur = currentobject.Session.GetObjectByKey<vwCurrency>(masterobject.DocCur.BoKey);

                        if (sDtl != null)
                        {
                            copyCon.copyClassDocumentDetail(sDtl, currentobject, e.ObjectSpace);
                        }
                    }
                    else if (collectionSource.MasterObjectType == typeof(PurchaseRequest))
                    {
                        PurchaseRequest masterobject = (PurchaseRequest)collectionSource.MasterObject;
                        PurchaseRequestDetail currentobject = (PurchaseRequestDetail)e.CreatedObject;

                        copyCon.GetDetailClassVisOrder(masterobject, ref minvalue, ref maxvisorder);

                        currentobject.Oid = minvalue;
                        currentobject.VisOrder = maxvisorder;

                        if (masterobject.DocCur != null)
                            currentobject.DocCur = currentobject.Session.GetObjectByKey<vwCurrency>(masterobject.DocCur.BoKey);
                        
                        if (sDtl != null)
                        {
                            copyCon.copyClassDocumentDetail(sDtl, currentobject, e.ObjectSpace);
                        }
                    }
                    else if (collectionSource.MasterObjectType == typeof(PurchaseDelivery))
                    {
                        PurchaseDelivery masterobject = (PurchaseDelivery)collectionSource.MasterObject;
                        PurchaseDeliveryDetail currentobject = (PurchaseDeliveryDetail)e.CreatedObject;

                        copyCon.GetDetailClassVisOrder(masterobject, ref minvalue, ref maxvisorder);

                        currentobject.Oid = minvalue;
                        currentobject.VisOrder = maxvisorder;

                        if (masterobject.DocCur != null)
                            currentobject.DocCur = currentobject.Session.GetObjectByKey<vwCurrency>(masterobject.DocCur.BoKey);
                        
                        if (sDtl != null)
                        {
                            copyCon.copyClassDocumentDetail(sDtl, currentobject, e.ObjectSpace);
                        }
                    }
                    else if (collectionSource.MasterObjectType == typeof(PurchaseReturn))
                    {
                        PurchaseReturn masterobject = (PurchaseReturn)collectionSource.MasterObject;
                        PurchaseReturnDetail currentobject = (PurchaseReturnDetail)e.CreatedObject;

                        copyCon.GetDetailClassVisOrder(masterobject, ref minvalue, ref maxvisorder);

                        currentobject.Oid = minvalue;
                        currentobject.VisOrder = maxvisorder;

                        if (masterobject.DocCur != null)
                            currentobject.DocCur = currentobject.Session.GetObjectByKey<vwCurrency>(masterobject.DocCur.BoKey);

                        if (sDtl != null)
                        {
                            copyCon.copyClassDocumentDetail(sDtl, currentobject, e.ObjectSpace);
                        }
                    }
                    else if (collectionSource.MasterObjectType == typeof(PurchaseQuotation))
                    {
                        PurchaseQuotation masterobject = (PurchaseQuotation)collectionSource.MasterObject;
                        PurchaseQuotationDetail currentobject = (PurchaseQuotationDetail)e.CreatedObject;

                        copyCon.GetDetailClassVisOrder(masterobject, ref minvalue, ref maxvisorder);

                        currentobject.Oid = minvalue;
                        currentobject.VisOrder = maxvisorder;

                        if (masterobject.DocCur != null)
                            currentobject.DocCur = currentobject.Session.GetObjectByKey<vwCurrency>(masterobject.DocCur.BoKey);

                        if (sDtl != null)
                        {
                            copyCon.copyClassDocumentDetail(sDtl, currentobject, e.ObjectSpace);
                        }
                    }
                }
            }

            sDtl = null;
            sDtlS = null;
        }

        private void DuplicateDetail_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (!ObjectSpace.IsModified)
            {
                genCon.showMsg("", "View Mode cannot proceed.", InformationType.Error);
                return;
            }
            if (((ListView)View).SelectedObjects.Count != 1)
            {
                genCon.showMsg("", "Please select only 1 item.", InformationType.Error);
                return;
            }

            IObjectSpace os = Application.CreateObjectSpace();
            if (typeof(ClassDocumentDetail).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                foreach (ClassDocumentDetail dtl in ((ListView)View).SelectedObjects)
                {
                    if (View.ObjectTypeInfo.Type == typeof(PurchaseRequestDetail))
                    {
                        sDtl = dtl;
                    }
                    if (View.ObjectTypeInfo.Type == typeof(PurchaseOrderDetail))
                    {
                        sDtl = dtl;
                    }
                    if (View.ObjectTypeInfo.Type == typeof(PurchaseDeliveryDetail))
                    {
                        sDtl = dtl;
                    }
                    if (View.ObjectTypeInfo.Type == typeof(PurchaseReturnDetail))
                    {
                        sDtl = dtl;
                    }
                    if (View.ObjectTypeInfo.Type == typeof(PurchaseQuotationDetail))
                    {
                        sDtl = dtl;
                    }
                    //copyCon.copyClassDocumentDetail(dtl, sDtl, os);
                }
            }
            if (typeof(ClassStockTransferDocumentDetail).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                foreach (ClassStockTransferDocumentDetail dtl in ((ListView)View).SelectedObjects)
                {
                    if (View.ObjectTypeInfo.Type == typeof(StockTransferRequestDetail))
                    {
                        sDtlS = dtl;
                    }
                    //copyCon.copyClassStockTransferDocumentDetail(dtl, sDtlS, os);
                }
            }
            NewObjectViewController newobjectcon = Frame.GetController<NewObjectViewController>();
            newobjectcon.NewObjectAction.DoExecute(newobjectcon.NewObjectAction.Items.FirstActiveItem);


        }
    }
}
