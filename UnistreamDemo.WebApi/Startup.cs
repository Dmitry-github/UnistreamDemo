namespace UnistreamDemo.WebApi
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using System;
    using FluentValidation;
    using Queries;
    using Middleware;
    using Microsoft.Extensions.Logging;
    using Interfaces;
    using Services;
    using Repositories;


    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "UnistreamDemo API",
                    Version = "v1",
                    Description = "...",
                    Contact = new OpenApiContact
                    {
                        Name = "Dmitry", Email = string.Empty, Url = new Uri("https://github.com/Dmitry-github/"),
                    },
                });
            });

            //FluentValidation
            services.AddScoped<IValidator<TransactionQuery>, TransactionQueryValidator>();

            services.AddTransient<ITransactionService, TransactionService>();
            
            var clientsSetup = AppConfig.Clients();

            //services.AddTransient<ITransactionRepository, TransactionRepository>();
            //services.AddTransient<IClientRepository, ClientRepository>();

            services.AddSingleton<ITransactionRepository, TransactionRepository>();             //***Storage Emulation ONLY
            //services.AddSingleton<IClientRepository, ClientRepository>();                     //***Storage Emulation ONLY
            services.AddSingleton<IClientRepository>(_ => new ClientRepository(clientsSetup));  //***Storage Emulation With config data

            services.AddSingleton<ILogger, Logger<ProblemDetails>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UnistreamDemo API V1");
                    // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                    c.RoutePrefix = string.Empty;
                });
            }

            app.ConfigureExceptionHandler(logger);

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
