using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TestHelper;
using IfWithoutBracketsAnalyzer;

namespace IfWithoutBracketsAnalyzer.Test
{
    [TestClass]
    public class IfWithoutBracketsUnitTests : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void AnalyzeIfStatementTest_NotBeAppliedInEmptyFile()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void AnalyzeIfStatementTest_NotBeAppliedInFileWithoutIfStatement()
        {
            var testFileContent = ReadFile("Resources\\CodeFileWithoutIfStatement.txt");
            VerifyCSharpDiagnostic(testFileContent);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void AnalyzeIfStatementTest_DiagnosticAndCodeFixTriggered()
        {
            var testFileContent = ReadFile("Resources\\CodeFile.txt");
            var expected = new DiagnosticResult
            {
                Id = "IfWithoutBracketsAnalyzer",
                Message = "If with missing brackets",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                        new DiagnosticResultLocation("Test0.cs", 16, 17)
                    }
            };

            VerifyCSharpDiagnostic(testFileContent, expected);

            var fixedTestFileContent = ReadFile("Resources\\FixedCodeFile.txt");
            VerifyCSharpFix(testFileContent, fixedTestFileContent);
        }

        private static string ReadFile(string path)
        {
            string content;
            using (var streamReader = new StreamReader(path))
            {
                content = streamReader.ReadToEnd();
            }
            return content;
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new IfWithoutBracketsAnalyzerCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new IfWithoutBracketsAnalyzerAnalyzer();
        }
    }
}