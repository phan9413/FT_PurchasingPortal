using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using FT_PurchasingPortal.Module.BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using WebApiXafSecurity.Helpers;

namespace WebApiXafSecurity.Controllers
{
    public class GetDocController : Microsoft.AspNetCore.Mvc.Controller
    {
        const string controllername = "GetDocController";
        SecurityProvider securityProvider;
        IObjectSpace objectSpace;
        public GetDocController(SecurityProvider securityProvider)
        {
            FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
            FT_PurchasingPortal.Module.GeneralValues.NetCoreUserName = securityProvider.GetUserName();
            this.securityProvider = securityProvider;
            objectSpace = securityProvider.ObjectSpaceProvider.CreateObjectSpace();
        }

        /// <summary>
        /// Get PO from API
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        /// </remarks>
        /// <param name="docno">PO No</param>       
        /// <response code="200">Returns found item</response>
        /// <response code="400">Not Found</response>
        [HttpGet]
        [Route("api/getpoitem/{docno}")]
        public IActionResult GetPO(string docno)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPO(" + docno + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                PurchaseOrder obj = objectSpace.FindObject<PurchaseOrder>(CriteriaOperator.Parse("DocNo=?", docno));
                if (obj == null)
                {
                    NotFound();
                }
                List<PurchaseOrder> objlist = new List<PurchaseOrder>();
                objlist.Add(obj);

                var result = objlist.Select(r => new
                {
                    Header = r,
                    PurchaseOrderDetail = r.PurchaseOrderDetail.ToArray()
                }).Single();

                return Ok(result);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPO:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// Get PR from API
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        /// </remarks>
        /// <param name="docno">PR No</param>       
        /// <response code="200">Returns found item</response>
        /// <response code="400">Not Found</response>
        [HttpGet]
        [Route("api/getpritem/{docno}")]
        public IActionResult GetPR(string docno)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPR(" + docno + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                PurchaseRequest obj = objectSpace.FindObject<PurchaseRequest>(CriteriaOperator.Parse("DocNo=?", docno));
                if (obj == null)
                {
                    NotFound();
                }
                List<PurchaseRequest> objlist = new List<PurchaseRequest>();
                objlist.Add(obj);

                var result = objlist.Select(r => new
                {
                    Header = r,
                    PurchaseOrderDetail = r.PurchaseRequestDetail.ToArray()
                }).Single();

                return Ok(result);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPR:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }
        }

    }
}
