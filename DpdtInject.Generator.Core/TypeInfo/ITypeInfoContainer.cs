using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Core.TypeInfo
{
    public interface ITypeInfoContainer : ITypeInfoProvider
    {
        bool IsFactoryBuildFor(ITypeSymbol factoryTargetType);
        void AddFactoryBuildFor(ITypeSymbol factoryTargetType);

        bool IsProxyBuildFor(ITypeSymbol proxyTargetType);
        void AddProxyBuildFor(ITypeSymbol proxyTargetType);

        void AddSources(
            ModificationDescription[] modificationDescriptions
            );

        void AddAdditionalFile(
            string xmlBody
            );
    }
}