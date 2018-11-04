using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using ProArch.CodingTest.Common.Repository;
using ProArch.CodingTest.Common.Services;
using ProArch.CodingTest.Repository;
using ProArch.CodingTest.Services;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace ProArch.CodingTest.Api
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "ProArch.CodingTest", Version = "v1" });
            });


            int breakDuration = Configuration.GetValue<int>("DurationOfCircuitBreakInSeconds");
            int exceptionCount = Configuration.GetValue<int>("ExceptionsAllowedBeforeBreak");
            var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreaker(exceptionCount, TimeSpan.FromSeconds(breakDuration));

            services.AddSingleton<Policy>(_ => circuitBreakerPolicy);
            services.AddDbContext<IProArchDbContext, ProArchDbContext>(options =>
                     options.UseSqlServer(Configuration.GetConnectionString("proarch")));

            services.AddScoped<IInvoiceRespository, InvoiceRespository>();
            services.AddScoped<ISpendService, SpendService>();
            services.AddScoped<IInvoiceService, Services.ExternalInvoiceService>();
            services.AddScoped<IInvoiceService, InternalInvoiceService>();
            services.AddScoped<IInvoiceService>(_ => new FailoverInvoiceService(Configuration.GetValue<int>("FailoverValidDays")));
            services.AddScoped<IInvoiceServiceStrategy>(s =>
            {
                return new InvoiceServiceStrategy(s.GetServices<IInvoiceService>());
            });
            services.AddScoped<ISupplierRespository, SupplierRespository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProArch.CodingTest");
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
