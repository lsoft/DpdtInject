using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Generator.Core.Binding.Settings.Constructor
{
    public interface IDefinedSetting
    {
        string Scope
        {
            get;
        }

        bool IsAllowedFor(BindScopeEnum scope);
    }
}
