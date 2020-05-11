using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DevExpress.Xpo;
using PRWebApi.Models;
using PRWebApi.Helpers;
using FT_PurchasingPortal.Module.BusinessObjects;

namespace PRWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        UnitOfWork _uow;

        public LoginController(IConfiguration config, UnitOfWork uow)
        {
            _config = config;
            _uow = uow;
        }

        //[Authorize]
        //[HttpGet("GetValue")]
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    //return new string[] { _config["Jwt:Key"], _config["Jwt:Issuer"], _config["ConnectionStrings:Default"] };
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;
        //    IList<Claim> claim = identity.Claims.ToList();
        //    return new string[] { claim[0].Value, claim[1].Value, claim[2].Value };
        //}


        [HttpPost]
        public IActionResult Post([FromBody] UserLogin data)
        {

            try
            {
                UserLogin login = new UserLogin();
                login.UserName = data.UserName;
                login.Password = data.Password;
                IActionResult response = Unauthorized();

                var user = AuthenticateUser(login);
                if (user != null)
                {
                    Helper hlp = new Helper(_config);
                    var tokenStr = hlp.GenerateJasonWebToken(user);
                    response = Ok(new { token = tokenStr });
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }


        private UserLogin AuthenticateUser(UserLogin login)
        {
            try
            {
                bool xaf = false;

                if (xaf)
                {
                    #region xaf connection (SystemUsers do not have StoredPassword property
                    //SystemUsers systemuser = (SystemUsers)_uow.Query<SystemUsers>().Where(pp => pp.UserName == login.UserName).FirstOrDefault();
                    //if (systemuser != null)
                    //{
                    //    UserLogin usr = new UserLogin();
                    //    if (HelperXaf.VerifyHashedPassword(systemuser.StoredPassword, login.Password))
                    //        return usr;
                    //}
                    #endregion
                }
                else
                {
                    #region sql connection
                    string connstr = _config["ConnectionStrings:Default"].ToString();

                    using (IDbConnection conn = new SqlConnection(connstr))
                    {
                        string query = "select T0.OID, T0.UserName, T0.StoredPassword, T1.Company, T1.Employee from PermissionPolicyUser T0 inner join OSUR T1 on T0.OID = T1.OID where UserName = @UserName";
                        UserLogin usr = SqlMapper.Query<UserLogin>(conn, query, new { UserName = login.UserName }).FirstOrDefault();
                        //UserLogin usr = new UserLogin { UserID = login.UserID, Password = login.Password };

                        //if (XafSecurityClass.VerifyHashedPassword(usr.StoredPassword, login.Password))
                        //    return usr;

                        //XafSecurityClass xafsecurity = new XafSecurityClass();
                        if (usr != null)
                            if (HelperXaf.VerifyHashedPassword(usr.StoredPassword, login.Password))
                                return usr;
                    }
                    #endregion
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}