namespace DpdtInject.Generator.Core.TypeInfo
{
    public interface ITypeInfoContainer : ITypeInfoProvider
    {
        void AddSources(
            ModificationDescription[] modificationDescriptions
            );
    }
}