using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web;
using System.Web.Routing;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Web;
using DevExpress.Web;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.XtraReports.UI;

namespace FT_PurchasingPortal.Web {
    public class Global : System.Web.HttpApplication {
        public Global() {
            InitializeComponent();
        }
        protected void Application_Start(Object sender, EventArgs e) {
            RouteTable.Routes.RegisterXafRoutes();
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
#if EASYTEST
            DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = true;
#endif
        }
        protected void Session_Start(Object sender, EventArgs e) {
            Tracing.Initialize();
            WebApplication.SetInstance(Session, new FT_PurchasingPortalAspNetApplication());
            SecurityStrategy security = (SecurityStrategy)WebApplication.Instance.Security;
            security.RegisterXPOAdapterProviders();
            DevExpress.ExpressApp.Web.Templates.DefaultVerticalTemplateContentNew.ClearSizeLimit();
            WebApplication.Instance.SwitchToNewStyle();

            #region GeneralSettings
            string temp = "";

            temp = ConfigurationManager.AppSettings["LiveWithPost"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.LiveWithPost = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                FT_PurchasingPortal.Module.GeneralValues.LiveWithPost = true;

            temp = ConfigurationManager.AppSettings["EmailSend"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.EmailSend = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                FT_PurchasingPortal.Module.GeneralValues.EmailSend = true;

            FT_PurchasingPortal.Module.GeneralValues.EmailHost = ConfigurationManager.AppSettings["EmailHost"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.EmailHostDomain = ConfigurationManager.AppSettings["EmailHostDomain"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.EmailPort = ConfigurationManager.AppSettings["EmailPort"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.Email = ConfigurationManager.AppSettings["Email"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.EmailPassword = ConfigurationManager.AppSettings["EmailPassword"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.EmailName = ConfigurationManager.AppSettings["EmailName"].ToString();

            FT_PurchasingPortal.Module.GeneralValues.TempFolder = ConfigurationManager.AppSettings["TempFolder"].ToString();

            temp = ConfigurationManager.AppSettings["EmailSSL"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.EmailSSL = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                FT_PurchasingPortal.Module.GeneralValues.EmailSSL = true;

            temp = ConfigurationManager.AppSettings["EmailUseDefaultCredential"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.EmailUseDefaultCredential = false;
            if (temp.ToUpper() == "Y" || temp.ToUpper() == "YES" || temp.ToUpper() == "TRUE" || temp == "1")
                FT_PurchasingPortal.Module.GeneralValues.EmailUseDefaultCredential = true;

            FT_PurchasingPortal.Module.GeneralValues.DeliveryMethod = ConfigurationManager.AppSettings["DeliveryMethod"].ToString();

            FT_PurchasingPortal.Module.GeneralValues.defwh = ConfigurationManager.AppSettings["defwh"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.definputtax = ConfigurationManager.AppSettings["definputtax"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.defoutputtax = ConfigurationManager.AppSettings["defoutputtax"].ToString();
            FT_PurchasingPortal.Module.GeneralValues.deflocalcurrency = ConfigurationManager.AppSettings["deflocalcurrency"].ToString();

            #endregion
            WebApplication.Instance.CustomizeFormattingCulture += Instance_CustomizeFormattingCulture;

            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#if EASYTEST
            if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
                WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
            }
#endif
#if DEBUG
            if(System.Diagnostics.Debugger.IsAttached && WebApplication.Instance.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
                WebApplication.Instance.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }
#endif
            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();
        }

        // solve get SecuritySystem.CurrentUserID return emtpy - start
        public class ViewerOperationLogger : WebDocumentViewerOperationLogger
        {
            public override Action BuildStarting(string reportId, XtraReport report, ReportBuildProperties buildProperties)
            {
                report.CreateDocument();
                return null;
            }
        }
        // solve get SecuritySystem.CurrentUserID return emtpy - end
        private void Instance_CustomizeFormattingCulture(object sender, CustomizeFormattingCultureEventArgs e)
        {
            //e.FormattingCulture.NumberFormat.CurrencySymbol = "MYR";
            e.FormattingCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        }

        protected void Application_BeginRequest(Object sender, EventArgs e) {
        }
        protected void Application_EndRequest(Object sender, EventArgs e) {
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
        }
        protected void Application_Error(Object sender, EventArgs e) {
            ErrorHandling.Instance.ProcessApplicationError();
        }
        protected void Session_End(Object sender, EventArgs e) {
            WebApplication.LogOff(Session);
            WebApplication.DisposeInstance(Session);
        }
        protected void Application_End(Object sender, EventArgs e) {
        }
        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}
