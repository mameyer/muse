using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using BackgroundServices.Services;
using BackgroundServices.Interfaces;
using BackgroundServices.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Muse.Helpers;
using Microsoft.EntityFrameworkCore;
using Localization.SqlLocalizer.DbStringLocalizer;
using System.Text.Json;
using Newtonsoft.Json.Serialization;

namespace Muse
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
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
            })

            .AddSpotify(options =>
            {
                options.ClientId = Configuration["ClientId"];
                options.ClientSecret = Configuration["ClientSecret"];
                options.CallbackPath = Configuration["CallbackPath"];
                options.SaveTokens = true;
                options.Scope.Add("playlist-read-private");
                options.Scope.Add("playlist-read-collaborative");
                options.Scope.Add("user-read-currently-playing");
                options.Scope.Add("user-read-playback-state");
                options.Scope.Add("user-modify-playback-state");
                options.Scope.Add("user-library-read");
                options.Scope.Add("user-top-read");
                options.Scope.Add("user-read-recently-played");
            });

            Configuration["SpotifyApiClientId"] = Configuration["ClientId"];
            Configuration["SpotifyApiClientSecret"] = Configuration["ClientSecret"];

            // localization
            var sqlConnectionString = Configuration["ConnectionString:Localization"];

            services.AddDbContext<LocalizationModelContext>(options =>
                options.UseSqlServer(
                    sqlConnectionString,
                    b => b.MigrationsAssembly("Muse")
                ),
                ServiceLifetime.Singleton,
                ServiceLifetime.Singleton
            );

            services.AddDbContext<MuseContext>
                (options =>
                {
                    options.UseNpgsql(
                        Configuration["ConnectionString:MuseDB"],
                        b => b.MigrationsAssembly("Muse"));
                });

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                })
                .AddRazorRuntimeCompilation();

            // Requires that LocalizationModelContext is defined
            // services.AddSqlLocalization<LocalizationItem>(options =>
            // {
            //     options.UseTypeFullNames = true;
            //     options.AddLocalizedItemHandler =
            //         (localizedRecord, context) =>
            //         {
            //             var ctx = (LocalizationContext)context;
            //             ctx.Add(localizedRecord);
            //             ctx.SaveChanges();
            //             return true;
            //         };
            // });

            services.AddHttpContextAccessor();

            services.AddSingleton<BackgroundServices.Interfaces.ILoggingService, Services.TaskLoggingService>();

            services.AddSingleton<IPlaylistsTaskQueue, PlaylistsTaskQueue>();
            services.AddHostedService<PlaylistsService>();

            services.AddSingleton<INewReleasesTaskQueue, NewReleasesTaskQueue>();
            services.AddHostedService<NewReleasesService>();

            services.AddMvc()
                .AddSessionStateTempDataProvider();

            services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);

            services.AddSession();
            services.AddSignalR();
        }

        public class PlaylistTask
            : BackgroundTaskBase<int>
        {
            private readonly ILogger<PlaylistTask> logger;

            public PlaylistTask(ILoggerFactory loggerFactory)
                : base("PlaylistTask", true, 2000)
            {
                if (loggerFactory != null)
                {
                    logger = loggerFactory.CreateLogger<PlaylistTask>();
                }
            }

            public override Action Action()
            {
                return () => 
                {
                    var cnt = this.In;

                    Task.Delay(1000).Wait();

                    cnt++;

                    logger.LogDebug(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffff") + " =>  run task '" + this.Name + "': " + cnt);
                    Out = cnt;
                    Result = cnt;
                };
            }
        }

        public class NewReleasesTask
            : BackgroundTaskBase<int>
        {
            private readonly ILogger<NewReleasesTask> logger;

            private HttpClient httpClient;
            // private readonly IHttpContextAccessor httpContextAccessor;

            public NewReleasesTask(ILoggerFactory loggerFactory)
                : base("NewReleasesTask", true, 4000)
            {
                httpClient = new HttpClient();
                // this.httpContextAccessor = httpContextAccessor;
                if (loggerFactory != null)
                {
                    logger = loggerFactory.CreateLogger<NewReleasesTask>();
                }
            }

            public override Action Action()
            {
                return () => 
                {
                    var cnt = this.In;

                    Task.Delay(2000).Wait();

                    // var accessToken = httpContextAccessor.HttpContext.GetTokenAsync("Spotify", "access_token").Result;
            
                    // var api = new PersonalizationApi(this.httpClient, accessToken);

                    // var data = api.GetTopArtists(accessToken).Result;

                    // logger.LogDebug(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffff") + " =>  data: " + data.Items);

                    cnt++;

                    logger.LogDebug(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss.ffff") + " =>  run task '" + this.Name + "': " + cnt);
                    Out = cnt;
                    Result = cnt;
                };
            }
        }

        public interface IPlaylistsTaskQueue : IBackgroundTaskQueue<int>
        {  
        }

        public interface INewReleasesTaskQueue : IBackgroundTaskQueue<int>
        {  
        }

        public class PlaylistsTaskQueue : BackgroundTaskQueue<int>, IPlaylistsTaskQueue
        {
        }

        public class NewReleasesTaskQueue : BackgroundTaskQueue<int>, INewReleasesTaskQueue
        {
        }

        public class PlaylistsService : QueuedHostedService<int>
        {
            public PlaylistsService(IPlaylistsTaskQueue taskQueue, ILoggerFactory loggerFactory,
                BackgroundServices.Interfaces.ILoggingService loggingService)
                : base(taskQueue, loggerFactory)
            {
                RegisterLoggingService(loggingService);
            }
        }

        public class NewReleasesService : QueuedHostedService<int>
        {
            public NewReleasesService(INewReleasesTaskQueue taskQueue, ILoggerFactory loggerFactory,
                BackgroundServices.Interfaces.ILoggingService loggingService)
                : base(taskQueue, loggerFactory)
            {
                RegisterLoggingService(loggingService);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IPlaylistsTaskQueue taskQueueFirst, INewReleasesTaskQueue taskQueueSecond,
            ILoggerFactory loggerFactory)
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

            // app.UseHttpsRedirection();
            app.UseSignalR(routes =>
            {
                routes.MapHub<Hubs.CurrentlyPlayingHub>("/currentlyPlaying");
                routes.MapHub<Hubs.TaskLoggingHub>("/taskLogging");
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                //context.Response.Headers.Add("X-Frame-Options", "open.spotify.com");
                context.Response.Headers.Add("X-Frame-Options", "AllowAll");
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Player}/{action=Index}/{id?}");
            });

            app.UseSession();

            //taskQueueFirst.Enqueue(new PlaylistTask(loggerFactory), true, false, true);
            //taskQueueSecond.Enqueue(new NewReleasesTask(loggerFactory), true, false, true);
        }
    }
}
