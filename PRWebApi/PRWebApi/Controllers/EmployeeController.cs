﻿using System;
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

        /// <summary>
        /// Get all Employee.
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        /// </remarks>
        /// <returns>Found Item</returns>
        /// <response code="200">Returns found item</response>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<Employee> list = _uow.Query<Employee>();
                return Ok(list);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Get a Employee.
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        /// </remarks>
        /// <param name="id"></param>
        /// <returns>Found Item</returns>
        /// <response code="200">Returns found item</response>
        /// <response code="400">Not Found</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                Employee obj = await _uow.GetObjectByKeyAsync<Employee>(id);
                if (obj == null)
                {
                    NotFound();
                }
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Create a Employee.
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        ///  
        ///     POST
        ///     {
        ///        Employees Object
        ///     }
        /// 
        /// </remarks>
        /// <returns>New Created Item</returns>
        /// <response code="200">Returns the newly created item</response>
        /// <response code="400">Not Found</response>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JObject values)
        {
            try
            {
                //PurchaseRequest obj = JsonPopulateObjectHelper.PopulateObject<PurchaseRequest>(values.ToString(), _uow);
                //PurchaseRequest customer = new PurchaseRequest(_uow);
                //customer.FullName = values["FullName"].Value<string>();
                //_uow.CommitChanges();
                Employee obj = JsonPopulateObjectHelper.PopulateObject<Employee>(values.ToString(), _uow);
                //RuleSet rule = new RuleSet();
                //rule.ValidateAll((IObjectSpace)obj.Session, _uow.GetObjectsToSave(), "Any");
                await _uow.CommitChangesAsync();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Update a Employee.
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        ///  
        ///     PUT
        ///     {
        ///        Employee Object
        ///     }
        /// 
        /// </remarks>
        /// <param name="id">Oid value</param>
        /// <returns>Updated Item</returns>
        /// <response code="200">Returns the updated item</response>
        /// <response code="400">Not Found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]JObject value)
        {
            try
            {
                ////PurchaseRequest customer = _uow.GetObjectByKey<PurchaseRequest>(id);
                ////JToken token;
                ////if (value.TryGetValue("Employee", out token))
                ////{
                ////    customer.Employee = _uow.GetObjectByKey<Employees>(token["Oid"].Value<int>());
                ////}

                ////if (value.TryGetValue("FullName", out token))
                ////{
                ////    customer.FullName = value["FullName"].Value<string>();
                ////}

                //PurchaseRequest customer = _uow.GetObjectByKey<PurchaseRequest>(id);

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Delete a Employee.
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        /// </remarks>
        /// <param name="id">Oid value</param>
        /// <returns></returns>
        /// <response code="200">Deleted Item</response>
        /// <response code="400">Not Found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //PurchaseRequest customer = _uow.GetObjectByKey<PurchaseRequest>(id);
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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