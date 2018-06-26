using System;
using System.IO;
using NUnit.Framework;

namespace p20_talos_test
{
    [SetUpFixture]
    public class SetupClass
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var dir = Path.GetDirectoryName(typeof(SetupClass).Assembly.Location);
            Directory.SetCurrentDirectory(dir);
        }
    }
}