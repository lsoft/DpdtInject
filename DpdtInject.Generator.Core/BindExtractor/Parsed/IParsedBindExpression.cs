using DpdtInject.Generator.Core.Binding;
using DpdtInject.Injector.Bind;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed
{
    public interface IParsedBindExpression
    {
        void Validate();

        IBindingContainer CreateBindingContainer(
            );
    }
}
