﻿
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WebApiXafSecurity.Helpers;

namespace WebApiXafSecurity.Controllers
{
	[Authorize]
	[Route("api/[controller]/[action]")]
	public class ActionsController : Microsoft.AspNetCore.Mvc.Controller
	{
		SecurityProvider securityProvider;
		public ActionsController(SecurityProvider securityProvider)
		{
			this.securityProvider = securityProvider;
		}
		[HttpPost]
		public ActionResult GetPermissions(List<Guid> keys, string typeName)
		{
			try
			{
				ActionResult result = NoContent();
				using (IObjectSpace objectSpace = securityProvider.ObjectSpaceProvider.CreateObjectSpace())
				{
					PermissionHelper permissionHelper = new PermissionHelper(securityProvider.Security);
					ITypeInfo typeInfo = objectSpace.TypesInfo.PersistentTypes.FirstOrDefault(t => t.Name == typeName);
					if (typeInfo != null)
					{
						IList entityList = objectSpace.GetObjects(typeInfo.Type, new InOperator(typeInfo.KeyMember.Name, keys));
						List<ObjectPermission> objectPermissions = new List<ObjectPermission>();
						foreach (object entity in entityList)
						{
							ObjectPermission objectPermission = permissionHelper.CreateObjectPermission(typeInfo, entity);
							objectPermissions.Add(objectPermission);
						}
						result = Ok(objectPermissions);
					}
				}
				return result;
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
				securityProvider?.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}