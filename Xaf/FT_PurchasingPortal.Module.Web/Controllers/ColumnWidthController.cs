using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Web;
using DevExpress.ExpressApp;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;


namespace FT_PurchasingPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ColumnWidthController : ViewController<DevExpress.ExpressApp.ListView>
    {
        //string adjustablelistview = "ClaimTrxDetails_ClaimTrxDetailNote_ListView";
        DevExpress.ExpressApp.Actions.SimpleAction bestfit = null;

        public ColumnWidthController()
        {
            // Target required Views (via the TargetXXX properties) and create their Actions.
            this.bestfit = new DevExpress.ExpressApp.Actions.SimpleAction(this, "SyncWidth", DevExpress.Persistent.Base.PredefinedCategory.View);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            this.bestfit.Active.SetItemValue("Enabled", false);
            if (View.Editor is ASPxGridListEditor)
            {
                ASPxGridListEditor gridListEditor = View.Editor as ASPxGridListEditor;
                //if (adjustablelistview.Contains(View.Id))
                if (gridListEditor.AllowEdit)
                {
                    if (gridListEditor != null)
                    {
                        gridListEditor.CreateCustomGridViewDataColumn += gridListEditor_CreateCustomGridViewDataColumn;

                        this.bestfit.Active.SetItemValue("Enabled", true);
                        this.bestfit.Execute += Bestfit_Execute;
                    }
                }
            }
        }

        private void Bestfit_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            ASPxGridListEditor gridListEditor = View.Editor as ASPxGridListEditor;
            if (gridListEditor != null)
            {
                ASPxGridView gridView = gridListEditor.Grid;
                gridView.Settings.UseFixedTableLayout = true;
                gridView.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
                gridView.Width = Unit.Percentage(100);
                foreach (WebColumnBase column in gridView.Columns)
                {
                    IColumnInfo columnInfo = ((IDataItemTemplateInfoProvider)gridListEditor).GetColumnInfo(column);
                    if (columnInfo != null)
                    {
                        IModelColumn modelColumn = (IModelColumn)columnInfo.Model;
                        column.Width = Unit.Pixel(200);
                    }
                }

            }
        }

        void gridListEditor_CreateCustomGridViewDataColumn(object sender, CreateCustomGridViewDataColumnEventArgs e)
        {
            ASPxGridListEditor gridListEditor = (ASPxGridListEditor)sender;
            e.GridViewDataColumnInfo = new MyGridViewDataColumnInfo(e.ModelColumn, gridListEditor.Grid, gridListEditor.Model.DataAccessMode);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
            if (View.Editor is ASPxGridListEditor)
            {
                ASPxGridListEditor gridListEditor = View.Editor as ASPxGridListEditor;
                //if (adjustablelistview.Contains(View.Id))
                if (gridListEditor.AllowEdit)
                {
                    if (gridListEditor != null)
                    {
                        gridListEditor.Grid.SettingsResizing.ColumnResizeMode = ColumnResizeMode.Control;
                    }
                }
            }
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
            if (View.Editor is ASPxGridListEditor)
            {
                ASPxGridListEditor gridListEditor = View.Editor as ASPxGridListEditor;
                //if (adjustablelistview.Contains(View.Id))
                if (gridListEditor.AllowEdit)
                {
                    if (gridListEditor != null)
                    {
                        gridListEditor.CreateCustomGridViewDataColumn -= gridListEditor_CreateCustomGridViewDataColumn;
                        this.bestfit.Execute -= Bestfit_Execute;
                    }
                }
            }
        }
    }

    public class MyGridViewDataColumnInfo : GridViewDataColumnInfo
    {
        public MyGridViewDataColumnInfo(IModelColumn modelColumn, ASPxGridView gridView, CollectionSourceDataAccessMode dataAccessMode) : base(modelColumn, gridView, dataAccessMode) { }
        protected override ModelSynchronizer CreateModelSynchronizer(WebColumnBase column)
        {
            return new MyASPxGridViewColumnModelSynchronizer(CreateColumnWrapper(column), Model, DataAccessMode, IsProtectedContentColumn);
        }
    }
    public class MyASPxGridViewColumnModelSynchronizer : ASPxGridViewColumnModelSynchronizer
    {
        public MyASPxGridViewColumnModelSynchronizer(ColumnWrapper gridColumn, IModelColumn model, CollectionSourceDataAccessMode dataAccessMode, bool isProtectedContent)
            : base(gridColumn, model, dataAccessMode, isProtectedContent) { }
        public override void SynchronizeControlWidth()
        {
            GridViewDataColumn gridColumn = ((ASPxGridViewColumnWrapper)Control).Column;
            if (!gridColumn.Width.IsEmpty && gridColumn.Width.Type == UnitType.Pixel)
            {
                Model.Width = (int)gridColumn.Width.Value;
            }
        }
        protected override void ApplyModelCore()
        {
            base.ApplyModelCore();
            if (Model.Width > 0)
            {
                ((ASPxGridViewColumnWrapper)Control).Column.Width = Unit.Pixel(Model.Width);
            }
        }
    }

}
