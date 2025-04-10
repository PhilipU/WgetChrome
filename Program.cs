using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WgetChrome
{
    internal class Program
    {
        static void ConsoleWriteLine(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
        static async Task<bool> Download(string url, string path, int timeout, string auth)
        {
            var browserFetcher = new BrowserFetcher();
            if (browserFetcher.GetInstalledBrowsers().ToList().Count == 0)
            {
                ConsoleWriteLine("    No web browser is installed. Downloading Google Chrome...");
                await browserFetcher.DownloadAsync();
                ConsoleWriteLine("    Finished downloading browser!");
            }
            else
            {
                ConsoleWriteLine("    Web browser is already installed.");
            }

            ConsoleWriteLine("    Launching browser...");
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            IPage page;
            try
            {
                ConsoleWriteLine($"    Navigate to '{url}'");
                page = await browser.NewPageAsync();

                if(!string.IsNullOrEmpty(auth))
                {
                    Dictionary<string, string> headers = new Dictionary<string, string>
                    {
                        { "Authorization", $"Basic {auth}" }
                    };

                    await page.SetExtraHttpHeadersAsync(headers);
                }


                var navigationTask = page.GoToAsync(url, new NavigationOptions
                {
                    Timeout = timeout * 1000,
                    WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }

                });
                if (await Task.WhenAny(navigationTask, Task.Delay(timeout * 1000)) != navigationTask)
                {
                    ConsoleWriteLine($"        Failed to navigate to page.");
                    ConsoleWriteLine($"        Error: Timeout reached");
                    return false;
                }
                await navigationTask;
            }
            catch (Exception e)
            {
                ConsoleWriteLine($"        Failed to navigate to page.");
                ConsoleWriteLine($"        Error: {e.Message}");
                return false;
            }

            ConsoleWriteLine("    Downloading content...");
            var content = await page.GetContentAsync();

            try
            {
                ConsoleWriteLine("    Saving content to file...");
                File.WriteAllText(path, content);
            }
            catch (Exception e)
            {
                ConsoleWriteLine($"        Failed to save page content.");
                ConsoleWriteLine($"        Error: {e.Message}");
                return false;
            }

            return true;
        }

        static void Main(string[] args)
        {
            // Check if no arguments are given or first argument is '--help' then show help message
            if (args.Length == 0 || args[0] == "--help")
            {
                Console.WriteLine("WgetChrome - A simple tool to download rendered web pages using Google Chrome");
                Console.WriteLine("Usage:");
                Console.WriteLine("    WgetChrome.exe <URL> [OPTIONS]");
                Console.WriteLine("        URL: The URL of the web page to download");
                Console.WriteLine("        OPTIONS: Additional Options as Key=Value pair");
                Console.WriteLine("            |   Key   |   Value                   |");
                Console.WriteLine("            |---------|---------------------------|");
                Console.WriteLine("            | path    | The output file path      |");
                Console.WriteLine("            | timeout | Timeout in seconds        |");
                Console.WriteLine("            | auth    | Basic Authorization Token |");
                return;
            }

            string url = args[0];

            // If given URL is not prefixed with 'http://' or 'https://' then prefix it with 'http://'
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "http://" + url;
            }

            // Check if the first argument is a valid URL
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                Console.WriteLine($"Invalid URL '{url}'");
                return;
            }

            // Options
            //--------
            string path = "output.html";
            int timeout = 30;
            string auth = "";

            // Options Arguments (Key-Value Pairs)
            //------------------------------------
            List<string> processedArgs = new List<string>();

            for(int i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith("path=")) path = args[i].Split('=')[1];
                else if (args[i].StartsWith("timeout=")) timeout = int.Parse(args[i].Split('=')[1]);
                else if (args[i].StartsWith("auth=")) auth = args[i].Split('=')[1];
                else
                {
                    processedArgs.Add(args[i]);
                }
            }

            // Legacy Arguments
            //-----------------
            {
                if(processedArgs.Count > 0) path = processedArgs[0];
                if(processedArgs.Count > 1) timeout = int.Parse(processedArgs[1]);
            }

            string authInfo = "";
            if(!string.IsNullOrEmpty(auth))
            {
                authInfo = "Authorization=True";
            }

            ConsoleWriteLine($"Downloading '{url}' to '{path}'. Timeout={timeout}s. {authInfo}");

            bool result = Download(url, path, timeout, auth).GetAwaiter().GetResult();

            if (result)
            {
                ConsoleWriteLine("Download completed successfully");
                Environment.Exit(0);
            }
            else
            {
                ConsoleWriteLine("Download failed");
                Environment.Exit(1);
            }
        }
    }
}
