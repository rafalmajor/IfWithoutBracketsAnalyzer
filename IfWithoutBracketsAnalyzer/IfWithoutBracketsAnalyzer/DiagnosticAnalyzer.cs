using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IfWithoutBracketsAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfWithoutBracketsAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "IfWithoutBracketsAnalyzer";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, ImmutableArray.Create(SyntaxKind.IfStatement));
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext nodeContext)
        {
            var ifStatementSyntax = nodeContext.Node as IfStatementSyntax;

            if (ifStatementSyntax?.Statement is ExpressionStatementSyntax)
            {
                var diagnostic = Diagnostic.Create(Rule, ifStatementSyntax.Statement.GetLocation());
                nodeContext.ReportDiagnostic(diagnostic);
            }            
        }
    }
}
