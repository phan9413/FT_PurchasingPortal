using DevExpress.ExpressApp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiXafSecurity.Helpers
{
    public static class GenHelper
    {
        public const string xafAssembly = "FT_PurchasingPortal.Module.BusinessObjects";
        public static string FilePath = "";
        public static string GetUserNameFromRequest(HttpRequest request)
        {
            StringValues userinfo;
            if (request.Headers.TryGetValue("username", out userinfo))
            {
                return userinfo.First();
            }
            return "";
        }
        public static void WriteLog(string lvl, string str)
        {
            FileStream fileStream = null;

            string filePath = FilePath + "[PRWebAPI] Log_" + System.DateTime.Today.ToString("yyyyMMdd") + "." + "txt";

            FileInfo fileInfo = new FileInfo(filePath);
            DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
            if (!dirInfo.Exists) dirInfo.Create();

            if (!fileInfo.Exists)
            {
                fileStream = fileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(filePath, FileMode.Append);
            }

            StreamWriter log = new StreamWriter(fileStream);
            string status = lvl.ToString();//.Replace("[Log]", "");

            //For SAP_Integration_Log
            log.WriteLine("{0}{1}", status, str.ToString());

            log.Close();

        }
    }
}
