using DpdtInject.Generator.Core.Binding;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed
{
    public interface IParsedBindExpression
    {
        void Validate();

        IBindingContainer CreateBindingContainer(
            );
    }
}
