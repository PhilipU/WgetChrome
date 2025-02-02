using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class Utils
    {
        public static int Execute(string parameters, out List<string> output)
        {
            output = new List<string>();
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "WgetChrome.exe",
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
        }
    }
}
