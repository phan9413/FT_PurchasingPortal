using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_PurchasingPortal.Module
{
    public static class DocTypeCodes
    {
        public const string SalesQuotation = "SalesQuotation";
        public const string SalesAgreement = "SalesAgreement";
        public const string SalesOrder = "SalesOrder";

        public const string PurchaseRequest = "PurchaseRequest";
        public const string PurchaseOrder = "PurchaseOrder";
        public const string PurchaseDelivery = "PurchaseDelivery";
        
        public const string StockTransferRequest = "StockTransferRequest";

    }
    public class GeneralValues
    {
        public const string viewpricestring = "_ViewPrice";
        //public static bool IsBeingLookup = false;
        //public static string ObjType = "";
        //public static string BusinessPartner = "";

        public const string FTAdmin = "FTAdmin";
        public const string FTAdminpwd = "8899";
        public static string appurl = "";

        public const string hq = "HQ";
        public static string defwh = "01";
        public static string deflocalcurrency = "01";

        public static string TempFolder = "";
        public const string claimsuperrole = "ClaimSuperRole";
        public const string claimrole = "ClaimUserRole";
        public const string verifyrole = "VerifyUserRole";
        public const string Acceptancerole = "AcceptanceUserRole";
        public const string postrole = "PostUserRole";
        public const string ApprovalRole = "ApprovalUserRole";
        public const string RejectApproveRole = "RejectApproveRole";

        public static string definputtax = "X1";
        public static string defoutputtax = "X0";

        public static bool EmailSend;
        public static string EmailHost = "";
        public static string EmailHostDomain = "";
        public static string EmailPort = "";
        public static string Email = "";
        public static string EmailPassword = "";
        public static string EmailName = "";
        public static bool EmailSSL;
        public static bool EmailUseDefaultCredential;
        public static string DeliveryMethod = "";

        public static bool IsNetCore = false;
    }
}
