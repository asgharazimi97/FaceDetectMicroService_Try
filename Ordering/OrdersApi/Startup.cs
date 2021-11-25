using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messaging.InterfacesConstants.Constants;
using GreenPipes;
using OrdersApi.Messages.Consumers;
using OrdersApi.Services;
using OrdersApi.Persistance;
using Microsoft.EntityFrameworkCore;

namespace OrdersApi
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
            services.AddDbContext<OrdersContext>(options => options.UseSqlServer
            (
                Configuration.GetConnectionString("OrderConnectionString")
            ));
            services.AddMassTransit(
             c =>
             {
                 c.AddConsumer<RegisterOrderCommandConsumer>();
             });
            services.AddSingleton(pro => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost", "/", h => { });
                cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, e =>
                {
                    e.PrefetchCount = 16;
                    e.UseMessageRetry(x => x.Interval(2, TimeSpan.FromSeconds(10)));
                    e.Consumer<RegisterOrderCommandConsumer>(pro);
                });
            }));

            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddSingleton<IHostedService, BusService>();
            services.AddHttpClient();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrdersApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrdersApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
