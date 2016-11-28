using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;



// http://www.w3schools.com/css/css_rwd_mediaqueries.asp
namespace NiHaoCookie.Controllers
{
    public class HomeController : Controller
    {

        public SmtpConfig SmtpConfig { get; }

        public Dictionary<string, ConnectionString> dict;

        public HomeController(Microsoft.Extensions.Options.IOptions<SmtpConfig> smtpConfig
            , Microsoft.Extensions.Options.IOptions<Dictionary<string, ConnectionString>>  connectionStrings)
        {


            SmtpConfig = smtpConfig.Value;
        } //Action Controller


        public IActionResult Index()
        {
            System.Console.WriteLine(SmtpConfig);
            System.Console.WriteLine(dict);
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
