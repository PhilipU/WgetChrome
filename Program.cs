using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        static async Task<bool> Download(string url, string path)
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
                await page.GoToAsync(url);
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
            catch(Exception e)
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
                Console.WriteLine("    WgetChrome.exe <URL> [PATH]");
                Console.WriteLine("        URL: The URL of the web page to download");
                Console.WriteLine("        PATH: The output file path");
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

            string path = args.Length > 1 ? args[1] : "output.html...";

            ConsoleWriteLine($"Downloading '{url}' to '{path}'");

            bool result = Download(url, path).GetAwaiter().GetResult();

            if (result)
            {
                ConsoleWriteLine("Download completed successfully");
            }
            else
            {
                ConsoleWriteLine("Download failed");
            }
        }
    }
}
