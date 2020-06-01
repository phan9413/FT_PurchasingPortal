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
    public class PurchaseDeliveryController : ControllerBase
    {
        IConfiguration _config;
        UnitOfWork _uow;
        public PurchaseDeliveryController(IConfiguration config, UnitOfWork uow)
        {
            _config = config;
            _uow = uow;
            FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
            FT_PurchasingPortal.Module.GeneralValues.NetCoreUserName = "";
            //XpoDefault.Session = new UnitOfWork();
        }
        /// <summary>
        /// Get all PurchaseDelivery.
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
                IEnumerable<PurchaseDelivery> list = _uow.Query<PurchaseDelivery>();
                return Ok(list);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Get a PurchaseDelivery.
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
                PurchaseDelivery obj = await _uow.GetObjectByKeyAsync<PurchaseDelivery>(id);
                if (obj == null)
                {
                    return NotFound();
                }
                //return Ok(obj);

                List<PurchaseDelivery> objlist = new List<PurchaseDelivery>();
                objlist.Add(obj);

                var result = objlist.Select(r => new
                {
                    Header = r,
                    PurchaseDeliveryDetail = r.PurchaseDeliveryDetail.ToArray()
                }).Single();

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Create a PurchaseDelivery.
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        ///  
        ///     POST
        ///     {
        ///        PurchaseDelivery Object
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
                PurchaseDelivery obj = null;
                //PurchaseDelivery obj = JsonPopulateObjectHelper.PopulateObject<PurchaseDelivery>(values.ToString(), _uow);
                //PurchaseDelivery customer = new PurchaseDelivery(_uow);
                //customer.FullName = values["FullName"].Value<string>();
                //_uow.CommitChanges();
                obj = JsonPopulateObjectHelper.PopulateObject<PurchaseDelivery>(values.ToString(), _uow);

                addNewDetailsOnly(values, obj, true);

                await _uow.CommitChangesAsync();
                //obj = JsonConvert.DeserializeObject<PurchaseDelivery>(values.ToString());
                return Ok(obj);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// Update a PurchaseDelivery.
        /// </summary>
        /// <remarks>
        /// Note that the key is a Oid and an integer.
        ///  
        ///     PUT
        ///     {
        ///        PurchaseDelivery Object
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
                ////PurchaseDelivery customer = _uow.GetObjectByKey<PurchaseDelivery>(id);
                ////JToken token;
                ////if (value.TryGetValue("Department", out token))
                ////{
                ////    customer.Department = _uow.GetObjectByKey<Departments>(token["Oid"].Value<int>());
                ////}

                ////if (value.TryGetValue("FullName", out token))
                ////{
                ////    customer.FullName = value["FullName"].Value<string>();
                ////}

                //PurchaseDelivery customer = _uow.GetObjectByKey<PurchaseDelivery>(id);

                //JsonPopulateObjectHelper.PopulateObject(value.ToString(), _uow, customer);
                //_uow.CommitChanges();
                PurchaseDelivery obj = null;

                obj = await _uow.GetObjectByKeyAsync<PurchaseDelivery>(id);
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
        /// Delete a PurchaseDelivery.
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
                //PurchaseDelivery customer = _uow.GetObjectByKey<PurchaseDelivery>(id);
                //_uow.Delete(customer);
                //_uow.CommitChanges();

                PurchaseDelivery obj = await _uow.GetObjectByKeyAsync<PurchaseDelivery>(id);
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

        private void addNewDetailsOnly(JObject value, PurchaseDelivery obj, bool IsNewHeader)
        {
            try
            {
                #region add details
                string detalclassname = "PurchaseDeliveryDetail";
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
                            PurchaseDeliveryDetail dtl = JsonPopulateObjectHelper.PopulateObject<PurchaseDeliveryDetail>(Jdtl.ToString(), _uow);
                            obj.PurchaseDeliveryDetail.Add(dtl);
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

        private void deleteDetails(JObject value, PurchaseDelivery obj)
        {
            try
            {
                #region delete details
                string detalclassname = "PurchaseDeliveryDetail";
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
                                PurchaseDeliveryDetail dtl = obj.PurchaseDeliveryDetail.Where(pp => pp.Oid == intkeyvalue).FirstOrDefault();
                                if (dtl != null)
                                    dtl.Delete();
                                    //obj.PurchaseDeliveryDetail.Remove(dtl);

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