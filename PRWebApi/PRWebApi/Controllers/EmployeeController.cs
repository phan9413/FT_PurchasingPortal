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

namespace PRWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        IConfiguration _config;
        UnitOfWork _uow;
        public EmployeeController(IConfiguration config, UnitOfWork uow)
        {
            _config = config;
            _uow = uow;
            //XpoDefault.Session = new UnitOfWork();
        }
        [HttpGet]
        public IEnumerable<Employee> Get()
        {
            try
            {
                IEnumerable<Employee> list = _uow.Query<Employee>();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public Employee Get(int id)
        {
            return _uow.GetObjectByKey<Employee>(id);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JObject values)
        {
            //Employee obj = JsonPopulateObjectHelper.PopulateObject<Employee>(values.ToString(), _uow);
            //Employee customer = new Employee(_uow);
            //customer.FullName = values["FullName"].Value<string>();
            //_uow.CommitChanges();
            Employee obj = JsonPopulateObjectHelper.PopulateObject<Employee>(values.ToString(), _uow);
            await _uow.CommitChangesAsync();
            return Ok(obj);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]JObject value)
        {
            ////Employee customer = _uow.GetObjectByKey<Employee>(id);
            ////JToken token;
            ////if (value.TryGetValue("Department", out token))
            ////{
            ////    customer.Department = _uow.GetObjectByKey<Departments>(token["Oid"].Value<int>());
            ////}

            ////if (value.TryGetValue("FullName", out token))
            ////{
            ////    customer.FullName = value["FullName"].Value<string>();
            ////}

            //Employee customer = _uow.GetObjectByKey<Employee>(id);

            //JsonPopulateObjectHelper.PopulateObject(value.ToString(), _uow, customer);
            //_uow.CommitChanges();

            Employee obj = await _uow.GetObjectByKeyAsync<Employee>(id);
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
            //Employee customer = _uow.GetObjectByKey<Employee>(id);
            //_uow.Delete(customer);
            //_uow.CommitChanges();

            Employee obj = await _uow.GetObjectByKeyAsync<Employee>(id);
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