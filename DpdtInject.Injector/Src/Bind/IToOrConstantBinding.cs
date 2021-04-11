namespace DpdtInject.Injector.Src.Bind
{
    public interface IToOrConstantBinding
    {
        IScopeBinding To<T>();

        IConstantConditionalBinding WithConstScope<T>(T? constant);

        IToFactoryBinding ToIsolatedFactory<TFactory>();

        IToProxyBinding ToProxy<TProxy>();
    }
}
