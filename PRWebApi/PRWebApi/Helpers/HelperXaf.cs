using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
//using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;

namespace PRWebApi.Helpers
{
    public static class HelperXaf
    {
        public const string xafAssembly = "FT_PurchasingPortal.Module.BusinessObjects";
    }
    public static bool VerifyHashedPassword(string StoredPassword, string Password)
    {
        PasswordCryptographer.EnableRfc2898 = true;
        PasswordCryptographer.SupportLegacySha512 = false;

        return new PasswordCryptographer().AreEqual(StoredPassword, Password);
        //return PasswordCryptographer.VerifyHashedPasswordDelegate(StoredPassword, Password);
    }
    public static string HashPassword(string Password)
    {
        PasswordCryptographer.EnableRfc2898 = true;
        PasswordCryptographer.SupportLegacySha512 = false;

        return new PasswordCryptographer().GenerateSaltedPassword(Password.Trim());
        //return PasswordCryptographer.HashPasswordDelegate(password);
    }

}
