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

            SystemUsers sampleUser = ObjectSpace.FindObject<SystemUsers>(new BinaryOperator("UserName", "User"));
            if(sampleUser == null) {
                sampleUser = ObjectSpace.CreateObject<SystemUsers>();
                sampleUser.UserName = "User";
                sampleUser.SetPassword("");
                sampleUser.Company = company;
                sampleUser.IsUpdater = true;
                sampleUser.Employee.IsUpdater = true;
                sampleUser.Employee.FullName = "User";
            }
            PermissionPolicyRole defaultRole = CreateDefaultRole();
            defaultRole.PermissionPolicy = SecurityPermissionPolicy.ReadOnlyAllByDefault;
            sampleUser.Roles.Add(defaultRole);

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
                userAdmin.IsUpdater = true;
                userAdmin.Employee.IsUpdater = true;
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
                userAdmin.IsUpdater = true;
                userAdmin.Employee.IsUpdater = true;
                userAdmin.Employee.FullName = GeneralValues.FTAdmin;
                userAdmin.Roles.Add(adminRole);
                userAdmin.Save();
            }


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
                defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
                defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
				defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);                
            }
            return defaultRole;
        }
    }
}
