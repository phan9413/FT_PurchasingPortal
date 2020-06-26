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
using Microsoft.AspNetCore.Cors;
using DevExpress.Data.Filtering;

namespace WebApiXafSecurity.Controllers
{
	//[AllowAnonymous]
	[Authorize]
	public class vwWarehouseBinController : Microsoft.AspNetCore.Mvc.Controller
	{
		const string controllername = "vwWarehouseBinController";
		SecurityProvider securityProvider;
		IObjectSpace objectSpace;
		public vwWarehouseBinController(SecurityProvider securityProvider)
		{
			FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
			FT_PurchasingPortal.Module.GeneralValues.NetCoreUserName = securityProvider.GetUserName();
			this.securityProvider = securityProvider;
			objectSpace = securityProvider.ObjectSpaceProvider.CreateObjectSpace();
		}

		[Route("api/WH/Bin/{bokey}")]
		[HttpGet]
		public ActionResult Get(string bokey)
		{
			try
			{
				GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Get(" + bokey + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

				vwWarehouseBins existing = objectSpace.FindObject<vwWarehouseBins>(CriteriaOperator.Parse("BoKey=?", bokey));
				if (existing == null)
				{
					NotFound();
				}
				return Ok(existing);
			}
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-Get(" + bokey + "):[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
				throw new Exception(ex.Message);
			}
		}
		[Route("api/WH/BinList/{companycode}")]
		[HttpGet]
		public ActionResult GetList(string companycode)
		{
			try
			{
				GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetList(" + companycode + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

				List<vwWarehouseBins> existing = objectSpace.GetObjects<vwWarehouseBins>(CriteriaOperator.Parse("CompanyCode=?", companycode)).ToList();
				return Ok(existing);
			}
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetList(" + companycode + "):[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
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
