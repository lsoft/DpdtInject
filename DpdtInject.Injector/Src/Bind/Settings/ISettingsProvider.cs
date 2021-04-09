using System.Diagnostics.CodeAnalysis;

namespace DpdtInject.Injector.Bind.Settings
{
    public interface ISettingsProvider
    {
        bool IsSetup<T>()
             where T : class, ISetting;

        bool TryGetSettingInScope<TScope>([NotNullWhen(true)]  out TScope? setting)
             where TScope : class, ISetting;

    }


}
