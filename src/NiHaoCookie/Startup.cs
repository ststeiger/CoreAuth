
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace NiHaoCookie
{

    public class SmtpConfig
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public int Port { get; set; }
    }


    public class ConnectionString
    {
        public string name { get; set; }
        public string connectionString { get; set; }
        public string providerName { get; set; }
    }


    public class Startup
    {


        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("SmtpSettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        } // End Constructor 


        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // http://developer.telerik.com/featured/new-configuration-model-asp-net-core/
            // services.Configure<SmtpConfig>(Configuration.GetSection("Smtp"));
            Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions.Configure<SmtpConfig>(services, Configuration.GetSection("Smtp"));

            // Foo:
            // Microsoft.Extensions.DependencyInjection.OptionsServiceCollectionExtensions.Configure<SmtpConfig>(services, Configuration.GetSection("Smtp"));


            // https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-strings
            // ////Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions.Configure<ConnectionString[]>(services, Configuration.GetSection("conStrings"));

            // https://stackoverflow.com/questions/31929482/retrieve-sections-from-config-json-in-asp-net-5
            //var objectSections = Configuration.GetSection("conStringDictionary").GetChildren();
            //foreach (var x in objectSections)
            //{
            //    System.Console.WriteLine(x.Key);
            //    var cs = new ConnectionString();
            //    ConfigurationBinder.Bind(x, cs);
            //    System.Console.WriteLine(cs);
            //}

            // http://andrewlock.net/how-to-use-the-ioptions-pattern-for-configuration-in-asp-net-core-rc2/
            Microsoft.Extensions.DependencyInjection.OptionsConfigurationServiceCollectionExtensions.Configure<Dictionary<string, ConnectionString>>(services, Configuration.GetSection("conStrings"));
            






        //var objects = objectSections.ToDictionary(x => x.Key, x =>
        //{
        //    var obj = new CustomObject();
        //    ConfigurationBinder.Bind(x, obj);
        //    return obj;
        //});




        string conString = Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString(this.Configuration, System.Environment.MachineName);
            System.Console.WriteLine(conString);


            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            services.AddAuthentication(
                delegate(Microsoft.AspNetCore.Authentication.SharedAuthenticationOptions options) 
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
                                     // .AddRequirements( new NoBannedIPsRequirement(new HashSet<string>() { "127.0.0.1", "0.0.0.1" } ))
                                     .Build();

                    config.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
                }
            );

            // services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, NotBannedRequirement>();
        } // End Sub ConfigureServices


        internal static IServiceProvider ServiceProvider { get; set; }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env
            , ILoggerFactory loggerFactory, IServiceProvider svp)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            System.Web.HttpContext.Configure(app.ApplicationServices.
                GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
            );

            ServiceProvider = svp;
            System.Web2.HttpContext.ServiceProvider = svp;
            System.Web2.Hosting.HostingEnvironment.m_IsHosted = true;



            Microsoft.IdentityModel.Tokens.SecurityKey signingKey = null;

            // var x = new System.Security.Cryptography.RSACryptoServiceProvider();
            // Microsoft.IdentityModel.Tokens.RsaSecurityKey rsakew = new Microsoft.IdentityModel.Tokens.RsaSecurityKey(x);

            Microsoft.IdentityModel.Tokens.SymmetricSecurityKey symkey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("Test"));

            // signingKey = rsakew;
            signingKey = symkey;

            // var securityKey = new InMemorySymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("sec"));

            /*
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey,
                            Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256Signature,
                            Microsoft.IdentityModel.Tokens.SecurityAlgorithms.Sha256Digest);
            */


            //      System.IdentityModel.Tokens.SecurityKey
            //System.IdentityModel.Tokens.AsymmetricSecurityKey
            //System.IdentityModel.Tokens.SymmetricSecurityKey


            //System.IdentityModel.Tokens.SymmetricSecurityKey
            //       System.IdentityModel.Tokens.InMemorySymmetricSecurityKey

            // System.IdentityModel.Tokens.AsymmetricSecurityKey
            //       System.IdentityModel.Tokens.RsaSecurityKey
            //       System.IdentityModel.Tokens.X509AsymmetricSecurityKey


            var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "ExampleIssuer",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "ExampleAudience",

                // Validate the token expiry
                ValidateLifetime = true,

                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero, 
            };


            var bearerOptions = new JwtBearerOptions()
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters,
                Events = new CustomBearerEvents()
            };

            // Optional 
            bearerOptions.SecurityTokenValidators.Clear();
            bearerOptions.SecurityTokenValidators.Add(new MyTokenHandler());


            app.UseJwtBearerAuthentication(bearerOptions);

            
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "MyCookieMiddlewareInstance",
                CookieName = "SecurityByObscurityDoesntWork",
                ExpireTimeSpan = new System.TimeSpan(15, 0, 0),
                LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Unauthorized/"),
                AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Forbidden/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                CookieSecure = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest,
                CookieHttpOnly = false,
                TicketDataFormat = new CustomJwtDataFormat("foo", tokenValidationParameters)

                // DataProtectionProvider = null,

                // DataProtectionProvider = new DataProtectionProvider(new System.IO.DirectoryInfo(@"c:\shared-auth-ticket-keys\"),
                //delegate (DataProtectionConfiguration options)
                //{
                //    var op = new Microsoft.AspNet.DataProtection.AuthenticatedEncryption.AuthenticatedEncryptionOptions();
                //    op.EncryptionAlgorithm = Microsoft.AspNet.DataProtection.AuthenticatedEncryption.EncryptionAlgorithm.AES_256_GCM:
                //    options.UseCryptographicAlgorithms(op);
                //}
                //),
            });
            /**/


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

        } // End Sub Configure 


    } // End Class Startup 


} // End Namespace NiHaoCookie 
