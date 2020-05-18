using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using FT_PurchasingPortal.Module.BusinessObjects;

namespace FT_PurchasingPortal.Module.DatabaseUpdate {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            string temp = GeneralValues.hq;
            Company company = ObjectSpace.FindObject<Company>(CriteriaOperator.Parse("BoCode=?", temp));
            if (company == null)
            {
                company = ObjectSpace.CreateObject<Company>();
                company.BoCode = temp;
                company.BoName = temp;
                company.Save();
            }

            temp = DocTypeCodes.SalesQuotation;
            DocType doctype = ObjectSpace.FindObject<DocType>(new BinaryOperator("BoCode", temp));
            if (doctype == null)
            {
                doctype = ObjectSpace.CreateObject<DocType>();
                doctype.BoCode = temp;
                doctype.BoName = temp;
                doctype.Save();
            }
            temp = DocTypeCodes.SalesAgreement;
            doctype = ObjectSpace.FindObject<DocType>(new BinaryOperator("BoCode", temp));
            if (doctype == null)
            {
                doctype = ObjectSpace.CreateObject<DocType>();
                doctype.BoCode = temp;
                doctype.BoName = temp;
                doctype.Save();
            }
            temp = DocTypeCodes.SalesOrder;
            doctype = ObjectSpace.FindObject<DocType>(new BinaryOperator("BoCode", temp));
            if (doctype == null)
            {
                doctype = ObjectSpace.CreateObject<DocType>();
                doctype.BoCode = temp;
                doctype.BoName = temp;
                doctype.Save();
            }
            temp = DocTypeCodes.PurchaseOrder;
            doctype = ObjectSpace.FindObject<DocType>(new BinaryOperator("BoCode", temp));
            if (doctype == null)
            {
                doctype = ObjectSpace.CreateObject<DocType>();
                doctype.BoCode = temp;
                doctype.BoName = temp;
                doctype.Save();
            }
            temp = DocTypeCodes.PurchaseRequest;
            doctype = ObjectSpace.FindObject<DocType>(new BinaryOperator("BoCode", temp));
            if (doctype == null)
            {
                doctype = ObjectSpace.CreateObject<DocType>();
                doctype.BoCode = temp;
                doctype.BoName = temp;
                doctype.Save();
            }
            temp = DocTypeCodes.PurchaseDelivery;
            doctype = ObjectSpace.FindObject<DocType>(new BinaryOperator("BoCode", temp));
            if (doctype == null)
            {
                doctype = ObjectSpace.CreateObject<DocType>();
                doctype.BoCode = temp;
                doctype.BoName = temp;
                doctype.Save();
            }
            temp = DocTypeCodes.StockTransferRequest;
            doctype = ObjectSpace.FindObject<DocType>(new BinaryOperator("BoCode", temp));
            if (doctype == null)
            {
                doctype = ObjectSpace.CreateObject<DocType>();
                doctype.BoCode = temp;
                doctype.BoName = temp;
                doctype.Save();
            }

            //SystemUsers sampleUser = ObjectSpace.FindObject<SystemUsers>(new BinaryOperator("UserName", "User"));
            //if(sampleUser == null) {
            //    sampleUser = ObjectSpace.CreateObject<SystemUsers>();
            //    sampleUser.UserName = "User";
            //    sampleUser.SetPassword("");
            //    sampleUser.Company = company;
            //    sampleUser.IsUpdater = true;
            //    sampleUser.Employee.IsUpdater = true;
            //    sampleUser.Employee.FullName = "User";
            //}
            PermissionPolicyRole defaultRole = CreateDefaultRole();
            //defaultRole.PermissionPolicy = SecurityPermissionPolicy.ReadOnlyAllByDefault;
            //sampleUser.Roles.Add(defaultRole);

            // If a role with the Administrators name doesn't exist in the database, create this role
            PermissionPolicyRole adminRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Administrators"));
            if (adminRole == null)
            {
                adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                adminRole.Name = "Administrators";
                adminRole.IsAdministrative = true;
            }

            SystemUsers userAdmin = ObjectSpace.FindObject<SystemUsers>(new BinaryOperator("UserName", "Admin"));
            if(userAdmin == null) {
                userAdmin = ObjectSpace.CreateObject<SystemUsers>();
                userAdmin.UserName = "Admin";
                // Set a password if the standard authentication type is used
                userAdmin.SetPassword("");
                userAdmin.Company = company;
                userAdmin.Employee.FullName = "Admin";
                userAdmin.Roles.Add(adminRole);
                userAdmin.Save();
            }

            userAdmin = ObjectSpace.FindObject<SystemUsers>(new BinaryOperator("UserName", GeneralValues.FTAdmin));
            if (userAdmin == null)
            {
                userAdmin = ObjectSpace.CreateObject<SystemUsers>();
                userAdmin.UserName = GeneralValues.FTAdmin;
                // Set a password if the standard authentication type is used
                userAdmin.SetPassword(GeneralValues.FTAdminpwd);
                userAdmin.Company = company;
                userAdmin.Employee.FullName = GeneralValues.FTAdmin;
                userAdmin.Roles.Add(adminRole);
                userAdmin.Save();
            }
            createportalroles();

