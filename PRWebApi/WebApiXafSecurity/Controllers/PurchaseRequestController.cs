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
	public class PurchaseRequestController : Microsoft.AspNetCore.Mvc.Controller
	{
		SecurityProvider securityProvider;
		IObjectSpace objectSpace;
		public PurchaseRequestController(SecurityProvider securityProvider)
		{
			this.securityProvider = securityProvider;
			objectSpace = securityProvider.ObjectSpaceProvider.CreateObjectSpace();
		}

		[HttpGet]
		public IActionResult Get()
		{
			try
			{
				//objectSpace = GenHelper.LoginByHeader(securityProvider, Request);
				//if (objectSpace is null) return Unauthorized();
				IQueryable<PurchaseRequest> employees = objectSpace.GetObjectsQuery<PurchaseRequest>();
				return Ok(employees);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[HttpGet("{id}")]
		public ActionResult Get(int id)
		{
			try
			{
				//objectSpace = GenHelper.LoginByHeader(securityProvider, Request);
				//if (objectSpace is null) return Unauthorized();
				PurchaseRequest existing = objectSpace.GetObjectByKey<PurchaseRequest>(id);
				if (existing == null)
				{
					NotFound();
				}
				return Ok(existing);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public ActionResult Delete(int id)
		{
			try
			{
				//objectSpace = GenHelper.LoginByHeader(securityProvider, Request);
				//if (objectSpace is null) return Unauthorized();
				PurchaseRequest existing = objectSpace.GetObjectByKey<PurchaseRequest>(id);
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
				throw new Exception(ex.Message);
			}
		}
		[HttpPut("{id}")]
		public ActionResult Update(int id, [FromBody] JObject values)
		{
			try
			{
				//objectSpace = GenHelper.LoginByHeader(securityProvider, Request);
				//if (objectSpace is null) return Unauthorized();
				PurchaseRequest employee = objectSpace.GetObjectByKey<PurchaseRequest>(id);
				if (employee != null)
				{
					JsonParser.ParseJObjectXPO<PurchaseRequest>(values, employee, objectSpace);
					return Ok(employee);
				}
				return NotFound();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		[HttpPost]
		public ActionResult Add([FromBody] JObject values)
		{
			try
			{
				//objectSpace = GenHelper.LoginByHeader(securityProvider, Request);
				//if (objectSpace is null) return Unauthorized();

				PurchaseRequest employee = objectSpace.CreateObject<PurchaseRequest>();
                JsonParser.ParseJObjectXPO<PurchaseRequest>(values, employee, objectSpace);
                return Ok(employee);
            }
			catch (Exception ex)
			{
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
