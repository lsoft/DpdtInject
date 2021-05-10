using DpdtInject.Generator.Core.Binding.Settings.Constructor;
using DpdtInject.Injector.Src.Bind.Settings;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DpdtInject.Generator.Core.Helpers
{
    public static class SettingHelper
    {
        public static bool IsSetup<T>(
            this IReadOnlyList<IDefinedSetting> settings
            )
             where T : class, IDefinedSetting
        {
            return settings.Any(s => s.GetType().FullName == typeof(T).FullName!);
        }

        public static bool TryGetSettingInScope<TScope>(
            this IReadOnlyList<IDefinedSetting> settings,
            string scope,
            [NotNullWhen(true)] out TScope? setting
            )
             where TScope : class, IDefinedSetting
        {
            setting = settings.FirstOrDefault(s => s.Scope == scope) as TScope;

            return setting != null;
        }

    }

}
