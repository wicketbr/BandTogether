using BandTogether.Client.Pages;
using BandTogether.Components;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace BandTogether
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string argumentFolder = string.Empty;

            if (args.Any()) {
                int index = -1;
                int total = args.Length;
                foreach (var arg in args) {
                    index++;

                    string nextArg = String.Empty;
                    if (index < total) {
                        try {
                            nextArg = args[index + 1];
                        }catch { }
                    }

                    switch(arg.ToLower()) {
                        case "-f":
                        case "-folder":
                            argumentFolder = nextArg;
                            break;
                    }
                }
            }

            bool windows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            bool mac = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
            bool linux = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);

            var builder = WebApplication.CreateBuilder(args);

            // Get the launch Url from appsettings.json
            var launchUrl = builder.Configuration.GetValue<string>("LaunchUrl");

            var isDevelopment = builder.Environment.IsDevelopment();
            if (!isDevelopment) {
                if (!String.IsNullOrWhiteSpace(launchUrl)) {
                    builder.WebHost.UseUrls(launchUrl);
                } else if (windows) {
                    builder.WebHost.UseUrls("http://0.0.0.0:5000");
                    //builder.WebHost.UseIISIntegration();
                } else if (mac) {
                    builder.WebHost.UseUrls("http://localhost:5000");
                }
            }

            builder.Logging.ClearProviders();

            //builder.WebHost.ConfigureLogging((context, logging) => {
            //    logging.ClearProviders();
            //});

            var applicationPath = AppDomain.CurrentDomain.BaseDirectory;
            
            // For Windows the basePath default can be a folder below the application.
            var basePath = System.IO.Path.Combine(applicationPath, "Data");

            // However, for Mac, the permissions won't allow this, so default to the user's home folder.
            if (mac) {
                basePath = "BandTogether";
            }

            builder.WebHost.UseKestrelCore();

            //builder.WebHost.UseKestrelCore().ConfigureKestrel(options => {
            //    options.ListenAnyIP(5000);
            //});

            builder.Services.AddControllersWithViews();

            builder.Services.AddSignalR();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddRazorPages();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSingleton<IServiceProvider>(provider => provider);

            if (!String.IsNullOrWhiteSpace(argumentFolder)) {
                basePath = argumentFolder;
            } else {
                // No folder was specified in the command-line arguments, check if a folder is configured in the appsettings.json file.
                var configuredFolder = builder.Configuration.GetValue<string>("DataPath");
                if (!String.IsNullOrWhiteSpace(configuredFolder)) {
                    basePath = configuredFolder;
                }
            }

            var da = new DataAccess(applicationPath, basePath);

            builder.Services.AddTransient<IDataAccess>(x => ActivatorUtilities.CreateInstance<DataAccess>(x, applicationPath, basePath, x.GetRequiredService<IServiceProvider>()));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseWebAssemblyDebugging();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            //app.MapStaticAssets();

            if (mac) {
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                provider.Mappings[".dat"] = "application/octet-stream";

                app.UseStaticFiles(
                    new StaticFileOptions {
                        FileProvider = new PhysicalFileProvider(Path.Combine(applicationPath, "wwwroot")), 
                        RequestPath = "",
                        ContentTypeProvider = provider,
                    }
                );
            } else {
                app.UseStaticFiles();
            }

            app.UseRouting();

            app.UseAntiforgery();

            app.MapHub<signalRhub>("/hub", signalRConnctionOptions => {
                signalRConnctionOptions.AllowStatefulReconnects = true;
            });

            app.MapRazorPages();

            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(BandTogether.Client.Pages.Home).Assembly);

            app.Start();
            var urls = app.Urls;

            // Draw the splash screen to the console.
            Console.WriteLine("");

            var bgColor = Console.BackgroundColor;
            var fgColor = Console.ForegroundColor;
            Console.OutputEncoding = Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            // https://patorjk.com/software/taag/#p=display&f=Graffiti&t=Type%20Something%20

            var splash = new List<string>{
                //@"",
                @"    ____                  _ _____                _   _                 ",
                @"   | __ )  __ _ _ __   __| |_   _|__   __ _  ___| |_| |__   ___ _ __   ",
                @"   |  _ \ / _` | '_ \ / _` | | |/ _ \ / _` |/ _ \ __| '_ \ / _ \ '__|  ",
                @"   | |_) | (_| | | | | (_| | | | (_) | (_| |  __/ |_| | | |  __/ |     ",
                @"   |____/ \__,_|_| |_|\__,_| |_|\___/ \__, |\___|\__|_| |_|\___|_|     ",
                @"                                      |___/                            ",
                @"   v " + da.Version + "  ",
                @"   © " + da.Released.Year.ToString() + " Bradley R. Wickett  ",
                @"   Data Path: " + basePath + "  ",
                @"     Started: " + DateTime.Now.ToString(),
                @"",
            };

            string ipAddress = String.Empty;
            try {
                var ipAddresses = IpTools.GetIpAddresses();
                if (ipAddresses.Count == 1) {
                    ipAddress = ipAddresses[0];
                }
            } catch { }

            string computerName = System.Environment.MachineName;
            if (!String.IsNullOrWhiteSpace(computerName)) {
                // Use this instead of the ipAddress
                ipAddress = computerName;
            }

            foreach (var url in urls) {
                var showUrl = url;
                if (!String.IsNullOrWhiteSpace(showUrl)) {
                    if (showUrl.Contains("0.0.0.0") && !String.IsNullOrWhiteSpace(ipAddress)) {
                        showUrl = showUrl.Replace("0.0.0.0", ipAddress);
                    }
                }

                splash.Add("         URL: " + showUrl);
            }
            splash.Add("");
            splash.Add("   Press Ctrl+C to exit.");
            splash.Add("");

            var longest = splash.Max(x => x.Length);
            foreach (var line in splash) {
                Console.WriteLine(line.PadRight(longest));
            }

            Console.WriteLine("");
            Console.ResetColor();
            Console.WriteLine("");


            app.WaitForShutdown();

            //app.Run();

        }
    }

    public static class IpTools
    {
        public static List<string> GetIpAddresses()
        {
            var output = new List<string>();

            var hostName = System.Net.Dns.GetHostName();
            if (!String.IsNullOrWhiteSpace(hostName)) {
                var hostEntry = System.Net.Dns.GetHostEntry(hostName);
                if (hostEntry != null && hostEntry.AddressList.Any()) {
                    foreach (var entry in hostEntry.AddressList) {
                        var address = entry.ToString();
                        if (!String.IsNullOrWhiteSpace(address) && !address.Contains(":") && !address.StartsWith("172.")) {
                            output.Add(address);
                        }
                    }
                }
            }

            return output;
        }
    }
}
