using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.GivingToCustomer.Core.Services;
using Pcf.GivingToCustomer.DataAccess;
using Pcf.GivingToCustomer.DataAccess.Data;
using Pcf.GivingToCustomer.DataAccess.Repositories;
using Pcf.GivingToCustomer.Integration;
using Pcf.GivingToCustomer.WebHost.Consumers;
using Pcf.Shared.Contracts;
using System;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Pcf.GivingToCustomer.WebHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddMvcOptions(x =>
                x.SuppressAsyncSuffixInActionNames = false);
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<INotificationGateway, NotificationGateway>();
            services.AddScoped<IDbInitializer, EfDbInitializer>();
            services.AddDbContext<DataContext>(x =>
            {
                //x.UseSqlite("Filename=PromocodeFactoryGivingToCustomerDb.sqlite");
                x.UseNpgsql(Configuration.GetConnectionString("PromocodeFactoryGivingToCustomerDb"));
                x.UseSnakeCaseNamingConvention();
                x.UseLazyLoadingProxies();
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddOpenApiDocument(options =>
            {
                options.Title = "PromoCode Factory Giving To Customer API Doc";
                options.Version = "1.0";
            });

            services.AddScoped<IPromoCodeService, PromoCodeService>();
            services.AddMassTransit(x =>
            {
                x.AddConsumer<PromoCodeConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration["RMQSettings:Host"],
                        Configuration["RMQSettings:VHost"], h =>
                        {
                            h.Username(Configuration["RMQSettings:Login"]);
                            h.Password(Configuration["RabRMQSettingsbitMq:Password"]);
                        });
                  
                    cfg.ReceiveEndpoint("promocode-received-giving-to-customer_1", e =>
                    {
                        e.ConfigureConsumer<PromoCodeConsumer>(context);
                    });
                });
            });

            //services.AddHttpClient<IPreferencesGateway, PreferencesGateway>(c =>
            //{
            //    c.BaseAddress = new Uri(Configuration["IntegrationSettings:ReferencesInfoApiUrl"]);
            //});

            services.AddGrpc(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            dbInitializer.InitializeDb();
        }
    }
}