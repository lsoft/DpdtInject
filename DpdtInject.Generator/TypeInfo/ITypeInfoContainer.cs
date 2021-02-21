namespace DpdtInject.Generator.TypeInfo
{
    public interface ITypeInfoContainer : ITypeInfoProvider
    {
        void AddSources(
            ModificationDescription[] modificationDescriptions
            );
    }
}