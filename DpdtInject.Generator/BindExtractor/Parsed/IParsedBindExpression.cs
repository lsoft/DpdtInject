using DpdtInject.Generator.Binding;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.BindExtractor.Parsed
{
    public interface IParsedBindExpression
    {
        ExpressionStatementSyntax ExpressionNode
        {
            get;
        }

        BindScopeEnum Scope
        {
            get;
        }

        void Validate();

        IBindingContainer CreateBindingContainer(
            );
    }
}
