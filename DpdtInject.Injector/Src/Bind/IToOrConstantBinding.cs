namespace DpdtInject.Injector.Bind
{
    public interface IToOrConstantBinding
    {
        IScopeBinding To<T>();

        IConstantConditionalBinding WithConstScope<T>(T? constant);

        IToFactoryBinding ToIsolatedFactory<TFactory>();

        IToProxyBinding ToProxy<TProxy>();
    }
}
