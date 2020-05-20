using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT_PurchasingPortal.Module.BusinessObjects;
namespace FT_PurchasingPortal.Module.Web.Controllers
{
    public partial class WebModifyController : WebModificationsController
    {
        public WebModifyController()
        {
        }
        protected override void Save(SimpleActionExecuteEventArgs args)
        {
            base.Save(args);
            RefreshController controller = Frame.GetController<RefreshController>();
            if (typeof(ClassDocument).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (controller != null)
                {
                    controller.RefreshAction.DoExecute();
                }
            }
            else if (typeof(ClassStockTransferDocument).IsAssignableFrom(View.ObjectTypeInfo.Type))
            {
                if (controller != null)
                {
                    controller.RefreshAction.DoExecute();
                }
            }

        }
    }
}
