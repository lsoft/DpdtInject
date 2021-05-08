using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DpdtInject.Injector.Src.Bind.Settings
{
    public static class SettingHelper
    {
        public static bool IsSetup<T>(
            this IReadOnlyList<ISetting> settings
            )
             where T : class, ISetting
        {
            return settings.Any(s => s.GetType().FullName == typeof(T).FullName!);
        }

        public static bool TryGetSettingInScope<TScope>(
            this IReadOnlyList<ISetting> settings,
            string scope,
            [NotNullWhen(true)] out TScope? setting
            )
             where TScope : class, ISetting
        {
            setting = settings.FirstOrDefault(s => s.Scope == scope) as TScope;

            return setting != null;
        }

    }

}
