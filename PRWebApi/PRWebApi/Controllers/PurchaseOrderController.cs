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
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        IConfiguration _config;
        UnitOfWork _uow;
        public PurchaseOrderController(IConfiguration config, UnitOfWork uow)
        {
            _config = config;
            _uow = uow;
            FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
            //XpoDefault.Session = new UnitOfWork();
        }
        /// <summary>
        /// Get all PurchaseOrder.
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
                IEnumerable<PurchaseOrder> list = _uow.Query<PurchaseOrder>();
                return Ok(list);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Get a PurchaseOrder.
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
                PurchaseOrder obj = await _uow.GetObjectByKeyAsync<PurchaseOrder>(id);
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Create a PurchaseOrder.
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        ///  
        ///     POST
        ///     {
        ///        PurchaseOrder Object
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
                PurchaseOrder obj = null;
                //PurchaseOrder obj = JsonPopulateObjectHelper.PopulateObject<PurchaseOrder>(values.ToString(), _uow);
                //PurchaseOrder customer = new PurchaseOrder(_uow);
                //customer.FullName = values["FullName"].Value<string>();
                //_uow.CommitChanges();
                obj = JsonPopulateObjectHelper.PopulateObject<PurchaseOrder>(values.ToString(), _uow);

                addNewDetailsOnly(values, obj, true);

                await _uow.CommitChangesAsync();
                //obj = JsonConvert.DeserializeObject<PurchaseOrder>(values.ToString());
                return Ok(obj);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Update a PurchaseOrder.
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        ///  
        ///     PUT
        ///     {
        ///        PurchaseOrder Object
        ///     }
        /// 
        /// </remarks>
        /// <returns>New Created Item</returns>
        /// <response code="200">Returns the newly created item</response>
        /// <response code="400">Not Found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]JObject values)
        {
            try
            {
                ////PurchaseOrder customer = _uow.GetObjectByKey<PurchaseOrder>(id);
                ////JToken token;
                ////if (value.TryGetValue("Department", out token))
                ////{
                ////    customer.Department = _uow.GetObjectByKey<Departments>(token["Oid"].Value<int>());
                ////}

                ////if (value.TryGetValue("FullName", out token))
                ////{
                ////    customer.FullName = value["FullName"].Value<string>();
                ////}

                //PurchaseOrder customer = _uow.GetObjectByKey<PurchaseOrder>(id);

                //JsonPopulateObjectHelper.PopulateObject(value.ToString(), _uow, customer);
                //_uow.CommitChanges();
                PurchaseOrder obj = null;

                obj = await _uow.GetObjectByKeyAsync<PurchaseOrder>(id);
                if (obj == null)
                {
                    return NotFound();
                }
                JsonPopulateObjectHelper.PopulateObject(values.ToString(), _uow, obj);

                addNewDetailsOnly(values, obj, false);
                deleteDetails(values, obj);

                await _uow.CommitChangesAsync();
                return Ok(obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Delete a PurchaseOrder.
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
                //PurchaseOrder customer = _uow.GetObjectByKey<PurchaseOrder>(id);
                //_uow.Delete(customer);
                //_uow.CommitChanges();

                PurchaseOrder obj = await _uow.GetObjectByKeyAsync<PurchaseOrder>(id);
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

        private void addNewDetailsOnly(JObject value, PurchaseOrder obj, bool IsNewHeader)
        {
            try
            {
                #region add details
                string detalclassname = "PurchaseOrderDetail";
                bool isnew = false;
                int intkeyvalue = -1;
                JArray jarray = (JArray)value[detalclassname];
                foreach (JObject Jdtl in jarray.Children())
                {
                    isnew = false;
                    if (IsNewHeader)
                    {
                        isnew = true;
                    }
                    else
                    {
                        if (Jdtl.ContainsKey("Oid"))
                        {
                            if (Jdtl["Oid"] == null)
                            {
                                isnew = true;
                            }
                            else
                            {
                                if (int.TryParse(Jdtl["Oid"].ToString(), out intkeyvalue))
                                {
                                    if (intkeyvalue == -1) isnew = true;
                                }
                            }
                        }
                        else
                        {
                            isnew = true;
                        }
                    }
                    if (isnew)
                    {
                        if (Jdtl.ContainsKey("IsBeingDelete"))
                        {
                            if (Jdtl["IsBeingDelete"].ToString() == "1" || Jdtl["IsBeingDelete"].ToString().ToUpper() == "TRUE")
                            {
                                isnew = false;
                            }
                        }
                        if (isnew)
                        {
                            PurchaseOrderDetail dtl = JsonPopulateObjectHelper.PopulateObject<PurchaseOrderDetail>(Jdtl.ToString(), _uow);
                            obj.PurchaseOrderDetail.Add(dtl);
                        }
                    }

                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void deleteDetails(JObject value, PurchaseOrder obj)
        {
            try
            {
                #region delete details
                string detalclassname = "PurchaseOrderDetail";
                bool isnew = false;
                int intkeyvalue = -1;
                JArray jarray = (JArray)value[detalclassname];
                foreach (JObject Jdtl in jarray.Children())
                {
                    isnew = false;
                    if (Jdtl.ContainsKey("Oid"))
                    {
                        if (Jdtl["Oid"] == null)
                        {
                            isnew = true;
                        }
                        else
                        {
                            if (int.TryParse(Jdtl["Oid"].ToString(), out intkeyvalue))
                            {
                                if (intkeyvalue == -1) isnew = true;
                            }
                        }
                    }
                    else
                    {
                        isnew = true;
                    }
                    if (!isnew)
                    {
                        if (Jdtl.ContainsKey("IsBeingDelete"))
                        {
                            if (Jdtl["IsBeingDelete"].ToString() == "1" || Jdtl["IsBeingDelete"].ToString().ToUpper() == "TRUE")
                            {
                                PurchaseOrderDetail dtl = obj.PurchaseOrderDetail.Where(pp => pp.Oid == intkeyvalue).FirstOrDefault();
                                if (dtl != null)
                                    dtl.Delete();
                                    //obj.PurchaseOrderDetail.Remove(dtl);

                            }
                        }
                    }

                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}