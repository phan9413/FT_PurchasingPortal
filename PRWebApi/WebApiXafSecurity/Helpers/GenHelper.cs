using DevExpress.ExpressApp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiXafSecurity.Helpers
{
    public static class GenHelper
    {
        public static IObjectSpace LoginByHeader(SecurityProvider securityProvider, HttpRequest request)
        {
            IObjectSpace objectSpace = null;
            StringValues userinfo;
            string username = "";
            string password = "";
            if (request.Headers.TryGetValue("username", out userinfo))
            {
                username = userinfo.First();
            }
            if (request.Headers.TryGetValue("password", out userinfo))
            {
                password = userinfo.First();
            }
            if (securityProvider.InitConnection(username, password))
            {
                objectSpace = securityProvider.ObjectSpaceProvider.CreateObjectSpace();

                FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
                FT_PurchasingPortal.Module.GeneralValues.NetCoreUserName = username;
            }
            return objectSpace;
        }
    }
}
