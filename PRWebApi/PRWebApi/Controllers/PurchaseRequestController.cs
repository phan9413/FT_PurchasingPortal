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
    public class PurchaseRequestController : ControllerBase
    {
        IConfiguration _config;
        UnitOfWork _uow;
        public PurchaseRequestController(IConfiguration config, UnitOfWork uow)
        {
            _config = config;
            _uow = uow;
            FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
            //XpoDefault.Session = new UnitOfWork();
        }
        [HttpGet]
        public IEnumerable<PurchaseRequest> Get()
        {
            try
            {
                IEnumerable<PurchaseRequest> list = _uow.Query<PurchaseRequest>();
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            PurchaseRequest obj = await _uow.GetObjectByKeyAsync<PurchaseRequest>(id);
            if (obj == null)
            {
                return NotFound();
            }
            //return Ok(obj);

            List<PurchaseRequest> objlist = new List<PurchaseRequest>();
            objlist.Add(obj);

            var result = objlist.Select(r => new {
                Header = r,
                PurchaseRequestDetail = r.PurchaseRequestDetail.ToArray()
            }).Single();

            return Ok(result);
        }
             
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]JObject values)
        {
            try
            {
                PurchaseRequest obj = null;
                //PurchaseRequest obj = JsonPopulateObjectHelper.PopulateObject<PurchaseRequest>(values.ToString(), _uow);
                //PurchaseRequest customer = new PurchaseRequest(_uow);
                //customer.FullName = values["FullName"].Value<string>();
                //_uow.CommitChanges();
                obj = JsonPopulateObjectHelper.PopulateObject<PurchaseRequest>(values.ToString(), _uow);

                addNewDetailsOnly(values, obj, true);

                await _uow.CommitChangesAsync();
                //obj = JsonConvert.DeserializeObject<PurchaseRequest>(values.ToString());
                return Ok(obj);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]JObject values)
        {
            try
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
                PurchaseRequest obj = null;

                obj = await _uow.GetObjectByKeyAsync<PurchaseRequest>(id);
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //PurchaseRequest customer = _uow.GetObjectByKey<PurchaseRequest>(id);
                //_uow.Delete(customer);
                //_uow.CommitChanges();

                PurchaseRequest obj = await _uow.GetObjectByKeyAsync<PurchaseRequest>(id);
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

        private void addNewDetailsOnly(JObject value, PurchaseRequest obj, bool IsNewHeader)
        {
            try
            {
                #region add details
                string detalclassname = "PurchaseRequestDetail";
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
                            PurchaseRequestDetail dtl = JsonPopulateObjectHelper.PopulateObject<PurchaseRequestDetail>(Jdtl.ToString(), _uow);
                            obj.PurchaseRequestDetail.Add(dtl);
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

        private void deleteDetails(JObject value, PurchaseRequest obj)
        {
            try
            {
                #region delete details
                string detalclassname = "PurchaseRequestDetail";
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
                                PurchaseRequestDetail dtl = obj.PurchaseRequestDetail.Where(pp => pp.Oid == intkeyvalue).FirstOrDefault();
                                if (dtl != null)
                                    dtl.Delete();
                                    //obj.PurchaseRequestDetail.Remove(dtl);

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