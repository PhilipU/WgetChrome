using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class Utils
    {
        public static int Execute(string parameters, out List<string> output)
        {
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";

            output = new List<string>();
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = Path.Combine(assemblyFolder, "WgetChrome.exe"),
                    Arguments = parameters,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                output.Add(process.StandardOutput.ReadLine());
            }
            process.WaitForExit();
            return process.ExitCode;
        }

        public static void OutputContains(List<string> output, string expected)
        {
            if (!output.Any(o => o.Contains(expected)))
            {
                Assert.Fail($"Output does not contain '{expected}'");
            }

            foreach (var line in output)
            {
                Console.WriteLine(line);
            }
        }
    }
}
