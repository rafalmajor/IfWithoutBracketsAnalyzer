using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace IfWithoutBracketsAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IfWithoutBracketsAnalyzerCodeFixProvider)), Shared]
    public class IfWithoutBracketsAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Add brackets to If statement";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(IfWithoutBracketsAnalyzerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var node = root.FindNode(context.Span);

            var expression = node as ExpressionStatementSyntax;
            if (expression == null)
            {
                return;
            }

            var action = CodeAction.Create(title, cancellationToken => this.AddBrackest(root, expression, context));

            context.RegisterCodeFix(action, context.Diagnostics.First());
        }

        private Task<Document> AddBrackest(SyntaxNode root, ExpressionStatementSyntax expression, CodeFixContext context)
        {
            var block = SyntaxFactory.Block(expression);

            var newRoot = root.ReplaceNode(expression, block);
            var newDocument = context.Document.WithSyntaxRoot(newRoot);
            return Task.FromResult(newDocument);
        }
    }
}