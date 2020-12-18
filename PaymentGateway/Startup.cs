using Framework.Configuration;
using Framework.DbContext;
using Framework.Interfaces;
using Framework.WebUtilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.WebApi.Domain;
using PaymentGateway.WebApi.Models;
using PaymentGateway.WebApi.Models.ValidationProviders;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway API");
            });

            // Small custom middlewares
            app.Use(async (context, next) =>
            {
                // Read request body for improved logging
                context.Request.EnableBuffering();
                var requestReader = new StreamReader(context.Request?.Body);
                Environment.SetEnvironmentVariable(Constants.PGRequestBody, await requestReader.ReadToEndAsync());
                context.Request.Body.Position = 0;                

                // Return header to search through logs
                context.Response.Headers.Add("TraceIdentifier", context.TraceIdentifier);
                context.Response.OnStarting(() =>
                {
                    // When exception thrown use ExceptionHandlerMiddleware
                    if (context.Response.StatusCode != 500)
                    {
                        var id = context.User.Identity;
                        Log.Information($"TraceIdentifier: {context.TraceIdentifier}\n"
                            + $"Request URI: {context.Request?.Method} {context.Request?.GetEncodedUrl()}\n"
                            + $"Request Body: {Environment.GetEnvironmentVariable(Constants.PGRequestBody)}\n"
                            + $"Status Code: {context.Response.StatusCode}\n"
                            + $"Request user: {id?.Name ?? "(anon)"}\n");
                    }

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
