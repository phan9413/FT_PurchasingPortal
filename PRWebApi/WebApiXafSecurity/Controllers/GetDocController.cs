using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using FT_PurchasingPortal.Module.BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiXafSecurity.Helpers;

namespace WebApiXafSecurity.Controllers
{
    [Authorize]
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
        [Route("api/getopenpo/{cardcodekey}")]
        public IActionResult GetOpenPO(string cardcodekey)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPO(" + cardcodekey + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                List<PurchaseOrder> obj = objectSpace.GetObjects<PurchaseOrder>(CriteriaOperator.Parse("[CardCode.BoKey]=? and [DocStatus.CurrDocStatus] in (?,?,?) and [PurchaseOrderDetail][[Quantity] > [CopyQty] and [VerNo] = [PostVerNo]]", cardcodekey, DocStatus.Accepted, DocStatus.Closed, DocStatus.Posted)).ToList();

                return Ok(obj);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPO:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }
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
        [Route("api/getopenpoitem/{cardcodekey}")]
        public IActionResult GetOpenPOItem(string cardcodekey)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenPOItem(" + cardcodekey + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                List<PurchaseOrderDetail> obj = objectSpace.GetObjects<PurchaseOrderDetail>(CriteriaOperator.Parse("!isnull([PurchaseOrder]) and [PurchaseOrder.CardCode.BoKey]=? and [PurchaseOrder.DocStatus.CurrDocStatus] in (?,?,?) and [Quantity] > [CopyQty] and [VerNo] = [PostVerNo]", cardcodekey, DocStatus.Accepted, DocStatus.Closed, DocStatus.Posted)).OrderBy(pp => pp.PurchaseOrder.DocNo).ToList();

                return Ok(obj);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenPOItem:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }

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
        [Route("api/getopenpoitembydoc/{docno}")]
        public IActionResult GetOpenPOItemByDoc(string docno)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenPOItemByDoc(" + docno + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                List<PurchaseOrderDetail> obj = objectSpace.GetObjects<PurchaseOrderDetail>(CriteriaOperator.Parse("!isnull([PurchaseOrder]) and [PurchaseOrder.DocNo]=? and [PurchaseOrder.DocStatus.CurrDocStatus] in (?,?,?) and [Quantity] > [CopyQty] and [VerNo] = [PostVerNo]", docno, DocStatus.Accepted, DocStatus.Closed, DocStatus.Posted)).OrderBy(pp => pp.PurchaseOrder.DocNo).ToList();

                return Ok(obj);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenPOItemByDoc:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }

        }
        /*
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
        [Route("api/getpoitem/{companycode}/{docno}")]
        public IActionResult GetPOItem(string companycode, string docno)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPOItem(" + docno + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                PurchaseOrder obj = objectSpace.FindObject<PurchaseOrder>(CriteriaOperator.Parse("Company.BoCode=? and DocNo=?", companycode, docno));
                if (obj == null)
                {
                    return NotFound();
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
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPOItem:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
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
        [Route("api/getpritem/{companycode}/{docno}")]
        public IActionResult GetPRItem(string companycode, string docno)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPRItem(" + docno + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                PurchaseRequest obj = objectSpace.FindObject<PurchaseRequest>(CriteriaOperator.Parse("Company.BoCode=? and DocNo=?", companycode, docno));
                if (obj == null)
                {
                    return NotFound();
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
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPRItem:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }
        }
        */
        /// <summary>
        /// Get PR from API
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        /// </remarks>
        /// <param name="username">user name</param>       
        /// <response code="200">Returns found item</response>
        /// <response code="400">Not Found</response>
        [HttpGet]
        [Route("api/getgrnitemcount/{username}")]
        public IActionResult GetGRNItemCount(string username)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetGRNItemCount(" + username + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                int cnt = 0;

                List<PurchaseDeliveryDetail> obj = objectSpace.GetObjects<PurchaseDeliveryDetail>(CriteriaOperator.Parse("[CreateUser.UserName]=? and isnull([PurchaseDelivery])", username)).ToList();
                if (obj != null)
                {
                    cnt = obj.Count;
                }
                return Ok(cnt);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetGRNItemCount:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }
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
        [HttpGet]
        [Route("api/getgrnitem/{username}")]
        public IActionResult GetGRNItem(string username)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetGRNItem(" + username + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                List<PurchaseDeliveryDetail> obj = objectSpace.GetObjects<PurchaseDeliveryDetail>(CriteriaOperator.Parse("[CreateUser.UserName]=? and isnull([PurchaseDelivery])", username)).ToList();
                if (obj == null)
                {
                    return NotFound();
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetGRNItem:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }
        }
    }
}
