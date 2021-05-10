using DpdtInject.Generator.Core.Binding.Settings.Constructor;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Generator.Core.Binding.Settings.Wrapper
{
    public class WrapperSetting : IDefinedSetting
    {
        public const string ScopeConstant = nameof(WrapperSetting);
        public string Scope => ScopeConstant;

        public bool DoProduceWrappers
        {
            get;
        }

        public WrapperSetting(bool doProduceWrappers)
        {
            DoProduceWrappers = doProduceWrappers;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => true;
    }


}