            ObjectSpace.CommitChanges(); //This line persists created object(s).
        }
        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
            //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
            //}
        }
        private PermissionPolicyRole CreateDefaultRole() {
            PermissionPolicyRole defaultRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Default"));
            if(defaultRole == null) {
                defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                defaultRole.Name = "Default";

				defaultRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.Read, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
				defaultRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
				defaultRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
				defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);

                defaultRole.AddObjectPermission<Employee>(SecurityOperations.ReadWriteAccess, "[SystemUser.Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddObjectPermission<SystemUsers>(SecurityOperations.ReadWriteAccess, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<Company>(SecurityOperations.Read, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<Departments>(SecurityOperations.Read, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<Divisions>(SecurityOperations.Read, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<Positions>(SecurityOperations.Read, SecurityPermissionState.Allow);
            }
            return defaultRole;
        }
        private void createGeneralPermission(PermissionPolicyRole newrole)
        {
            newrole.AddTypePermissionsRecursively<Approval>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<Budget>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<BudgetMaster>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<Company>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<CrReport>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<CrReportParam>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<Departments>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<Divisions>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<DocType>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<DocTypeSeries>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<DocTypeSeriesDoc>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<FilteringCriterion>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<FilteringCriterionRole>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<FTCrystalReportConn>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<FTModule>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<FTSAPConn>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<MyNotification>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<MyTask>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<Positions>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<Vehicle>(SecurityOperations.Read, SecurityPermissionState.Allow);

            newrole.AddTypePermissionsRecursively<vwAccounts>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwBillToAddress>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwBusinessPartners>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwContactPersons>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwCurrency>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwDimension1>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwDimension2>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwDimension3>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwDimension4>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwDimension5>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwItemMasters>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwPriceList>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwProjects>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwSalesPersons>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwSAP_ITEM_AVAILABILITY>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwShipToAddress>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwTaxes>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<vwWarehouses>(SecurityOperations.Read, SecurityPermissionState.Allow);

            newrole.AddTypePermissionsRecursively<Employee>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<SystemUsers>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<PermissionPolicyUser>(SecurityOperations.Read, SecurityPermissionState.Allow);
            newrole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Allow);

        }
        private void createportalroles()
        {
            PermissionPolicyRole newrole = null;
            string rolename = "";
            string viewpricerole = GeneralValues.viewpricestring;
            #region PurchaseDelivery
            rolename = DocTypeCodes.PurchaseDelivery;
            newrole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", rolename));
            if (newrole == null)
            {
                newrole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                newrole.Name = rolename;
                newrole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchasing/Items/PurchaseDelivery_ListView", SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseDelivery>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseDeliveryApp>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseDeliveryAppStage>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseDeliveryAppStatus>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseDeliveryAttachment>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseDeliveryDetail>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseDeliveryDetailUDF>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseDeliveryDoc>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseDeliveryDocStatus>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseDeliveryUDF>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
            }
            createGeneralPermission(newrole);

            rolename = rolename + viewpricerole;
            newrole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", rolename));
            if (newrole == null)
            {
                newrole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                newrole.Name = rolename;
            }
            #endregion

            #region PurchaseOrder
            rolename = DocTypeCodes.PurchaseOrder;
            newrole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", rolename));
            if (newrole == null)
            {
                newrole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                newrole.Name = rolename;
                newrole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchasing/Items/PurchaseOrder_ListView", SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseOrder>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseOrderApp>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseOrderAppStage>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseOrderAppStatus>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseOrderAttachment>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseOrderDetail>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseOrderDetailUDF>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseOrderDoc>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseOrderDocStatus>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseOrderUDF>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);

            }
            createGeneralPermission(newrole);

            rolename = rolename + viewpricerole;
            newrole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", rolename));
            if (newrole == null)
            {
                newrole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                newrole.Name = rolename;
            }
            #endregion

            #region PurchaseRequest
            rolename = DocTypeCodes.PurchaseRequest;
            newrole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", rolename));
            if (newrole == null)
            {
                newrole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                newrole.Name = rolename;
                newrole.AddNavigationPermission(@"Application/NavigationItems/Items/Purchasing/Items/PurchaseRequest_ListView", SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseRequest>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseRequestApp>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseRequestAppStage>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseRequestAppStatus>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseRequestAttachment>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseRequestDetail>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseRequestDetailUDF>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseRequestDoc>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseRequestDocStatus>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<PurchaseRequestUDF>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                
            }
            createGeneralPermission(newrole);

            rolename = rolename + viewpricerole;
            newrole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", rolename));
            if (newrole == null)
            {
                newrole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                newrole.Name = rolename;
            }
            #endregion

            #region StockTransferRequest
            rolename = DocTypeCodes.StockTransferRequest;
            newrole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", rolename));
            if (newrole == null)
            {
                newrole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                newrole.Name = rolename;
                newrole.AddNavigationPermission(@"Application/NavigationItems/Items/Inventory/Items/StockTransferRequest_ListView", SecurityPermissionState.Allow);
                newrole.AddNavigationPermission(@"Application/NavigationItems/Items/Inventory/Items/vwSAP_ITEM_AVAILABILITY_ListView", SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<StockTransferRequest>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<StockTransferRequestApp>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<StockTransferRequestAppStage>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<StockTransferRequestAppStatus>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<StockTransferRequestDetail>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<StockTransferRequestDetailUDF>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<StockTransferRequestDoc>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<StockTransferRequestDocStatus>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                newrole.AddTypePermissionsRecursively<StockTransferRequestUDF>(SecurityOperations.CRUDAccess, SecurityPermissionState.Allow);
                
            }
            createGeneralPermission(newrole);

            rolename = rolename + viewpricerole;
            newrole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", rolename));
            if (newrole == null)
            {
                newrole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                newrole.Name = rolename;
            }
            #endregion

        }
    }
}
