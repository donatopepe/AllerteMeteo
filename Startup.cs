using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using MeteoAlert.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MeteoAlert.Models;
using MeteoAlert.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using MeteoAlert.Entities;
using System.Text;
using System.Net.Http;
using System.Net.Security;
using System.Net;
using Microsoft.Extensions.Logging;

namespace MeteoAlert
{
    public class Startup
    {
        private ILogger<Startup> _logger;
        public Startup(IConfiguration configuration)//, ILogger<Startup> logger)
        {
            Configuration = configuration;
            //_logger = logger;
        }

        public IConfiguration Configuration { get; }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            try
            {
                //initializing custom roles 
                var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                string[] roleNames = { "Admin", "Manager" };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        //create the roles and seed them to the database: Question 1
                        roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }


                //Ensure you have these values in your appsettings.json file
                string userPWD = Configuration.GetValue<string>("UserPassword");
                string user = Configuration.GetValue<string>("AdminUserEmail");
                //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>User:"+ user);
                //_logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>Pwd:" + userPWD);
                //Here you could create a super user who will maintain the web app
                var poweruser = new ApplicationUser
                {

                    UserName = user,
                    Email = user,
                    EmailConfirmed = true,
                    Name = user
                };
                var _user = await UserManager.FindByEmailAsync(user);

                if (_user == null)
                {
                    var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                    if (createPowerUser.Succeeded)
                    {
                        //here we tie the new user to the role
                        await UserManager.AddToRoleAsync(poweruser, "Admin");

                    }
                }
                else
                {
                    await UserManager.AddToRoleAsync(_user, "Admin");
                }
            }
            catch
            {
                _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>errore connessione database");
            }
        }
        /*
        private static HttpClientHandler GetHttpClientHandler()
        {
            //eventuali credenziali
            var credentialsCache = new CredentialCache();
            //credentialsCache.Add(new Uri(url), "NTLM", CredentialCache.DefaultNetworkCredentials);

            var handler = new HttpClientHandler { Credentials = credentialsCache };

            //disabilito il check del certificato
            handler.ServerCertificateCustomValidationCallback = (a, b, c, d) => true;

            return handler;
        }
        */
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*
            services.AddHttpClient(settings.HttpClientName, client =>
            {
                // code to configure headers etc..
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                if (hostingEnvironment.IsDevelopment())
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                }
                return handler;
            });
            */
            /*
            var handler = new System.Net.Http.HttpClientHandler();
            using (var httpClient = new System.Net.Http.HttpClient(handler))
            {
                handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) =>
                {
                    // Log it, then use the same answer it would have had if we didn't make a callback.
                    _logger.LogInformation(cert);
                    return errors == SslPolicyErrors.None;
                };


            }
            */
            /*

            using (var handler = GetHttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        result = await Task<T>.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(responseString));
                    }
                    else
                    {
                        _logger.LogError("Helper.HttpGetAsync: {0}", response);
                        throw new HttpRequestException(response.ToString());
                    }
                }
            }
            */

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); //Adding missing encodings
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));



            services.AddOptions();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<ElaborazioniDati>();
            services.AddSingleton<AllarmiSMS>();
            services.AddSingleton<SendSmsMail>();

            services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddSingleton<IEmailSender, EmailSender>();



            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                //options.MinimumSameSitePolicy = 0;
            });
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddControllers(config =>
            {
                // using Microsoft.AspNetCore.Mvc.Authorization;
                // using Microsoft.AspNetCore.Authorization;
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddHttpContextAccessor();
            //services.AddScoped<IFileRepository, FileRepository>();
            services.AddSingleton<IAppVersionService, AppVersionService>();
            services.AddHostedService<MyServiceReader>();
            services.AddHostedService<ManutenzioneDb>();

            //importato da LampiNet
            services.AddHostedService<WgetService>();   //PROVA PER IL TIMER CHE NON VIENE REITERATO
            services.AddTransient<IAdoMeteoService, AdoMeteoService>();
            services.AddTransient<IMsSqlService, MsSqlDbAccess>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("wwwroot/Logs/app-{Date}.txt");
            _logger = new Logger<Startup>(loggerFactory);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                _logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>ambiente in develop mode");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection(); ///disabilito ssl per non mostrare errori certificati
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            CreateRoles(serviceProvider).Wait();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
