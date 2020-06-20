﻿
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using FT_PurchasingPortal.Module.BusinessObjects;

namespace WebApiXafSecurity.Helpers
{
	public class SecurityProvider : IDisposable
	{
		public SecurityStrategyComplex Security { get; private set; }
		public IObjectSpaceProvider ObjectSpaceProvider { get; private set; }
		XpoDataStoreProviderService xpoDataStoreProviderService;
		IConfiguration config;
		IHttpContextAccessor contextAccessor;
		public string GetUserName()
        {
			return contextAccessor.HttpContext.User.Identity.Name;
		}
		public SecurityProvider(XpoDataStoreProviderService xpoDataStoreProviderService, IConfiguration config, IHttpContextAccessor contextAccessor)
		{
			DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = true;
			DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = false;
			this.xpoDataStoreProviderService = xpoDataStoreProviderService;
			this.config = config;
			this.contextAccessor = contextAccessor;
			if (contextAccessor.HttpContext.User.Identity.IsAuthenticated)
			{
				Initialize();
			}
		}
		public bool InitConnection(string userName, string password)
		{
			AuthenticationStandardLogonParameters parameters = new AuthenticationStandardLogonParameters(userName, password);
			SecurityStrategyComplex security = GetSecurity(typeof(AuthenticationStandardProvider).Name, parameters);
			IObjectSpaceProvider objectSpaceProvider = GetObjectSpaceProvider(security);
			//ObjectSpaceProvider = GetObjectSpaceProvider(security);
			try
			{
				Login(security, objectSpaceProvider);
				SignIn(contextAccessor.HttpContext, userName);
				return true;
			}
			catch (Exception ex)
			{
				GenHelper.WriteLog("[Error]", "[" + userName + "]Login Error:[" + ex.Message + "][" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "]");
				return false;
			}
		}
		public void Initialize()
		{
			Security = GetSecurity(typeof(IdentityAuthenticationProvider).Name, contextAccessor.HttpContext.User.Identity);
			ObjectSpaceProvider = GetObjectSpaceProvider(Security);
			Login(Security, ObjectSpaceProvider);
		}
		private void SignIn(HttpContext httpContext, string userName)
		{
			List<Claim> claims = new List<Claim>{
				new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
			};
			ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
			ClaimsPrincipal principal = new ClaimsPrincipal(id);
			httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
		}
		private SecurityStrategyComplex GetSecurity(string authenticationName, object parameter)
		{
			AuthenticationMixed authentication = new AuthenticationMixed();
			authentication.LogonParametersType = typeof(AuthenticationStandardLogonParameters);
			authentication.AddAuthenticationStandardProvider(typeof(PermissionPolicyUser));
			authentication.AddIdentityAuthenticationProvider(typeof(PermissionPolicyUser));
			authentication.SetupAuthenticationProvider(authenticationName, parameter);
			SecurityStrategyComplex security = new SecurityStrategyComplex(typeof(PermissionPolicyUser), typeof(PermissionPolicyRole), authentication);
			security.RegisterXPOAdapterProviders();
			return security;
		}
		private IObjectSpaceProvider GetObjectSpaceProvider(SecurityStrategyComplex security)
		{
			string connectionString = config.GetConnectionString("XafApplication");
			SecuredObjectSpaceProvider objectSpaceProvider = new SecuredObjectSpaceProvider(security, xpoDataStoreProviderService.GetDataStoreProvider(connectionString, null, true), true);
			RegisterEntities(objectSpaceProvider);
			return objectSpaceProvider;
		}
		private void Login(SecurityStrategyComplex security, IObjectSpaceProvider objectSpaceProvider)
		{
			try
			{
				IObjectSpace objectSpace = objectSpaceProvider.CreateObjectSpace();
				security.Logon(objectSpace);
			}
			catch (Exception ex)
            {
				throw new Exception(ex.Message);
            }

		}
		private void RegisterEntities(SecuredObjectSpaceProvider objectSpaceProvider)
		{
			objectSpaceProvider.TypesInfo.RegisterEntity(typeof(Employee));
			objectSpaceProvider.TypesInfo.RegisterEntity(typeof(PermissionPolicyUser));
			objectSpaceProvider.TypesInfo.RegisterEntity(typeof(PermissionPolicyRole));
			objectSpaceProvider.TypesInfo.RegisterEntity(typeof(PurchaseRequest));
			objectSpaceProvider.TypesInfo.RegisterEntity(typeof(PurchaseOrder));
			objectSpaceProvider.TypesInfo.RegisterEntity(typeof(PurchaseDelivery));
		}
		public void Dispose()
		{
			Security?.Dispose();
			((SecuredObjectSpaceProvider)ObjectSpaceProvider)?.Dispose();
		}
	}
}