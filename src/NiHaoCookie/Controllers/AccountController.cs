using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NiHaoCookie.Controllers
{

    // Very important...
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class AccountController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Unauthorized()
        {
            // https://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core
            System.Net.IPAddress remoteIpAddress = this.Request.HttpContext.Connection.RemoteIpAddress;
            System.Console.WriteLine(remoteIpAddress);


            List<System.Security.Claims.Claim> ls = new List<System.Security.Claims.Claim>();

            ls.Add(
                new System.Security.Claims.Claim(
                    System.Security.Claims.ClaimTypes.Name, "IcanHazUsr_éèêëïàáâäåãæóòôöõõúùûüñçø_ÉÈÊËÏÀÁÂÄÅÃÆÓÒÔÖÕÕÚÙÛÜÑÇØ 你好，世界 Привет\tмир"
                , System.Security.Claims.ClaimValueTypes.String
                )
            );

            // 

            System.Security.Claims.ClaimsIdentity id = new System.Security.Claims.ClaimsIdentity("authenticationType");
            id.AddClaims(ls);

            System.Security.Claims.ClaimsPrincipal principal = new System.Security.Claims.ClaimsPrincipal(id);

            // https://docs.asp.net/en/latest/security/authentication/cookie.html
            HttpContext.Authentication.SignInAsync("MyCookieMiddlewareInstance", principal);
            


            return Content("Unauthorized", "text/plain");
        }

        public IActionResult LogOut()
        {
            // await HttpContext.Authentication.SignOutAsync("MyCookieMiddlewareInstance");
            HttpContext.Authentication.SignOutAsync("MyCookieMiddlewareInstance");

            return Content("logged out", "text/plain");
        }



        public IActionResult Forbidden()
        {
            return Content("Forbidden", "text/plain");
        }
    }
}
