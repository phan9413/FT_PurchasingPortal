using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PRWebApi.Helpers;
using PRWebApi.Models;
using DevExpress.ExpressApp;
using System;

namespace PRWebApi.Controllers
{
	/*
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Microsoft.AspNetCore.Mvc.Controller
	{
		SecurityProvider securityProvider;
		public AuthenticationController(SecurityProvider securityProvider)
		{
			this.securityProvider = securityProvider;
		}
		
		[HttpPost]
		//[Route("Login")]
		[AllowAnonymous]
		//public ActionResult Login(string userName, string password)
		public ActionResult Login([FromBody] UserLogin data)
		{
			try
			{

				ActionResult result;
				if (securityProvider.InitConnection(data.UserName, data.Password))
				{
					result = Ok();
				}
				else
				{
					result = Unauthorized();
				}
				return result;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		[HttpGet]
		[Route("Logout")]
		public async Task<ActionResult> Logout()
		{
			try
			{
				await HttpContext.SignOutAsync();
				return Ok();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
		[HttpGet]
		[Route("Authentication")]
		public IActionResult Authentication()
		{
			try
			{
				return View();
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
	*/
}