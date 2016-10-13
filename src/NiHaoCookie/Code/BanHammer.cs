
using System.Collections.Generic;
using System.Threading.Tasks;

// Dependency hell
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authorization;


namespace NiHaoCookie
{


    // http://odetocode.com/blogs/scott/archive/2015/10/06/authorization-policies-and-middleware-in-asp-net-5.aspx
    public class BanHammer
    {
        public HashSet<string> Jerks { get; set; }
    } // End Class BanHammer 


    public static class BanHammerExtensions
    {

        public static IApplicationBuilder UseBanHammer(
            this IApplicationBuilder builder, BanHammer options)
        {
            return builder.UseMiddleware<BanHammerMeddleWare>(options);
        } // End Extension Function UseBanHammer 

    } // End Static Class BanHammerExtensions 


    public class BanHammerMeddleWare // Not a spelling mistake, FYI
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate m_next;
        private readonly HashSet<string> m_bannedIPs;
        

        public BanHammerMeddleWare(RequestDelegate next, BanHammer options)
        {
            this.m_next = next;
            this.m_bannedIPs = options.Jerks;
        } // End Constructor 


        public async Task Invoke(HttpContext context,
                                 IAuthorizationService authorizationService)
        {
            string requestIP = context.Connection.RemoteIpAddress.MapToIPv4().ToString();

            if(m_bannedIPs.Contains(requestIP))
            {
                context.Response.Headers.Clear();
                context.Response.Clear();

                context.Response.StatusCode = (int)System.Net.HttpStatusCode.ServiceUnavailable;
                await context.Response.WriteAsync("Banned. And I don't feel sorry.");

                // context.Abort();
                return;
            } // End if(m_bannedIPs.Contains(requestIP)) 

            //await this.m_next(context);
            await this.m_next.Invoke(context);
        } // End Function Invoke 


    } // End Class BanHammerMeddleWare 


} // End Namespace 
