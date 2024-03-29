﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Broker.Bindings;
using Broker.Instances;
using idunno.Authentication.Basic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenServiceBroker;
using OpenServiceBroker.Bindings;
using OpenServiceBroker.Catalogs;
using OpenServiceBroker.Instances;
using Steeltoe.Extensions.Configuration.CloudFoundry;

namespace Broker
{
    public class Startup
    {

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services
                .AddMvc(options =>
                {
                    // Add authorize filter globally for all controllers.
                    options.Filters.Add(new AuthorizeFilter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .AddTransient<ICatalogService, CatalogService>()
                .AddTransient<IServiceInstanceBlocking, ServiceInstanceBlocking>()
                .AddTransient<IServiceBindingBlocking, ServiceBindingBlocking>()
                .AddOpenServiceBroker();

            services
                .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasic(options =>
                {
                    // We need to allow insecure basic http authentication, which is generally a bad idea.
                    // However, PCF terminates SSL at the load balancer and sends requests to the application
                    // via plain http so we have no choice.
                    options.AllowInsecureProtocol = true;

                    options.Realm = "broker";
                    options.Events = new BasicAuthenticationEvents
                    {
                        OnValidateCredentials = context =>
                        {
                            var password = Configuration["Authentication:Password"];
                            if (context.Username == "mybrokeruser" && context.Password == password)
                            {
                                // Generate principal.
                                var claims = new[]
                                {
                                    new Claim(ClaimTypes.NameIdentifier,
                                        context.Username, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name,
                                        context.Username, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                                };
                                context.Principal = new ClaimsPrincipal(
                                    new ClaimsIdentity(
                                        claims,
                                        context.Scheme.Name
                                    )
                                );

                                context.Success();
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // Add Cloud Foundry options.
            services.ConfigureCloudFoundryOptions(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();
        }

    }
}
