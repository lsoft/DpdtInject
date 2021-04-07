using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Generator.Core.Producer;

namespace DpdtInject.Extension.Machinery.Add
{
    public class BindClauseProducer
    {
        private readonly NewBindingInfo _newBindingInfo;

        public BindClauseProducer(
            NewBindingInfo newBindingInfo
            )
        {
            if (newBindingInfo is null)
            {
                throw new ArgumentNullException(nameof(newBindingInfo));
            }

            _newBindingInfo = newBindingInfo;
        }

        public StatementSyntax ProduceBinding(
            string leadingTrivia
            )
        {
            var indend1 = leadingTrivia + "    ";
            var indend2 = indend1 + "    ";

            var clauses = new List<string>();

            var bindFroms = string.Join(",", _newBindingInfo.BindFroms.Select(b => b.ToGlobalDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            clauses.Add($"Bind<{bindFroms}>()");

            if (_newBindingInfo.BindScope != BindScopeEnum.Constant)
            {
                clauses.Add($"{indend2}.To<{_newBindingInfo.BindTo.ToGlobalDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}>()");
                clauses.Add($"{indend2}.With{_newBindingInfo.BindScope}Scope()");
            }
            else
            {
                clauses.Add($"{indend2}.WithConstScope(/* place here your (static) readonly field */)");
            }


            if (_newBindingInfo.IsConditional)
            {
                clauses.Add($"{indend2}.When(rt => /* compose predicate against rt */ )");
            }

            foreach (var mca in _newBindingInfo.ManualConstructorArguments)
            {
                clauses.Add($"{indend2}.Configure(new ConstructorArgument(\"{mca.Name}\", /* your parameter value */))");
            }

            var clause = string.Join(Environment.NewLine, clauses);

            return
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.ParseExpression(
                        clause
                        )
                    ).WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithLeadingTrivia(SyntaxFactory.Whitespace(indend1))
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                ;

        }
    }
}
