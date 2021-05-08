namespace DpdtInject.Injector.Src.Bind.Settings.Wrapper
{
    public abstract class WrappersSettings : ISetting
    {
        public const string ScopeConstant = nameof(WrappersSettings);
        public string Scope => ScopeConstant;

        public abstract bool DoProduceWrappers
        {
            get;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => true;
    }


}
