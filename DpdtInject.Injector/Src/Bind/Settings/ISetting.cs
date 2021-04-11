namespace DpdtInject.Injector.Src.Bind.Settings
{
    public interface ISetting
    {
        bool IsAllowedFor(BindScopeEnum scope);
    }


}
