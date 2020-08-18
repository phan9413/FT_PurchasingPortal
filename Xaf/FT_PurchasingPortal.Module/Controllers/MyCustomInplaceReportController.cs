using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FT_PurchasingPortal.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace FT_PurchasingPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class MyCustomInplaceReportController : ViewController
    {
        private SingleChoiceAction ShowInPlaceReportAction;
        public MyCustomInplaceReportController()
        {
            //Target required Views(via the TargetXXX properties) and create their Actions.

            ShowInPlaceReportAction = new SingleChoiceAction(
                this, "InPlaceReport", PredefinedCategory.Unspecified);
            ShowInPlaceReportAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            ShowInPlaceReportAction.ImageName = "BO_Report";
            ShowInPlaceReportAction.Caption = "Print Layout";
            ShowInPlaceReportAction.Execute += ShowInPlaceReportAction_Execute;
            TargetViewNesting = Nesting.Root;
        }



        private bool checkObjectType()
        {
            if (typeof(ClassDocument).IsAssignableFrom(View.ObjectTypeInfo.Type))
            { return true; }
            else if (typeof(ClassStockTransferDocument).IsAssignableFrom(View.ObjectTypeInfo.Type))
            { return true; }

            return false;
        }
        private void ShowInPlaceReportAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            if (checkObjectType())
            {
                IList<int> oids = new List<int>();
                string reportname = e.SelectedChoiceActionItem.Data.ToString();

                if (typeof(ClassDocument).IsAssignableFrom(View.ObjectTypeInfo.Type))
                {
                    if (View is ListView)
                    {
                        foreach (ClassDocument dtl in View.SelectedObjects)
                        {
                            oids.Add(dtl.Oid);
                        }
                    }
                    else if (View is DetailView)
                    {
                        oids.Add(((ClassDocument)View.CurrentObject).Oid);
                    }
                }
                else if (typeof(ClassStockTransferDocument).IsAssignableFrom(View.ObjectTypeInfo.Type))
                {
                    if (View is ListView)
                    {
                        foreach (ClassStockTransferDocument dtl in View.SelectedObjects)
                        {
                            oids.Add(dtl.Oid);
                        }
                    }
                    else if (View is DetailView)
                    {
                        oids.Add(((ClassStockTransferDocument)View.CurrentObject).Oid);
                    }
                }

                if (oids.Count > 0)
                {
                    string temp = "";
                    foreach (int dtl in oids)
                    {
                        if (temp == "")
                            temp = dtl.ToString();
                        else
                            temp += "," + dtl.ToString();
                    }
                    IObjectSpace objectSpace = ReportDataProvider.ReportObjectSpaceProvider.CreateObjectSpace(typeof(ReportDataV2));
                    IReportDataV2 reportData = objectSpace.FindObject<ReportDataV2>(CriteriaOperator.Parse("[DisplayName] = '" + reportname + "'"));
                    objectSpace.Dispose();
                    string handle = ReportDataProvider.ReportsStorage.GetReportContainerHandle(reportData);
                    Frame.GetController<ReportServiceController>().ShowPreview(handle, CriteriaOperator.Parse("Oid in (" + temp + ")"));
                }
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            PrintSelectionBaseController PrintSelectionController = Frame.GetController<PrintSelectionBaseController>();
            if (PrintSelectionController != null && PrintSelectionController.Actions.Count > 0)
            {

                if (PrintSelectionController.Actions["ShowInReportV2"] != null)
                {
                    if (checkObjectType())
                        PrintSelectionController.Actions["ShowInReportV2"].Active.SetItemValue("EnableAction", false);
                    //PrintSelectionController.Actions["ShowInReportV2"].Executing += FilterCopyFromController_Executing;
                }
            }

            ShowInPlaceReportAction.Items.Clear();
            if (checkObjectType())
            {

                bool found = false;
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                ChoiceActionItem setStatusItem;
                foreach (FilteringCriterion criterion in ObjectSpace.GetObjects<FilteringCriterion>())
                {
                    if (criterion.ObjectType == typeof(ReportDataV2))
                    {
                        if (criterion.Roles.Count > 0)
                        {
                            foreach (ReportDataV2 rpt in ObjectSpace.GetObjects<ReportDataV2>(CriteriaOperator.Parse(criterion.Criterion)))
                            {
                                if (rpt.DataTypeName == View.ObjectTypeInfo.Type.ToString())
                                {
                                    //if (criterion.Criterion.Contains("DataTypeCaption"))
                                    //{
                                        found = true;

                                        foreach (IPermissionPolicyRole role in user.Roles)
                                        {
                                            if (criterion.Roles.Where(p => p.FilterRole.Name == role.Name).Count() > 0)
                                            {
                                                if (rpt.IsInplaceReport)
                                                {
                                                    if (ShowInPlaceReportAction.Items.FindItemByID(rpt.DisplayName) == null)
                                                    {
                                                        setStatusItem = new ChoiceActionItem(rpt.DisplayName, rpt.DisplayName);
                                                        ShowInPlaceReportAction.Items.Add(setStatusItem);
                                                    }
                                                }
                                            }
                                        }
                                    //}
                                }
                            }
                        }
                        else
                        {
                            foreach (ReportDataV2 rpt in ObjectSpace.GetObjects<ReportDataV2>(CriteriaOperator.Parse(criterion.Criterion)))
                            {
                                if (rpt.DataTypeName == View.ObjectTypeInfo.Type.ToString())
                                {
                                    //if (criterion.Criterion.Contains("DataTypeCaption"))
                                    //{
                                        found = true;

                                        if (rpt.IsInplaceReport)
                                        {
                                            if (ShowInPlaceReportAction.Items.FindItemByID(rpt.DisplayName) == null)
                                            {
                                                if (ShowInPlaceReportAction.Items.FindItemByID(rpt.DisplayName) == null)
                                                {
                                                    setStatusItem = new ChoiceActionItem(rpt.DisplayName, rpt.DisplayName);
                                                    ShowInPlaceReportAction.Items.Add(setStatusItem);
                                                }
                                            }
                                        }
                                    //}
                                }
                            }
                        }
                    }
                }
                if (!found)
                {
                    foreach (ReportDataV2 rpt in ObjectSpace.GetObjects<ReportDataV2>())
                    {
                        if (rpt.DataTypeName == View.ObjectTypeInfo.Type.ToString() && rpt.IsInplaceReport)
                        {
                            if (ShowInPlaceReportAction.Items.FindItemByID(rpt.DisplayName) == null)
                            {
                                ShowInPlaceReportAction.Items.Add(new ChoiceActionItem(rpt.DisplayName, rpt.DisplayName));
                            }
                        }
                    }

                }

                if (View.GetType() == typeof(DetailView))
                {
                    this.ShowInPlaceReportAction.Active.SetItemValue("Enabled", ((DetailView)View).ViewEditMode == ViewEditMode.View);
                    ((DetailView)View).ViewEditModeChanged += MyCustomInplaceReportController_ViewEditModeChanged;
                }
            }
        }

        private void MyCustomInplaceReportController_ViewEditModeChanged(object sender, EventArgs e)
        {
            if (View.GetType() == typeof(DetailView))
            {
                this.ShowInPlaceReportAction.Active.SetItemValue("Enabled", ((DetailView)View).ViewEditMode == ViewEditMode.View);
            }
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            if (checkObjectType())
            {
                if (View.GetType() == typeof(DetailView))
                {
                    ((DetailView)View).ViewEditModeChanged -= MyCustomInplaceReportController_ViewEditModeChanged;
                }
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
