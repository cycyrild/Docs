using DocsWASM.Pages.LoginRegister;
using DocsWASM.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DocsWASM.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
    public class Helper
    {
        public static string ApplicationExeDirectory()
        {
            var location = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var appRoot = Path.GetDirectoryName(location);

            return appRoot;
        }

        public static string sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();
            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));
                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }


        public static string DBConnectionString()
        {
            return GetAppSettings()["ConnectionStrings:Default"];
        }

        public static (string mail, string password) SmtCredentials()
        {
            var settings = GetAppSettings();
            return (settings["SmtpCredential:Mail"], settings["Password"]);

        }

        public static IConfigurationRoot GetAppSettings()
        {
            string applicationExeDirectory = ApplicationExeDirectory();

            var builder = new ConfigurationBuilder()
            .SetBasePath(applicationExeDirectory)
            .AddJsonFile("appsettings.json");

            return builder.Build();
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
        {
			services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddServerSideBlazor();
			services.AddTransient<AppDb>(_ => new AppDb(Configuration["ConnectionStrings:Default"]));

			services.AddHttpContextAccessor();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
				// Specify where to redirect un-authenticated users
				//options.LoginPath = "/login";
				options.LoginPath = string.Empty;
				options.AccessDeniedPath = string.Empty;
				options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);

                // Specify the name of the auth cookie.
                options.Cookie.Name = "auth";
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;

				options.Events = new CookieAuthenticationEvents
                {
                    OnValidatePrincipal = ValidateAsync
                };
            });
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
		}

        private static async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            var userCookie = context?.Principal?.Identity as ClaimsIdentity;
            var userId = userCookie?.FindFirst(ClaimTypes.NameIdentifier);
            var userHash = userCookie?.FindFirst(ClaimTypes.Hash);

            if (userCookie != null && userId != null && userHash != null && ulong.TryParse(userId.Value, out ulong userIdUlong) &&
                await new LoginRegisterSQL().CheckSession(userIdUlong, userHash.Value))
                return;
            else
				context.RejectPrincipal();
        }

    }

}