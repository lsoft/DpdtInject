namespace DpdtInject.Injector.Bind
{
    public interface IToOrConstantBinding
    {
        IScopeBinding To<T>();

        IToFactoryBinding ToIsolatedFactory<TFactory>();

        IConstantConditionalBinding WithConstScope<T>(T? constant);
    }
}
