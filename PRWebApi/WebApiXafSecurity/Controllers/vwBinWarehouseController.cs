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
	public class vwBinWarehouseController : Microsoft.AspNetCore.Mvc.Controller
	{
		const string controllername = "vwBinWarehouseController";
		SecurityProvider securityProvider;
		IObjectSpace objectSpace;
		public vwBinWarehouseController(SecurityProvider securityProvider)
		{
			FT_PurchasingPortal.Module.GeneralValues.IsNetCore = true;
			FT_PurchasingPortal.Module.GeneralValues.NetCoreUserName = securityProvider.GetUserName();
			this.securityProvider = securityProvider;
			objectSpace = securityProvider.ObjectSpaceProvider.CreateObjectSpace();
		}
		[Route("api/WhBin/WhsBin/{bokey}")]
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

		[Route("api/WhBin/WhsBinList/{companycode}")]
		[HttpGet]
		public ActionResult GetWhsBinList(string companycode)
		{
			try
			{
				GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetWhsBinList(" + companycode + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

				List<vwWarehouseBins> existing = objectSpace.GetObjects<vwWarehouseBins>(CriteriaOperator.Parse("CompanyCode=?", companycode)).ToList();
				return Ok(existing);
			}
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetWhsBinList(" + companycode + "):[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
				throw new Exception(ex.Message);
			}
		}
		[Route("api/WhBin/BinList/{companycode}")]
		[HttpGet]
		public ActionResult GetBinList(string companycode)
		{
			try
			{
				GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetBinList(" + companycode + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

				List<vwWarehouseBins> existing = objectSpace.GetObjects<vwWarehouseBins>(CriteriaOperator.Parse("CompanyCode=? and BinAbsEntry>0", companycode)).ToList();
				return Ok(existing);
			}
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetBinList(" + companycode + "):[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
				throw new Exception(ex.Message);
			}
		}
		[Route("api/WhBin/WhsList/{companycode}")]
		[HttpGet]
		public ActionResult GetWhsList(string companycode)
		{
			try
			{
				GenHelper.WriteLog("[Log]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetWhsList(" + companycode + "):[" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");

				List<vwWarehouseBins> existing = objectSpace.GetObjects<vwWarehouseBins>(CriteriaOperator.Parse("CompanyCode=? and BinAbsEntry=0", companycode)).ToList();
				return Ok(existing);
			}
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + securityProvider.GetUserName() + "]" + controllername + "-GetWhsList(" + companycode + "):[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
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
