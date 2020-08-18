using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.XtraCharts;
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

        #region open PO
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
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenPO(" + cardcodekey + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                List<PurchaseOrder> obj;
                if (GenHelper.LiveWithPost)
                    obj = objectSpace.GetObjects<PurchaseOrder>(CriteriaOperator.Parse("[CardCode.BoKey]=? and [DocStatus.CurrDocStatus] in (?) and [PurchaseOrderDetail][[Quantity] > [CopyQty] and [VerNo] = [PostVerNo]]", cardcodekey, DocStatus.Accepted)).ToList();
                else
                    obj = objectSpace.GetObjects<PurchaseOrder>(CriteriaOperator.Parse("[CardCode.BoKey]=? and [DocStatus.CurrDocStatus] in (?) and [PurchaseOrderDetail][[Quantity] > [CopyQty]]", cardcodekey, DocStatus.Accepted)).ToList();

                return Ok(obj);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenPO:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
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
                List<PurchaseOrderDetail> obj;
                if (GenHelper.LiveWithPost)
                    obj = objectSpace.GetObjects<PurchaseOrderDetail>(CriteriaOperator.Parse("!isnull([PurchaseOrder]) and [PurchaseOrder.CardCode.BoKey]=? and [PurchaseOrder.DocStatus.CurrDocStatus] in (?) and [Quantity] > [CopyQty] and [VerNo] = [PostVerNo]", cardcodekey, DocStatus.Accepted)).OrderBy(pp => pp.PurchaseOrder.DocNo).ToList();
                else
                    obj = objectSpace.GetObjects<PurchaseOrderDetail>(CriteriaOperator.Parse("!isnull([PurchaseOrder]) and [PurchaseOrder.CardCode.BoKey]=? and [PurchaseOrder.DocStatus.CurrDocStatus] in (?) and [Quantity] > [CopyQty]", cardcodekey, DocStatus.Accepted)).OrderBy(pp => pp.PurchaseOrder.DocNo).ToList();

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
                List<PurchaseOrderDetail> obj;
                if (GenHelper.LiveWithPost)
                    obj = objectSpace.GetObjects<PurchaseOrderDetail>(CriteriaOperator.Parse("!isnull([PurchaseOrder]) and [PurchaseOrder.DocNo]=? and [PurchaseOrder.DocStatus.CurrDocStatus] in (?) and [Quantity] > [CopyQty] and [VerNo] = [PostVerNo]", docno, DocStatus.Accepted)).OrderBy(pp => pp.PurchaseOrder.DocNo).ToList();
                else
                    obj = objectSpace.GetObjects<PurchaseOrderDetail>(CriteriaOperator.Parse("!isnull([PurchaseOrder]) and [PurchaseOrder.DocNo]=? and [PurchaseOrder.DocStatus.CurrDocStatus] in (?) and [Quantity] > [CopyQty]", docno, DocStatus.Accepted)).OrderBy(pp => pp.PurchaseOrder.DocNo).ToList();

                return Ok(obj);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenPOItemByDoc:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }

        }
        #endregion
        #region GRN item
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
        #endregion


        #region open GRN
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
        [Route("api/getopengrn/{cardcodekey}")]
        public IActionResult GetOpenGRN(string cardcodekey)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenGRN(" + cardcodekey + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                List<PurchaseDelivery> obj;
                if(GenHelper.LiveWithPost)
                    obj = objectSpace.GetObjects<PurchaseDelivery>(CriteriaOperator.Parse("[CardCode.BoKey]=? and [DocStatus.CurrDocStatus]=? and [DocStatus.IsSAPPosted]=1 and [PurchaseOrderDetail][[Quantity] > [CopyQty] and [VerNo] = [PostVerNo]]", cardcodekey, DocStatus.Posted)).ToList();
                else
                    obj = objectSpace.GetObjects<PurchaseDelivery>(CriteriaOperator.Parse("[CardCode.BoKey]=? and [DocStatus.CurrDocStatus] in (?,?) and [PurchaseOrderDetail][[Quantity] > [CopyQty]]", cardcodekey, DocStatus.Accepted, DocStatus.Posted)).ToList();

                return Ok(obj);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenGRN:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
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
        [Route("api/getopengrnitem/{cardcodekey}")]
        public IActionResult GetOpenGRNItem(string cardcodekey)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenGRNItem(" + cardcodekey + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                List<PurchaseDeliveryDetail> obj;
                if (GenHelper.LiveWithPost)
                    obj = objectSpace.GetObjects<PurchaseDeliveryDetail>(CriteriaOperator.Parse("!isnull([PurchaseDelivery]) and [PurchaseDelivery.CardCode.BoKey]=? and [PurchaseDelivery.DocStatus.CurrDocStatus]=? and [PurchaseDelivery.DocStatus.IsSAPPosted]=1 and [Quantity] > [CopyQty] and [VerNo] = [PostVerNo]", cardcodekey, DocStatus.Posted)).OrderBy(pp => pp.PurchaseDelivery.DocNo).ToList();
                else
                    obj = objectSpace.GetObjects<PurchaseDeliveryDetail>(CriteriaOperator.Parse("!isnull([PurchaseDelivery]) and [PurchaseDelivery.CardCode.BoKey]=? and [PurchaseDelivery.DocStatus.CurrDocStatus] in (?,?) and [Quantity] > [CopyQty]", cardcodekey, DocStatus.Accepted, DocStatus.Posted)).OrderBy(pp => pp.PurchaseDelivery.DocNo).ToList();

                return Ok(obj);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenGRNItem:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
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
        [Route("api/getopengrnitembydoc/{docno}")]
        public IActionResult GetOpenGRNItemByDoc(string docno)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenGRNItemByDoc(" + docno + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                List<PurchaseDeliveryDetail> obj;
                if (GenHelper.LiveWithPost)
                    obj = objectSpace.GetObjects<PurchaseDeliveryDetail>(CriteriaOperator.Parse("!isnull([PurchaseDelivery]) and [PurchaseDelivery.DocNo]=? and [PurchaseDelivery.DocStatus.CurrDocStatus]=? and [PurchaseDelivery.DocStatus.IsSAPPosted]=1 and [Quantity] > [CopyQty] and [VerNo] = [PostVerNo]", docno, DocStatus.Posted)).OrderBy(pp => pp.PurchaseDelivery.DocNo).ToList();
                else
                    obj = objectSpace.GetObjects<PurchaseDeliveryDetail>(CriteriaOperator.Parse("!isnull([PurchaseDelivery]) and [PurchaseDelivery.DocNo]=? and [PurchaseDelivery.DocStatus.CurrDocStatus] in (?,?) and [Quantity] > [CopyQty]", docno, DocStatus.Accepted, DocStatus.Posted)).OrderBy(pp => pp.PurchaseDelivery.DocNo).ToList();

                return Ok(obj);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetOpenGRNItemByDoc:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }

        }
        #endregion
        #region PUR item
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
        [Route("api/getpuritemcount/{username}")]
        public IActionResult GetPURItemCount(string username)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPURItemCount(" + username + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                int cnt = 0;

                List<PurchaseReturnDetail> obj = objectSpace.GetObjects<PurchaseReturnDetail>(CriteriaOperator.Parse("[CreateUser.UserName]=? and isnull([PurchaseReturn])", username)).ToList();
                if (obj != null)
                {
                    cnt = obj.Count;
                }
                return Ok(cnt);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPURItemCount:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
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
        [Route("api/getpuritem/{username}")]
        public IActionResult GetPURItem(string username)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPURItem(" + username + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                List<PurchaseReturnDetail> obj = objectSpace.GetObjects<PurchaseReturnDetail>(CriteriaOperator.Parse("[CreateUser.UserName]=? and isnull([PurchaseReturn])", username)).ToList();
                if (obj == null)
                {
                    return NotFound();
                }

                return Ok(obj);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetPURItem:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }
        }
        #endregion

    }
}
