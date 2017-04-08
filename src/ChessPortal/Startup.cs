using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChessPortal.DataInterfaces;
using ChessPortal.DtoProviders;
using ChessPortal.Entities;
using ChessPortal.Handlers;
using ChessPortal.Models.Dtos;
using ChessPortal.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ChessPortal
{
    public class Startup
    {

        public static IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
               .AddMvcOptions(o =>
                   o.OutputFormatters.Add(
                       new XmlDataContractSerializerOutputFormatter()));
            var connectionString = Startup.Configuration["connectionStrings:ChessPortalDBConnectionString"];
            services.AddDbContext<ChessPortalContext>(o => o.UseSqlServer(connectionString));
            //services.AddScoped<ICityInfoRepoistory, CityInfoRepository>();

            services.AddIdentity<ChessPlayer, IdentityRole>(options => options.Cookies.ApplicationCookie.AutomaticChallenge = false)
                .AddEntityFrameworkStores<ChessPortalContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IChessPortalRepository, ChessPortalRepository>();
            services.AddScoped<IChallengeHandler, ChallengeHandler>();
            services.AddScoped<IChallengeDtoProvider, ChallengeDtoProvider>();
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("CityVisited", policy => policy.Requirements.Add(new CityVisitedRequirement()));
            //});

            //services.AddSingleton<IAuthorizationHandler, CityVisitedHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            loggerFactory.AddDebug();

            //loggerFactory.AddProvider(new NLogLoggerProvider());
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            app.UseStatusCodePages();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ChallengeDto, ChallengeEntity>();
                cfg.CreateMap<ChallengeEntity, ChallengeDto>();
                cfg.CreateMap<MoveDto, MoveEntity>();
                cfg.CreateMap<MoveEntity, MoveDto>();
                cfg.CreateMap<DrawRequestDto, DrawRequestEntity>();
                cfg.CreateMap<DrawRequestEntity, DrawRequestDto>();
            });

            app.UseIdentity();

            app.UseMvc();
        }
    }
}
