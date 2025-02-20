using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Radzen;

namespace BandTogether.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddSingleton<BlazorDataModel>();

            builder.Services.AddRadzenComponents();

            builder.Services.AddBlazorContextMenu();

            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
        }
    }
}
