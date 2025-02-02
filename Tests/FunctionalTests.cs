using NUnit.Framework;
using System;

namespace Tests
{
    [TestFixture]
    public class FunctionalTests
    {
        [Test]
        public void ShowHelpNoParameters()
        {
            Utils.Execute("", out var output);
            Utils.OutputContains(output, "Usage:");
        }

        [Test]
        public void ShowHelp()
        {
            Utils.Execute("--help", out var output);
            Utils.OutputContains(output, "Usage:");
        }

        [Test]
        public void AccessUnreachableUrl()
        {
            var exitCode = Utils.Execute("192.168.255.255/unreachable output.html 1", out var output);
            Utils.OutputContains(output, "Timeout=1s");
            Utils.OutputContains(output, "Failed to navigate to page");

            Assert.That(1 == exitCode);
        }

        [Test]
        public void OutputPathWrong()
        {
            var exitCode = Utils.Execute("https://example.com/ ./this/path/does/not/exist/example.html 1", out var output);
            Utils.OutputContains(output, "Timeout=1s");
            Utils.OutputContains(output, "Failed to save page content");

            Assert.That(1 == exitCode);
        }

        [Test]
        public void SuccessfulDownloadNoParameters()
        {
            var exitCode = Utils.Execute("https://example.com/", out var output);
            Utils.OutputContains(output, "Timeout=30s");
            Utils.OutputContains(output, "Download completed successfully");

            Assert.That(0 == exitCode);
        }

        [Test]
        public void SuccessfulDownloadAllParameters()
        {
            var exitCode = Utils.Execute("https://example.com/ example.html 5", out var output);
            Utils.OutputContains(output, "to 'example.html'");
            Utils.OutputContains(output, "Timeout=5s");
            Utils.OutputContains(output, "Download completed successfully");

            Assert.That(0 == exitCode);
        }
    }
}
