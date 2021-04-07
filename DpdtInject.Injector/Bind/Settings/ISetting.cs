namespace DpdtInject.Injector.Bind.Settings
{
    public interface ISetting
    {
        bool IsAllowedFor(BindScopeEnum scope);
    }


}
