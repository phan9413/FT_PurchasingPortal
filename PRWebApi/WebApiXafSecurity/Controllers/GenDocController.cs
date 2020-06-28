using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using FT_PurchasingPortal.Module;
using FT_PurchasingPortal.Module.BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApiXafSecurity.Helpers;

namespace WebApiXafSecurity.Controllers
{
    [Authorize]
    public class GenDocController : Microsoft.AspNetCore.Mvc.Controller
    {
        const string controllername = "GenDocController";
        SecurityProvider securityProvider;
        IObjectSpace objectSpace;
        public GenDocController(SecurityProvider securityProvider)
        {
            FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
            FT_PurchasingPortal.Module.GeneralValues.NetCoreUserName = securityProvider.GetUserName();
            this.securityProvider = securityProvider;
            objectSpace = securityProvider.ObjectSpaceProvider.CreateObjectSpace();
        }

        /// <summary>
        /// Get PR from API
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        /// </remarks>
        /// <param name="username">user name</param>       
        /// <response code="200">Returns found item</response>
        /// <response code="400">Not Found</response>
        [HttpPost]
        [Route("api/gengrnitem")]
        public IActionResult GenGRNItem([FromBody] JArray values)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRNItem:[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                GenHelper.WriteLog("[Json]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRNItem:[" + Environment.NewLine + values.ToString() + Environment.NewLine + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                PurchaseDeliveryDetail dtl;
                string temp = "";
                JToken token;
                foreach (JObject Jdtl in values.Children())
                {
                    dtl = objectSpace.CreateObject<PurchaseDeliveryDetail>();
                    temp = Jdtl["Baseline"].ToString();
                    dtl.Baseline = int.Parse(temp);

                    if (Jdtl.ContainsKey("BaseDocNo"))
                    {
                        temp = Jdtl["BaseDocNo"].ToString();
                        dtl.BaseDocNo = temp;
                    }

                    temp = DocTypeCodes.PurchaseOrder;
                    dtl.BaseType = objectSpace.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", temp));

                    temp = Jdtl["Quantity"].ToString();
                    dtl.Quantity = double.Parse(temp);

                    if (Jdtl.ContainsKey("BatchNumber"))
                    {
                        if (Jdtl["BatchNumber"] != null)
                            temp = Jdtl["BatchNumber"].ToString();
                            if (!string.IsNullOrEmpty(temp))
                                dtl.BatchNumber = temp;
                    }

                    token = Jdtl["BinCode"];
                    temp = token["BoKey"].ToString();
                    dtl.BinCode = objectSpace.FindObject<vwWarehouseBins>(CriteriaOperator.Parse("BoKey=?", temp));

                    token = Jdtl["LineVendor"];
                    temp = token["BoKey"].ToString();
                    dtl.LineVendor = objectSpace.FindObject<vwBusinessPartners>(CriteriaOperator.Parse("BoKey=?", temp));

                }
                objectSpace.CommitChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRNItem:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }
        }
    }
}
