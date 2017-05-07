using ChessPortal.Data.DtoProviders;
using ChessPortal.Data.Entities;
using ChessPortal.Data.Handlers;
using ChessPortal.Data.Repositories;
using ChessPortal.Data.Repositories.Interfaces;
using ChessPortal.Data.Services;
using ChessPortal.Data.Settings;
using ChessPortal.Infrastructure.DataInterfaces;
using ChessPortal.Infrastructure.Dtos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using NLog.Web;

namespace ChessPortal.Web
{
    public class Startup
    {

        public static IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env)
        {
            env.ConfigureNLog("nlog.config");

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
               .AddJsonOptions(o =>
                       o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                       .AddMvcOptions(o =>
                   o.OutputFormatters.Add(
                       new XmlDataContractSerializerOutputFormatter()));
            var connectionString = Startup.Configuration["ConnectionStrings:ChessPortalDBConnectionString"];
            services.AddDbContext<ChessPortalContext>(o => o.UseSqlServer(connectionString));

            services.AddIdentity<ChessPlayer, IdentityRole>(options => options.Cookies.ApplicationCookie.AutomaticChallenge = false)
                .AddEntityFrameworkStores<ChessPortalContext>()
                .AddDefaultTokenProviders();

            services.AddOptions();

            services.Configure<ChessProblemSettings>(Configuration.GetSection("ChessProblemSettings"));

            services.AddScoped<IChessPortalRepository, ChessPortalRepository>();
            services.AddScoped<IChallengeHandler, ChallengeHandler>();
            services.AddScoped<IChallengeDtoProvider, ChallengeDtoProvider>();
            services.AddScoped<IGameDtoProvider, GameDtoProvider>();
            services.AddScoped<IChessProblemHandler, ChessProblemHandler>();
            services.AddScoped<IAccountHandler, AccountHandler>();
            services.AddScoped<IChessPlayerDtoProvider, ChessPlayerDtoProvider>();

            services.AddSingleton<IChessProblemService, ChessProblemService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            app.AddNLogWeb();

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
                cfg.CreateMap<ChallengeEntity, GameDto>();
                cfg.CreateMap<MoveDto, MoveEntity>();
                cfg.CreateMap<MoveEntity, MoveDto>();
                cfg.CreateMap<DrawRequestDto, DrawRequestEntity>();
                cfg.CreateMap<DrawRequestEntity, DrawRequestDto>();
                cfg.CreateMap<ChessPlayer, ChessPlayerDto>();
                cfg.CreateMap<ChessPlayerDto, ChessPlayer>();
            });

            app.UseIdentity();

            app.UseMvc();
        }
    }
}
