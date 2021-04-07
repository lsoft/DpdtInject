namespace DpdtInject.Injector.Bind.Settings.Wrapper
{
    public abstract class WrappersSettings : ISetting
    {
        public abstract bool DoProduceWrappers
        {
            get;
        }

        public bool IsAllowedFor(BindScopeEnum scope) => true;
    }


}
