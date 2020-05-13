using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using PRWebApi.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.PlatformAbstractions;

namespace PRWebApi
{
    public class Startup
    {
        //private string loginPath = "/Authentication";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //string conn = Configuration["ConnectionStrings:Default"].ToString();
            //XpoDefault.Session.ConnectionString = conn;
            /*
            string server = Configuration["ConnectionStrings:Server"].ToString();
            string database = Configuration["ConnectionStrings:Database"].ToString();
            int port = 0;
            int.TryParse(Configuration["ConnectionStrings:Port"].ToString(), out port);
            string uid = Configuration["ConnectionStrings:uid"].ToString();
            string pwd = Configuration["ConnectionStrings:pwd"].ToString();
            string sqlConn = "";

            if (port > 0)
                sqlConn = MSSqlConnectionProvider.GetConnectionString(server, port, uid, pwd, database);
            else
                sqlConn = MSSqlConnectionProvider.GetConnectionString(server, uid, pwd, database);

            XpoDefault.DataLayer = XpoDefault.GetDataLayer(sqlConn, AutoCreateOption.None);
            //XpoDefault.Session = null;

            //string conn = Configuration["ConnectionStrings:Default"].ToString();
            //XpoDefault.Session.Connect();
            */

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#if !NETCOREAPP
#endif

            //#region xaf authentication

            JsonSerializationContractResolver resolver = new JsonSerializationContractResolver();
            Action<MvcNewtonsoftJsonOptions> JsonOptions =
                options =>
                {
                    options.SerializerSettings.ContractResolver = resolver;
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                };
            //services.AddMvc(options =>
            //{
            //    options.EnableEndpointRouting = false;
            //})
            //    .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddControllers()
                .AddNewtonsoftJson(JsonOptions);
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(options =>
            //     {
            //         options.LoginPath = loginPath;
            //     });
            //services.AddSingleton<XpoDataStoreProviderService>();
            //services.AddSingleton(Configuration);
            //services.AddHttpContextAccessor();
            //services.AddScoped<SecurityProvider>();
            //#endregion

            //services.AddControllers();


            //services.AddControllers()
            //    .AddNewtonsoftJson(options =>
            //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //);
            //services.AddControllers();
            //services.AddMvcCore();
            //services.AddApiExplorer();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            })
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

#region xaf uow
            services.AddControllers()
                .AddNewtonsoftJson(JsonOptions)
                .AddDatabaseJsonOptions();
            services.AddXpoDefaultUnitOfWork(true, (DataLayerOptionsBuilder options) =>
                options.UseConnectionString(Configuration.GetConnectionString("ConnectionString"))
                // .UseAutoCreationOption(AutoCreateOption.DatabaseAndSchema) // debug only
                .UseEntityTypes(PRWebApi.Helpers.ConnectionHelper.GetPersistentTypes())
                );

#endregion
            services.AddSingleton<XpoDataStoreProviderService>();
            services.AddSingleton(Configuration);
            services.AddHttpContextAccessor();
            services.AddScoped<SecurityProvider>();

#region cors
            services.AddCors();
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy", builder =>
            //     builder.AllowAnyOrigin()
            //     .AllowAnyMethod()
            //     .AllowAnyHeader()
            //     .AllowCredentials()
            //     .Build());

            //});
            #endregion
            #region jwt bearer
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = Configuration["Jwt:Issuer"],
            //            ValidAudience = Configuration["Jwt:Issuer"],
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
            //        };
            //    });
            #endregion

            #region swagger
            // Inject an implementation of ISwaggerProvider with defaulted settings applied.
            services.AddSwaggerGen();
            // Add the detail information for the API.
            services.ConfigureSwaggerGen((options) =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Swagger API",
                    Version = "v1"
                });

                //Determine base path for the application.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                //Set the comments path for the swagger json and ui.
                options.IncludeXmlComments(basePath + "\\PRWebApi.xml");
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //#region xaf authentication
            //app.UseCookiePolicy();
            //app.UseDefaultFiles();
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    OnPrepareResponse = context =>
            //    {
            //        if (context.Context.User.Identity.IsAuthenticated)
            //        {
            //            return;
            //        }
            //        else
            //        {
            //            //string referer = context.Context.Request.Headers["Referer"].ToString();
            //            //string authenticationPagePath = loginPath;
            //            //string vendorString = "vendor.css";
            //            //if (context.Context.Request.Path.HasValue && context.Context.Request.Path.StartsWithSegments(authenticationPagePath)
            //            //    || referer != null && (referer.Contains(authenticationPagePath) || referer.Contains(vendorString)))
            //            //{
            //            //    return;
            //            //}
            //            //context.Context.Response.Redirect(loginPath);
            //        }
            //    }
            //});
            //#endregion
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            #region swagger
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI( c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Swagger v1");

            });
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
