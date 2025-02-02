using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests
{
    [TestClass]
    public class FunctionalTests
    {
        [TestMethod]
        public void ShowHelpNoParameters()
        {
            Utils.Execute("", out var output);
            Utils.OutputContains(output, "Usage:");
        }

        [TestMethod]
        public void ShowHelp()
        {
            Utils.Execute("--help", out var output);
            Utils.OutputContains(output, "Usage:");
        }

        [TestMethod]
        public void AccessUnreachableUrl()
        {
            var exitCode = Utils.Execute("192.168.255.255/unreachable output.html 1", out var output);
            Utils.OutputContains(output, "Timeout=1s");
            Utils.OutputContains(output, "Failed to navigate to page");

            Assert.AreEqual(1, exitCode);
        }

        [TestMethod]
        public void OutputPathWrong()
        {
            var exitCode = Utils.Execute("https://example.com/ ./this/path/does/not/exist/example.html 1", out var output);
            Utils.OutputContains(output, "Timeout=1s");
            Utils.OutputContains(output, "Failed to save page content");

            Assert.AreEqual(1, exitCode);
        }

        [TestMethod]
        public void SuccessfulDownloadNoParameters()
        {
            var exitCode = Utils.Execute("https://example.com/", out var output);
            Utils.OutputContains(output, "Timeout=30s");
            Utils.OutputContains(output, "Download completed successfully");

            Assert.AreEqual(0, exitCode);
        }

        [TestMethod]
        public void SuccessfulDownloadAllParameters()
        {
            var exitCode = Utils.Execute("https://example.com/ example.html 5", out var output);
            Utils.OutputContains(output, "to 'example.html'");
            Utils.OutputContains(output, "Timeout=5s");
            Utils.OutputContains(output, "Download completed successfully");

            Assert.AreEqual(0, exitCode);
        }
    }
}
