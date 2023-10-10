using System.IO;
using NUnit.Framework;

namespace PDFGenUnitTests
{
    public static class PathUtils
    {
        private static readonly string ProjectPath;

        public static string TestFilesPath { get; }

        public static string TestOutputPath { get; }

        static PathUtils()
        {
            ProjectPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "..\\..");
            TestFilesPath = Path.Combine(ProjectPath, "TestFiles");
            TestOutputPath = Path.Combine(ProjectPath, "TestOutput");
        }
    }
}
