using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using FT_PurchasingPortal.Module.BusinessObjects;

namespace SAP_Integration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static System.Threading.Mutex mutex = null;
        [STAThread]
        static void Main()
        {
            const string appName = "SAP_Integration";
            bool createdNew;

            mutex = new System.Threading.Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                Application.Exit();
                return;
            }

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form());
            //Application.Exit();
            //Code obj = new Code();


            //GC.KeepAlive(mutex);                // important!

            RegisterEntities();
            AuthenticationStandard authentication = new AuthenticationStandard();
            SecurityStrategyComplex security = new SecurityStrategyComplex(typeof(PermissionPolicyUser), typeof(PermissionPolicyRole), authentication);
            security.RegisterXPOAdapterProviders();
            string connectionString = ConfigurationManager.ConnectionStrings["DataSourceConnectionString"].ConnectionString;
            IObjectSpaceProvider objectSpaceProvider = new SecuredObjectSpaceProvider(security, connectionString, null);

            DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = true;
            DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = false;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainForm = new MainForm(security, objectSpaceProvider);

            mainForm.defuserid = ConfigurationManager.AppSettings["DataSourceUserID"].ToString();
            mainForm.defpassword = ConfigurationManager.AppSettings["DataSourcePassword"].ToString();
            string temp = ConfigurationManager.AppSettings["AutoPostAfterLogin"].ToString().ToUpper();
            if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                mainForm.autopostafterlogin = true;
            else
                mainForm.autopostafterlogin = false;

            temp = "";
            temp = ConfigurationManager.AppSettings["AutoLogin"].ToString().ToUpper();
            if (temp == "Y" || temp == "YES" || temp == "TRUE" || temp == "1")
                mainForm.autologin = true;
            else
                mainForm.autologin = false;

            Application.Run(mainForm);

        }
        private static void RegisterEntities()
        {
            XpoTypesInfoHelper.GetXpoTypeInfoSource();

            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseReturn));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseReturnApp));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseReturnAppStage));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseReturnAppStatus));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseReturnAttachment));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseReturnDetail));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseReturnDetailUDF));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseReturnDoc));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseReturnDocStatus));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseReturnUDF));

            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseDelivery));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseDeliveryApp));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseDeliveryAppStage));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseDeliveryAppStatus));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseDeliveryAttachment));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseDeliveryDetail));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseDeliveryDetailUDF));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseDeliveryDoc));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseDeliveryDocStatus));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseDeliveryUDF));

            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseRequest));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseRequestApp));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseRequestAppStage));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseRequestAppStatus));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseRequestAttachment));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseRequestDetail));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseRequestDetailUDF));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseRequestDoc));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseRequestDocStatus));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseRequestUDF));

            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrder));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrderApp));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrderAppStage));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrderAppStatus));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrderAttachment));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrderDetail));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrderDetailUDF));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrderDoc));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrderDocStatus));
            XafTypesInfo.Instance.RegisterEntity(typeof(PurchaseOrderUDF));

            XafTypesInfo.Instance.RegisterEntity(typeof(StockTransferRequest));
            XafTypesInfo.Instance.RegisterEntity(typeof(StockTransferRequestApp));
            XafTypesInfo.Instance.RegisterEntity(typeof(StockTransferRequestAppStage));
            XafTypesInfo.Instance.RegisterEntity(typeof(StockTransferRequestAppStatus));
            XafTypesInfo.Instance.RegisterEntity(typeof(StockTransferRequestDetail));
            XafTypesInfo.Instance.RegisterEntity(typeof(StockTransferRequestDetailUDF));
            XafTypesInfo.Instance.RegisterEntity(typeof(StockTransferRequestDoc));
            XafTypesInfo.Instance.RegisterEntity(typeof(StockTransferRequestDocStatus));
            XafTypesInfo.Instance.RegisterEntity(typeof(StockTransferRequestUDF));

            XafTypesInfo.Instance.RegisterEntity(typeof(Employee));
            XafTypesInfo.Instance.RegisterEntity(typeof(SystemUsers));
            XafTypesInfo.Instance.RegisterEntity(typeof(PermissionPolicyUser));
            XafTypesInfo.Instance.RegisterEntity(typeof(PermissionPolicyRole));
        }
    }
}
