using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.XtraCharts;
using FT_PurchasingPortal.Module;
using FT_PurchasingPortal.Module.BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebApiXafSecurity.Helpers;

namespace WebApiXafSecurity.Controllers
{
    [Authorize]
    public class GenDocController : Microsoft.AspNetCore.Mvc.Controller
    {
        const string controllername = "GenDocController";
        SecurityProvider securityProvider;
        IObjectSpace objectSpace;
        public GenDocController(SecurityProvider securityProvider)
        {
            FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
            FT_PurchasingPortal.Module.GeneralValues.NetCoreUserName = securityProvider.GetUserName();
            this.securityProvider = securityProvider;
            objectSpace = securityProvider.ObjectSpaceProvider.CreateObjectSpace();
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
        [HttpPost]
        [Route("api/gengrn")]
        public IActionResult GenGRN([FromBody] JObject values)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRN:[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                GenHelper.WriteLog("[Json]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRN:[" + Environment.NewLine + values.ToString() + Environment.NewLine + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                //
                //JsonParser.ParseJObjectXPO<PurchaseDelivery>(values, employee, objectSpace);
                string temp = "";
                JToken token;
                PurchaseDelivery employee = objectSpace.CreateObject<PurchaseDelivery>();
                token = values["CardCode"];
                temp = token["BoKey"].ToString();
                employee.CardCode = objectSpace.FindObject<vwBusinessPartners>(CriteriaOperator.Parse("BoKey=?", temp));
                employee.NumAtCard = values["NumAtCard"].ToString();

                //JsonPopulateObjectHelper.PopulateObjectWODetail(values.ToString(), employee.Session, employee);

                string detalclassname = "PurchaseDeliveryDetail";
                int intkeyvalue = -1;
                JArray jarray = (JArray)values[detalclassname];
                int cnt = 0;
                foreach (JObject Jdtl in jarray.Children())
                {
                    if (Jdtl.ContainsKey("Oid"))
                    {
                        if (int.TryParse(Jdtl["Oid"].ToString(), out intkeyvalue))
                        {
                            PurchaseDeliveryDetail dtl = objectSpace.GetObjectByKey<PurchaseDeliveryDetail>(intkeyvalue);
                            cnt++;
                            dtl.VisOrder = cnt;
                            employee.PurchaseDeliveryDetail.Add(dtl);
                        }
                        else
                        {
                            GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRN:[Details Key value is invalid][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                            throw new Exception("Details Key value is invalid");
                        }
                    }
                    else
                    {
                        GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRN:[Details Key Column Not found][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                        throw new Exception("Details Key Column Not found");
                    }
                }
                if (employee.DocTypeSeries == null)
                {
                    GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRN:[Document series is not found][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                    throw new Exception("Document series is not found");
                }
                employee.DocStatus.AddDocStatus(DocStatus.Accepted, "WebApi Generated");
                employee.DocStatus.CurrDocStatus = DocStatus.Accepted;
                employee.AssignDocNumber();
                objectSpace.CommitChanges();
                return Ok(employee.DocNum);
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRN:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
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
        [HttpPost]
        [Route("api/gengrnitem")]
        public IActionResult GenGRNItem([FromBody] JArray values)
        {
            try
            {
                GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRNItem:[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                GenHelper.WriteLog("[Json]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRNItem:[" + Environment.NewLine + values.ToString() + Environment.NewLine + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

                PurchaseDeliveryDetail dtl;
                string temp = "";
                JToken token;
                foreach (JObject Jdtl in values.Children())
                {
                    dtl = objectSpace.CreateObject<PurchaseDeliveryDetail>();
                    temp = Jdtl["Baseline"].ToString();
                    dtl.Baseline = int.Parse(temp);

                    if (Jdtl.ContainsKey("BaseDocNo"))
                    {
                        temp = Jdtl["BaseDocNo"].ToString();
                        dtl.BaseDocNo = temp;
                    }

                    temp = DocTypeCodes.PurchaseOrder;
                    dtl.BaseType = objectSpace.FindObject<DocType>(CriteriaOperator.Parse("BoCode=?", temp));

                    temp = Jdtl["Quantity"].ToString();
                    dtl.Quantity = double.Parse(temp);

                    if (Jdtl.ContainsKey("BatchNumber"))
                    {
                        if (Jdtl["BatchNumber"] != null)
                            temp = Jdtl["BatchNumber"].ToString();
                            if (!string.IsNullOrEmpty(temp))
                                dtl.BatchNumber = temp;
                    }

                    token = Jdtl["BinCode"];
                    temp = token["BoKey"].ToString();
                    dtl.BinCode = objectSpace.FindObject<vwWarehouseBins>(CriteriaOperator.Parse("BoKey=?", temp));

                    token = Jdtl["LineVendor"];
                    temp = token["BoKey"].ToString();
                    dtl.LineVendor = objectSpace.FindObject<vwBusinessPartners>(CriteriaOperator.Parse("BoKey=?", temp));

                }
                objectSpace.CommitChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GenGRNItem:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
                throw new Exception(ex.Message);
            }
        }
    }
}
