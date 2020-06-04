﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApiXafSecurity.Helpers;
using WebApiXafSecurity.Models;

namespace WebApiXafSecurity.Controllers
{
	public class AuthenticationController : Controller
	{
		SecurityProvider securityProvider;
		public AuthenticationController(SecurityProvider securityProvider)
		{
			this.securityProvider = securityProvider;
		}
		[HttpPost]
		[Route("Login")]
		[AllowAnonymous]
		public ActionResult Login([FromBody] UserLogin data)
		{
			ActionResult result;
			string userName = data.UserName;
			string password = data.Password;
			if (securityProvider.InitConnection(userName, password))
			{
				result = Ok();
			}
			else
			{
				result = Unauthorized();
			}
			return result;
		}
		[HttpGet]
		[Route("Logout")]
		public async Task<ActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			return Ok();
		}
		[Route("Authentication")]
		public IActionResult Authentication()
		{
			return View();
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
