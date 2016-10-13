using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.DataProtection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;




using Microsoft.AspNetCore.DataProtection;


namespace NiHaoCookie
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(delegate(Microsoft.AspNetCore.Authentication.SharedAuthenticationOptions options) 
                {
                    // options.SignInScheme = "foo";
                }    
            );

            // Add framework services.
            // services.AddMvc();
            services.AddMvc(
                delegate (Microsoft.AspNetCore.Mvc.MvcOptions config)
                {
                    Microsoft.AspNetCore.Authorization.AuthorizationPolicy policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                                     .RequireAuthenticatedUser()
                                     //.AddRequirements( new NoBannedIPsRequirement(new HashSet<string>() { "127.0.0.1", "0.0.0.1" } ))
                                     .Build();

                    config.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
                }
            );

            // services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, NotBannedRequirement>();
        }

        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();



           

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "MyCookieMiddlewareInstance",
                LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Unauthorized/"),
                AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Forbidden/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                CookieSecure = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest

               , CookieHttpOnly=false
               // , DataProtectionProvider = null

                //, DataProtectionProvider = new DataProtectionProvider(new System.IO.DirectoryInfo(@"c:\shared-auth-ticket-keys\"),
                //delegate (DataProtectionConfiguration options)
                //{
                //    var op = new Microsoft.AspNet.DataProtection.AuthenticatedEncryption.AuthenticatedEncryptionOptions();
                //    op.EncryptionAlgorithm = Microsoft.AspNet.DataProtection.AuthenticatedEncryption.EncryptionAlgorithm.AES_256_GCM:
                //    options.UseCryptographicAlgorithms(op);
                    
                //}
                //)
            });
            

            // app.UseBanHammer(new BanHammer { Jerks = new HashSet<string>() { "127.0.0.1", "0.0.0.1" } });
            

            app.UseProtectFolder(new ProtectFolderOptions
            {
                Path = "/Secret",
                PolicyName = "Authenticated"
            });



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
