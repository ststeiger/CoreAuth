
using System.Collections.Generic;
using System.Threading.Tasks;


// Dependency hell
// using Microsoft.AspNetCore.Http; // for WriteAsync 
using Microsoft.AspNetCore.Authorization;


// https://msdn.microsoft.com/en-us/magazine/mt707525.aspx


// http://stackoverflow.com/questions/38264791/custom-authorization-attributes-in-asp-net-core
// http://odetocode.com/blogs/scott/archive/2015/10/06/authorization-policies-and-middleware-in-asp-net-5.aspx
// http://stackoverflow.com/questions/31464359/custom-authorizeattribute-in-asp-net-5-mvc-6
// http://stackoverflow.com/questions/35296657/asp-net-5-passing-parameter-to-authorizationhandlert
// http://www.mikesdotnetting.com/article/269/asp-net-5-middleware-or-where-has-my-httpmodule-gone
// https://docs.asp.net/en/latest/security/authorization/resourcebased.html
// http://stackoverflow.com/questions/31464359/custom-authorizeattribute-in-asp-net-5-mvc-6
// http://blog.stoverud.no/posts/how-to-unit-test-asp-net-core-authorizationhandler/


namespace NiHaoCookie
{

    
    public class NoBannedIPsRequirement : AuthorizationHandler<NoBannedIPsRequirement>, IAuthorizationRequirement
    {
        // System.Collections.Concurrent.ConcurrentDictionary<string, byte> m_bannedIPs = 
        private HashSet<string> m_bannedIPs;

        public NoBannedIPsRequirement(params HashSet<string>[] bannedUsers)
        {
            // m_bannedIPs = new System.Collections.Concurrent.ConcurrentDictionary<string, byte>(System.StringComparer.Ordinal);
            m_bannedIPs = new HashSet<string>(); // { "127.0.0.1", "0.0.0.1" };

            for (int i = 0; i < bannedUsers.Length; ++i)
            {
                m_bannedIPs.UnionWith(bannedUsers[i]);
            } // Next i 

        } // End Constructor 
        

        // https://docs.asp.net/en/latest/security/authorization/resourcebased.html
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NoBannedIPsRequirement requirement)
        {
            Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext ct = context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;
            string ip = ct.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();


            if (m_bannedIPs.Contains(ip))
            {
                // ct.HttpContext.Response.Headers.Clear();
                // ct.HttpContext.Response.Clear();

                // ct.HttpContext.Response.StatusCode = (int) System.Net.HttpStatusCode.ServiceUnavailable;
                // ct.HttpContext.Response.WriteAsync("foo");
                context.Fail();
            } // End if (m_bannedIPs.Contains(ip)) 
            else
                context.Succeed(requirement);

            return Task.CompletedTask;
        } // End Function HandleRequirementAsync 


    } // End Class NoBannedIPsRequirement 


} // End Namespace 
