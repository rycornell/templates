using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ASPNETCoreProjectTemplate
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();
            services.AddHttpContextAccessor();

            services.AddControllers(options =>
            {
                // Ensure all responses are returned with the application/json content type
                // Helpful for legacy front ends that use jquery $.ajax and running into this issue:
                // https://stackoverflow.com/questions/5061310/jquery-returning-parsererror-for-ajax-request
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Helpful for legacy data models that may be inflexible and causing binding errors
                    options.SuppressModelStateInvalidFilter = true;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                    // Helpful for legacy front ends that don't support camel casing.
                    options.UseMemberCasing();
                })
                // Helpful for legacy APIs that support XML.
                .AddXmlSerializerFormatters();

            services.AddAuthorization()
                .AddAntiforgery(x =>
                {
                    x.Cookie.Name = "__RequestVerificationToken";
                });

            // Combination of Forms and JWT auth
            services.AddAuthentication("FormsAuth")
                .AddPolicyScheme("FormsAuth", "Used for legacy form submissions that don't have auth headers", x =>
                {
                    x.ForwardDefaultSelector = ConfigureFormsAuthentication;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("yourIssuerSigningKeyCode")),
                        ValidateIssuer = true,
                        ValidIssuer = "yourIssuerCode",
                        ValidateAudience = false,
                        ValidateLifetime = true,
                    };
                    //x.EventsType = typeof(CustomEvents);
                });

            // CORS support
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
                        
            services.AddTransient<IEventLogger, ApplicationInsightsEventLogger>();
            services.AddTransient<IIdentityService, IdentityService>();
            //services.AddTransient(_ => ApplicationUserManager.Create(new ApplicationDbContext()));
            //services.AddTransient<IDBConnection>(_ => new DBConnection(Configuration.GetValue<string>("User"), Configuration.GetValue<string>("Password"), Configuration.GetValue<string>("Url")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            ConfigureAuth(app);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization(); // enforce authorization on controllers so we don't have to add [Authorize] to all controllers
            });
        }

        /// <summary>
        /// This method exists as virtual so that it can be overriden in tests
        /// </summary>
        /// <param name="app"></param>
        protected virtual void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
        }

        private string ConfigureFormsAuthentication(HttpContext context)
        {
            if (context.Request.HasFormContentType && context.Request.Path.StartsWithSegments("/api/legacy/path"))
            {
                var t = context.Request.Form["encryptedtoken"];
                var accessToken = t[0]; // decrypt the token
                var refreshToken = t[1]; // decrypt the refresh token

                if (!string.IsNullOrWhiteSpace(t) && !context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Request.Headers.Add("Authorization", new[] { string.Format("Bearer {0}", accessToken) });
                }

                if (!string.IsNullOrWhiteSpace(refreshToken) && !context.Request.Headers.ContainsKey("__RequestVerificationToken"))
                {
                    context.Request.Headers.Add("__RequestVerificationToken", new[] { refreshToken });
                }
            }

            return JwtBearerDefaults.AuthenticationScheme;
        }
    }
}
