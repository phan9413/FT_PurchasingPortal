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
    public partial class vwSAP_ITEM_AVAILABILITYController : ViewController
    {
        GenController gen = null;
        public vwSAP_ITEM_AVAILABILITYController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetObjectType = typeof(vwSAP_ITEM_AVAILABILITY);
            TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            gen = Frame.GetController<GenController>();
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

        private void CopyToSTR_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace os = Application.CreateObjectSpace();
            DetailView dv = Application.CreateDetailView(os, os.CreateObject<CopyToAction>(), true);
            dv.ViewEditMode = DevExpress.ExpressApp.Editors.ViewEditMode.Edit;

            if (((ListView)View).SelectedObjects.Count <= 0)
            {
                ((CopyToAction)dv.CurrentObject).IsErr = true;
                ((CopyToAction)dv.CurrentObject).ActionMessage = "No Item selected.";
            }
            else
            {
                ((CopyToAction)dv.CurrentObject).IsErr = false;
                ((CopyToAction)dv.CurrentObject).ActionMessage = "Press OK to CONFIRM the action, else press Cancel.";
            }

            e.View = dv;
        }
        private void CopyToSTR_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            CopyToAction p = (CopyToAction)e.PopupWindow.View.CurrentObject;
            if (p.IsErr) return;

            string company = "";
            SystemUsers CreateUser = ObjectSpace.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            if (CreateUser.Company is null)
                company = "-";
            else
                company = CreateUser.Company.BoCode;

            int cnt = 0;
            StockTransferRequest obj = null;
            IObjectSpace os = null;
            foreach (vwSAP_ITEM_AVAILABILITY dtl in ((ListView)View).SelectedObjects)
            {
                cnt++;
                if (cnt == 1)
                {
                    os = Application.CreateObjectSpace();
                    obj = os.CreateObject<StockTransferRequest>();
                }
                StockTransferRequestDetail dtlobj = os.CreateObject<StockTransferRequestDetail>();
                dtlobj.Oid = cnt * -1;
                dtlobj.VisOrder = cnt;
                dtlobj.ItemCode = os.FindObject<vwItemMasters>(CriteriaOperator.Parse("ItemCode=? and CompanyCode=?", dtl.ItemCode, company));
                if (p.ParamAction == CopyToEnum.CopyAvailableQty)
                {
                    dtlobj.FromWhsCod = os.FindObject<vwWarehouses>(CriteriaOperator.Parse("WhsCode=? and CompanyCode=?", dtl.WhsCode, company));
                    //dtlobj.WhsCode = os.FindObject<vwWarehouses>(CriteriaOperator.Parse("WhsCode=? and CompanyCode=?", obj.ToWhsCode.WhsCode, company));
                    if (dtl.OnHand - dtl.IsCommited + dtl.OnOrder > 0)
                        dtlobj.Quantity = dtl.OnHand - dtl.IsCommited + dtl.OnOrder;
                }
                else if (p.ParamAction == CopyToEnum.CopyOnhandQty)
                {
                    dtlobj.FromWhsCod = os.FindObject<vwWarehouses>(CriteriaOperator.Parse("WhsCode=? and CompanyCode=?", dtl.WhsCode, company));
                    //dtlobj.WhsCode = os.FindObject<vwWarehouses>(CriteriaOperator.Parse("WhsCode=? and CompanyCode=?", obj.ToWhsCode.WhsCode, company));
                    dtlobj.Quantity = dtl.OnHand;
                }
                obj.StockTransferRequestDetail.Add(dtlobj);
            }
            gen.openNewView(os, obj, ViewEditMode.Edit);
            return;
        }
    }
}
