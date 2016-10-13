using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NiHaoCookie
{

    // http://odetocode.com/blogs/scott/archive/2015/10/06/authorization-policies-and-middleware-in-asp-net-5.aspx
    public class ProtectFolderOptions
    {
        public PathString Path { get; set; }
        public string PolicyName { get; set; }
    }

    public static class ProtectFolderExtensions
    {
        public static IApplicationBuilder UseProtectFolder(
            this IApplicationBuilder builder,
            ProtectFolderOptions options)
        {
            return builder.UseMiddleware<ProtectFolder>(options);
        }
    }

    public class ProtectFolder
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
        private readonly Microsoft.AspNetCore.Http.PathString _path;
        private readonly string _policyName;

        public ProtectFolder(RequestDelegate next, ProtectFolderOptions options)
        {
            _next = next;
            _path = options.Path;
            _policyName = options.PolicyName;
        }

        public async Task Invoke(HttpContext httpContext,
                                 IAuthorizationService authorizationService)
        {
            if (httpContext.Request.Path.StartsWithSegments(_path))
            {
                var authorized = await authorizationService.AuthorizeAsync(
                                    httpContext.User, null, _policyName);
                if (!authorized)
                {
                    await httpContext.Authentication.ChallengeAsync();
                    return;
                }
            }

            await _next(httpContext);
        }
    }

}
