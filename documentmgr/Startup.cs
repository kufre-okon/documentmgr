using documentmgr.data;
using documentmgr.data.Repositories.Infrastructure;
using documentmgr.data.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using documentmgr.business.Utilities;
using Microsoft.EntityFrameworkCore;
using documentmgr.business.Services;
using documentmgr.dto;

namespace documentmgr
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {

            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 2147483648;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(
                                Configuration.GetConnectionString("DefaultConnection")));

            // configure unitof work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddControllersWithViews(options =>
            {
                options.AllowEmptyInputInBodyModelBinding = true;
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            })
            .AddRazorRuntimeCompilation();

            services.AddRazorPages();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddSingleton(Log.Logger);
            services.AddSingleton(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
           
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
                options.SlidingExpiration = true;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;

                // if we are accessing the /api and an unauthorized request is made
                // do not redirect to the login page, but simply return "Unauthorized"
                options.Events.OnRedirectToLogin = ctx =>
                {
                    if (ctx.Request.IsAjaxRequest())
                        ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

                    return Task.FromResult(0);
                };
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Unspecified;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllHeaders",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
            });

            services.AddHttpContextAccessor();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IFileUtility, FileUtility>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<IUserService, UserService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}