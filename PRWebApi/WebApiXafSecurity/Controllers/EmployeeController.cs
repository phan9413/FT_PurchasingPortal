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

namespace WebApiXafSecurity.Controllers
{
	[AllowAnonymous]
	[Route("api/[controller]")]
	public class EmployeeController : Microsoft.AspNetCore.Mvc.Controller
	{
		SecurityProvider securityProvider;
		IObjectSpace objectSpace;
		public EmployeeController(SecurityProvider securityProvider)
		{
			this.securityProvider = securityProvider;
			objectSpace = securityProvider.ObjectSpaceProvider.CreateObjectSpace();
		}
		[HttpGet]
		public object Get(DataSourceLoadOptions loadOptions)
		{
			IQueryable<Employee> employees = objectSpace.GetObjectsQuery<Employee>();
			return DataSourceLoader.Load(employees, loadOptions);
		}
		[HttpDelete]
		public ActionResult Delete(int key)
		{
			Employee existing = objectSpace.GetObjectByKey<Employee>(key);
			if (existing != null)
			{
				objectSpace.Delete(existing);
				objectSpace.CommitChanges();
				return NoContent();
			}
			return NotFound();
		}
		[HttpPut]
		public ActionResult Update(int key, string values)
		{
			Employee employee = objectSpace.GetObjectByKey<Employee>(key);
			if (employee != null)
			{
				JsonParser.ParseJObjectXPO<Employee>(JObject.Parse(values), employee, objectSpace);
				return Ok(employee);
			}
			return NotFound();
		}
		[HttpPost]
		public ActionResult Add(string values)
		{
			Employee employee = objectSpace.CreateObject<Employee>();
			JsonParser.ParseJObjectXPO<Employee>(JObject.Parse(values), employee, objectSpace);
			return Ok(employee);
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
