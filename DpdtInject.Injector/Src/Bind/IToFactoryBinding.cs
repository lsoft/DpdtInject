namespace DpdtInject.Injector.Bind
{
    public interface IToFactoryBinding
    {
        IScopeBinding WithPayload<TPayload>();
    }
}