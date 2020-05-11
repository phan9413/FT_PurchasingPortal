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
    public class GenDocController : ControllerBase
    {
        IConfiguration _config;
        UnitOfWork _uow;
        public GenDocController(IConfiguration config, UnitOfWork uow)
        {
            _config = config;
            _uow = uow;
            FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
            //XpoDefault.Session = new UnitOfWork();
        }
        [HttpPost]
        [Route("api/submitgrnfrompo/{docno}")]
        public async Task<IActionResult> GenGRN(string docno, [FromBody]JObject values)
        {
            try
            {
                PurchaseOrder sobj = _uow.Query<PurchaseOrder>().Where(pp => pp.DocNo == docno).FirstOrDefault();
                if (sobj == null)
                {
                    return NotFound();
                }
                PurchaseDelivery obj = JsonPopulateObjectHelper.PopulateObject<PurchaseDelivery>(values.ToString(), _uow);

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

    }

}