using DocsWASM.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Headers;
using DocsWASM.Client.AppState;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace DocsWASM.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Services.AddScoped<Actions>();
			builder.Services.AddScoped<Session>();
			builder.Services.AddScoped<DocumentZoom>();
            builder.Services.AddScoped<UserTypes>();
            builder.Services.AddPWAUpdater();

            builder.Services.AddScoped(sp =>
            {
                var hc = new HttpClient(new HttpClientHandler {AllowAutoRedirect = false});
                hc.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/");
                hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/bson"));
				return hc;
            });

            await builder.Build().RunAsync();
        }
    }
}