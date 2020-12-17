using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Configuration;
using Framework.DbContext;
using Framework.Interfaces;
using Framework.WebUtilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentGateway.WebApi.Domain;
using PaymentGateway.WebApi.Models;
using PaymentGateway.WebApi.Models.ValidationProviders;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace PaymentGateway.WebApi
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
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
                {
                    config.Cookie.Name = "Payments.Cookie";
                    config.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                });

            // Register the Swagger generator
            services.AddSwaggerGen();

            // DI Registrations
            services.AddTransient(typeof(IPaymentBLL), typeof(PaymentBLL));
            services.AddTransient(typeof(IUserBLL), typeof(UserBLL));

            services.AddTransient(typeof(IDatabaseContext), typeof(DatabaseContext));
            services.AddTransient(typeof(IHttpClientWrapper), typeof(HttpClientWrapper));

            services.AddTransient(typeof(IValidate<PaymentModel>), typeof(PaymentModelValidationProvider));
            services.AddTransient(typeof(IValidate<UserModel>), typeof(UserModelValidationProvider));

            services.AddSingleton(typeof(ILiteDbWrapper), typeof(LiteDbWrapper));
            services.AddSingleton(typeof(IPasswordHasher<UserModel>), typeof(PasswordHasher<UserModel>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Setup middleware to handle application exceptions
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.UseMiddleware<ExceptionHandlerMiddleware>();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            // Small custom middlewares
            app.Use(async (context, next) =>
            {
                // Add TraceIdentifier header when sending response
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add("TraceIdentifier", context.TraceIdentifier);
                    return Task.CompletedTask;
                });
                await next.Invoke();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            // Authenticate first to see who the user is
            app.UseAuthentication();
            // Authorise then if access allowed
            app.UseAuthorization();       

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Load defaults from appsettings.json
            var appConfig = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Set environment config values
            PGConfiguration.SetKeyValues(appConfig.GetChildren());
        }
    }
}
