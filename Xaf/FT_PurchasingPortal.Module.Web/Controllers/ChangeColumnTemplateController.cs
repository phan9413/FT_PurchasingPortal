using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Web;
using FT_PurchasingPortal.Module.BusinessObjects;
using DevExpress.Data;
using FT_PurchasingPortal.Module.Controllers;

namespace FT_PurchasingPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ChangeColumnTemplateController : ViewController<ListView>
    {
        GenController genCon;
        public ChangeColumnTemplateController()
        {
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            if (Frame.Context == TemplateContext.LookupControl || Frame.Context == TemplateContext.LookupWindow)
            {
                NewObjectViewController newcon = Frame.GetController<NewObjectViewController>();
                if (newcon != null)
                {
                    newcon.NewObjectAction.Active.SetItemValue("LookupNewListView", false);
                }
                DeleteObjectsViewController deleteCon = Frame.GetController<DeleteObjectsViewController>();
                if (deleteCon != null)
                {
                    deleteCon.DeleteAction.Active.SetItemValue("LookupDelListView", false);
                }
            }
        }
        void grid_DataBound(object sender, EventArgs e)
        {
            if (Frame.Context == TemplateContext.LookupControl || Frame.Context == TemplateContext.LookupWindow)
            {
                
                ASPxGridListEditor editor = ((ListView)View).Editor as ASPxGridListEditor;
                foreach (GridViewColumn column in editor.Grid.Columns)
                {
                    if (column is GridViewDataActionColumn)
                    {
                        if (((GridViewDataActionColumn)column).Action.Id == "Edit")
                        {
                            column.Visible = false;
                        }
                    }
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            genCon = Frame.GetController<GenController>();
            //Access and customize the target View control.

            var listEditor1 = View.Editor as ASPxGridListEditor;
            if (listEditor1 != null && listEditor1.Model.DataAccessMode == CollectionSourceDataAccessMode.Server)
            {
                foreach (var column in listEditor1.Grid.Columns)
                {
                    var commandColumn = column as GridViewCommandColumn;
                    if (commandColumn != null && commandColumn.ShowSelectCheckbox)
                    {
                        commandColumn.SelectAllCheckboxMode = GridViewSelectAllCheckBoxMode.Page;
                    }
                }
            }

            ASPxGridView grid = ((ListView)this.View).Editor.Control as ASPxGridView;
            if (grid != null)
            {
                grid.DataBound += grid_DataBound;
            }
        }
        protected override void OnDeactivated()
        {
            ASPxGridView grid = ((ListView)this.View).Editor.Control as ASPxGridView;
            if (grid != null)
            {
                grid.DataBound -= grid_DataBound;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
