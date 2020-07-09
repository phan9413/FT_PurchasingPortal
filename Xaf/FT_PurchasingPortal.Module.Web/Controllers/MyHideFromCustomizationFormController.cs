
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
using FT_PurchasingPortal.Module.Controllers;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using System.Web.UI;
using System;
using FT_PurchasingPortal.Module.BusinessObjects;
// ...
namespace FT_PurchasingPortal.Module.Web.Controllers
{
    public class HideFromCustomizationFormController : ViewController<ListView>, IModelExtender
    {
        GenController genCon;
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add(typeof(IModelMember),
                typeof(IModelMemberShowInCustomizationForm));
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            genCon = Frame.GetController<GenController>();
            ASPxGridView grid = ((ListView)this.View).Editor.Control as ASPxGridView;
            if (grid != null)
            {
                grid.DataBound += Grid_DataBound;
            }
            if (typeof(ClassDocument).IsAssignableFrom(View.ObjectTypeInfo.Type)
                || typeof(ClassDocumentDetail).IsAssignableFrom(View.ObjectTypeInfo.Type)
                || typeof(ClassStockTransferDocument).IsAssignableFrom(View.ObjectTypeInfo.Type)
                || typeof(ClassStockTransferDocumentDetail).IsAssignableFrom(View.ObjectTypeInfo.Type)
                )
            {
                if (!genCon.GetCurrentUserViewPriceStatus())
                {
                    ColumnsListEditor listEditor = View.Editor as ColumnsListEditor;

                    if (listEditor != null)
                    {
                        foreach (ColumnWrapper column in listEditor.Columns)
                        {
                            if (!string.IsNullOrEmpty(column.Id))
                            {
                                IModelColumn mycol = View.Model.Columns[column.Id];
                                if (mycol != null)
                                {
                                    string temp = View.Model.Columns[column.Id].ModelMember.PropertyEditorType.Name;
                                    //IModelPropertyEditor prop = (IModelPropertyEditor)View.Model.Columns[column.Id].ModelMember.PropertyEditorType.GetType();
                                    IModelMemberShowInCustomizationForm modelMember = (IModelMemberShowInCustomizationForm)View.Model.Columns[column.Id].ModelMember;
                                    //if (temp == "MyDecPropertyEditorVP" || temp == "MyDouPropertyEditorVP")
                                    if (temp.Contains("PropertyEditorVP"))
                                    {
                                        column.ShowInCustomizationForm = false;
                                    }
                                    else
                                    {
                                        column.ShowInCustomizationForm = modelMember.ShowInCustomizationForm;
                                    }

                                    //switch (column.Id)
                                    //{
                                    //    case "UnitPrice":
                                    //    case "LineTotal":
                                    //    case "TotalBeforeDiscount":
                                    //    case "Discount":
                                    //    case "TripCost":
                                    //    case "GST":
                                    //    case "GrandTotal":
                                    //        column.ShowInCustomizationForm = false;
                                    //        break;
                                    //    default:
                                    //        column.ShowInCustomizationForm = modelMember.ShowInCustomizationForm;
                                    //        break;
                                    //}
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Grid_DataBound(object sender, System.EventArgs e)
        {
            genCon = Frame.GetController<GenController>();
            if (typeof(ClassDocument).IsAssignableFrom(View.ObjectTypeInfo.Type)
                || typeof(ClassDocumentDetail).IsAssignableFrom(View.ObjectTypeInfo.Type)
                || typeof(ClassStockTransferDocument).IsAssignableFrom(View.ObjectTypeInfo.Type)
                || typeof(ClassStockTransferDocumentDetail).IsAssignableFrom(View.ObjectTypeInfo.Type)
                )
            {
                if (!genCon.GetCurrentUserViewPriceStatus())
                {
                    ASPxGridListEditor listEditor = View.Editor as ASPxGridListEditor;

                    if (listEditor != null)
                    {
                        foreach (GridViewColumn column in listEditor.Grid.VisibleColumns)
                        {
                            if (column is GridViewDataColumn)
                            {
                                GridViewDataColumn col = (GridViewDataColumn)column;
                                if (!string.IsNullOrEmpty(col.FieldName))
                                {
                                    try
                                    {
                                        IModelColumn mycol = View.Model.Columns[col.FieldName];
                                        if (mycol != null)
                                        {
                                            string temp = View.Model.Columns[col.FieldName].ModelMember.PropertyEditorType.Name;
                                            //if (temp == "MyDecPropertyEditorVP" || temp == "MyDouPropertyEditorVP")
                                            if (temp.Contains("PropertyEditorVP"))
                                            {
                                                column.Visible = false;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnDeactivated()
        {
            ASPxGridView grid = ((ListView)this.View).Editor.Control as ASPxGridView;
            if (grid != null)
            {
                grid.DataBound -= Grid_DataBound;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
    public interface IModelMemberShowInCustomizationForm : IModelNode
    {
        [DefaultValue(true)]
        bool ShowInCustomizationForm { get; set; }
    }

}