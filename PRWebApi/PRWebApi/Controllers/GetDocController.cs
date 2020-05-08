using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;
using DevExpress.Xpo;
using Microsoft.Extensions.Configuration;
using PRWebApi.Helpers;
using FT_PurchasingPortal.Module.BusinessObjects;
using Newtonsoft.Json;

namespace PRWebApi.Controllers
{
    [ApiController]
    public class GetDocController : ControllerBase
    {
        IConfiguration _config;
        UnitOfWork _uow;
        public GetDocController(IConfiguration config, UnitOfWork uow)
        {
            _config = config;
            _uow = uow;
            FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
            //XpoDefault.Session = new UnitOfWork();
        }
        [HttpGet]
        [Route("api/getpoitem/{docno}")]
        public IActionResult Getpo(string docno)
        {
            PurchaseOrder obj = _uow.Query<PurchaseOrder>().Where(pp => pp.DocNo == docno).FirstOrDefault();
            if (obj == null)
            {
                return NotFound();
            }
            //return Ok(obj);

            List<PurchaseOrder> objlist = new List<PurchaseOrder>();
            objlist.Add(obj);

            var result = objlist.Select(r => new
            {
                Header = r,
                PurchaseOrderDetail = r.PurchaseOrderDetail.ToArray()
            }).Single();

            return Ok(result);
        }
        [HttpGet]
        [Route("api/getpritem/{docno}")]
        public IActionResult Getpr(string docno)
        {
            PurchaseRequest obj = _uow.Query<PurchaseRequest>().Where(pp => pp.DocNo == docno).FirstOrDefault();
            if (obj == null)
            {
                return NotFound();
            }
            //return Ok(obj);

            List<PurchaseRequest> objlist = new List<PurchaseRequest>();
            objlist.Add(obj);

            var result = objlist.Select(r => new
            {
                Header = r,
                PurchaseRequestDetail = r.PurchaseRequestDetail.ToArray()
            }).Single();

            return Ok(result);
        }

    }

}