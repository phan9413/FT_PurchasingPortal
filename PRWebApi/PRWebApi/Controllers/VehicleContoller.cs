using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DevExpress.Xpo;
using Microsoft.Extensions.Configuration;

//using System.Linq;
//using Microsoft.AspNetCore.Http;
//using DevExpress.Xpo.DB;
//using PRWebApi.Models;
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using Newtonsoft.Json.Serialization;

//using System.Net;
//using Newtonsoft.Json;
//using System.Data;
//using System.Data.SqlClient;

using Newtonsoft.Json.Linq;
using Dapper;
using PRWebApi.Helpers;
using FT_PurchasingPortal.Module.BusinessObjects;
using DevExpress.Xpo.Helpers;
using DevExpress.Persistent.Validation;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Xpo;

namespace PRWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        IConfiguration _config;
        UnitOfWork _uow;
        public VehicleController(IConfiguration config, UnitOfWork uow)
        {
            _config = config;
            _uow = uow;
            //XpoDefault.Session = new UnitOfWork();
        }
        [HttpGet]
        public IEnumerable<Vehicle> Get()
        {
            try
            {
                IEnumerable<Vehicle> list = _uow.Query<Vehicle>();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public Vehicle Get(int id)
        {
            return _uow.GetObjectByKey<Vehicle>(id);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JObject values)
        {
            //PurchaseRequest obj = JsonPopulateObjectHelper.PopulateObject<PurchaseRequest>(values.ToString(), _uow);
            //PurchaseRequest customer = new PurchaseRequest(_uow);
            //customer.FullName = values["FullName"].Value<string>();
            //_uow.CommitChanges();
            Vehicle obj = JsonPopulateObjectHelper.PopulateObject<Vehicle>(values.ToString(), _uow);
            //RuleSet rule = new RuleSet();
            //rule.ValidateAll((IObjectSpace)obj.Session, _uow.GetObjectsToSave(), "Any");
            await _uow.CommitChangesAsync();
            return Ok(obj);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]JObject value)
        {
            ////PurchaseRequest customer = _uow.GetObjectByKey<PurchaseRequest>(id);
            ////JToken token;
            ////if (value.TryGetValue("Department", out token))
            ////{
            ////    customer.Department = _uow.GetObjectByKey<Departments>(token["Oid"].Value<int>());
            ////}

            ////if (value.TryGetValue("FullName", out token))
            ////{
            ////    customer.FullName = value["FullName"].Value<string>();
            ////}

            //PurchaseRequest customer = _uow.GetObjectByKey<PurchaseRequest>(id);

            //JsonPopulateObjectHelper.PopulateObject(value.ToString(), _uow, customer);
            //_uow.CommitChanges();

            Vehicle obj = await _uow.GetObjectByKeyAsync<Vehicle>(id);
            if (obj == null)
            {
                return NotFound();
            }
            JsonPopulateObjectHelper.PopulateObject(value.ToString(), _uow, obj);
            await _uow.CommitChangesAsync();
            return Ok(obj);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //PurchaseRequest customer = _uow.GetObjectByKey<PurchaseRequest>(id);
            //_uow.Delete(customer);
            //_uow.CommitChanges();

            Vehicle obj = await _uow.GetObjectByKeyAsync<Vehicle>(id);
            if (obj == null)
            {
                return NotFound();
            }
            _uow.Delete(obj);
            await _uow.CommitChangesAsync();
            return Ok(obj);
        }
        //[HttpGet]
        //public List<DocTypeSeries> Get()
        //{
        //    List<DocTypeSeries> result = null;
        //    XPQuery<DocTypeSeries> query = (XPQuery<DocTypeSeries>)_uow.Query<DocTypeSeries>();
        //    result = query.ToList<DocTypeSeries>();

        //    return result;
        //}


    }
}