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
    public partial class FilteringCriterionController : ViewController
    {
        private SingleChoiceAction filteringCriterionAction;
        public FilteringCriterionController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
            TargetViewType = ViewType.ListView;
            filteringCriterionAction = new SingleChoiceAction(
                this, "FilteringCriterion", PredefinedCategory.Filters);
            filteringCriterionAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.FilteringCriterionAction_Execute);

        }
        private void FilteringCriterionAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            ((ListView)View).CollectionSource.BeginUpdateCriteria();
            ((ListView)View).CollectionSource.Criteria.Clear();
            ((ListView)View).CollectionSource.Criteria[e.SelectedChoiceActionItem.Caption] =
                CriteriaEditorHelper.GetCriteriaOperator(
                e.SelectedChoiceActionItem.Data as string, View.ObjectTypeInfo.Type, ObjectSpace);
            ((ListView)View).CollectionSource.EndUpdateCriteria();
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            if (View is ListView)
            {
                bool filterrolefound = false;
                string description1st = "";
                string description = "";

                filteringCriterionAction.Items.Clear();
                foreach (FilteringCriterion criterion in ObjectSpace.GetObjects<FilteringCriterion>())
                {
                    if (criterion.ObjectType.IsAssignableFrom(View.ObjectTypeInfo.Type))
                    {
                        if (criterion.Roles.Count <= 0)
                        {
                            if (criterion.Description == "-")
                            {
                                description = "Default";
                                description1st = description;
                            }
                            else
                                description = criterion.Description;

                            filteringCriterionAction.Items.Add(
                                new ChoiceActionItem(description, criterion.Criterion));

                            if (description1st == "") description1st = description;
                        }
                        else
                        {
                            filterrolefound = false;
                            foreach (IPermissionPolicyRole role in user.Roles)
                            {
                                if (!filterrolefound)
                                {
                                    if (criterion.Roles.Where(p => p.FilterRole.Name == role.Name).Count() > 0)
                                    {
                                        filterrolefound = true;
                                        if (criterion.Description == "-")
                                        {
                                            description = "Default";
                                            description1st = description;
                                        }
                                        else
                                            description = criterion.Description;

                                        filteringCriterionAction.Items.Add(
                                            new ChoiceActionItem(description, criterion.Criterion));

                                        if (description1st == "") description1st = description;

                                    }
                                }
                            }
                        }
                    }
                }

                filteringCriterionAction.SelectedItem = filteringCriterionAction.Items.FindItemByID(description1st);
                if (filteringCriterionAction.SelectedItem != null)
                    filteringCriterionAction.DoExecute(filteringCriterionAction.SelectedItem);
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
            base.OnDeactivated();
        }
    }
}
