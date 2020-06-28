using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using FT_PurchasingPortal.Module.BusinessObjects;
using WebApiXafSecurity.Helpers;
using DevExpress.CodeParser;

namespace WebApiXafSecurity.Controllers
{
	[AllowAnonymous]
	[Route("api/[controller]")]
	public class PurchaseDeliveryDetailController : Microsoft.AspNetCore.Mvc.Controller
	{
		const string controllername = "PurchaseDeliveryDetailController";
		SecurityProvider securityProvider;
		IObjectSpace objectSpace;
		public PurchaseDeliveryDetailController(SecurityProvider securityProvider)
		{
			FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
			FT_PurchasingPortal.Module.GeneralValues.NetCoreUserName = securityProvider.GetUserName();
			this.securityProvider = securityProvider;
			objectSpace = securityProvider.ObjectSpaceProvider.CreateObjectSpace();
		}

		[HttpGet]
		public IActionResult Get()
		{
			try
			{
				GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Get:[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

				IQueryable<PurchaseDeliveryDetail> employees = objectSpace.GetObjectsQuery<PurchaseDeliveryDetail>();
				return Ok(employees);
			}
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Get:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
				throw new Exception(ex.Message);
			}
		}

		[HttpGet("{id}")]
		public ActionResult Get(int id)
		{
			try
			{
				GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Get(" + id.ToString() + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

				PurchaseDeliveryDetail existing = objectSpace.GetObjectByKey<PurchaseDeliveryDetail>(id);
				if (existing == null)
				{
					NotFound();
				}
				return Ok(existing);
			}
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Get(" + id.ToString() + "):[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
				throw new Exception(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public ActionResult Delete(int id)
		{
			try
			{
				GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Delete(" + id.ToString() + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

				PurchaseDeliveryDetail existing = objectSpace.GetObjectByKey<PurchaseDeliveryDetail>(id);
				if (existing != null)
				{
					objectSpace.Delete(existing);
					objectSpace.CommitChanges();
					return NoContent();
				}
				return NotFound();
			}
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Delete(" + id.ToString() + "):[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
				throw new Exception(ex.Message);
			}
		}
		[HttpPut("{id}")]
		public ActionResult Update(int id, [FromBody] JObject values)
		{
			try
			{
				GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Put(" + id.ToString() + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

				PurchaseDeliveryDetail employee = objectSpace.GetObjectByKey<PurchaseDeliveryDetail>(id);
				if (employee != null)
				{
					JsonParser.ParseJObjectXPO<PurchaseDeliveryDetail>(values, employee, objectSpace);
					objectSpace.CommitChanges();
					return Ok(employee);
				}
				return NotFound();
			}
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Put(" + id.ToString() + "):[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
				throw new Exception(ex.Message);
			}
		}
		[HttpPost]
		public ActionResult Add([FromBody] JObject values)
		{
			try
			{
				GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Post:[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

				PurchaseDeliveryDetail employee = objectSpace.CreateObject<PurchaseDeliveryDetail>();
                JsonParser.ParseJObjectXPO<PurchaseDeliveryDetail>(values, employee, objectSpace);
				objectSpace.CommitChanges();
				return Ok(employee);
            }
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Post:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
				throw new Exception(ex.Message);
			}
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				objectSpace?.Dispose();
				securityProvider?.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
