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
    }
}
