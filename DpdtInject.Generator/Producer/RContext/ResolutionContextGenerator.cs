using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer.RContext
{
    public static class ResolutionContextGenerator
    {
        public static string GetContextStableName(
            this InstanceContainerGenerator generator,
            ITypeSymbol bindFrom
            )
        {
            if (bindFrom is null)
            {
                throw new System.ArgumentNullException(nameof(bindFrom));
            }

            var bindFromTypeFullName = bindFrom.GetFullName();
            var bindToTypeFullName = generator.BindingContainer.BindToType.GetFullName();

            var createContextVariableName = $"Context_{bindFromTypeFullName.EscapeSpecialTypeSymbols()}_{bindToTypeFullName.EscapeSpecialTypeSymbols()}_{generator.GetVariableStableName()}";
            return createContextVariableName;
        }
    }
}
