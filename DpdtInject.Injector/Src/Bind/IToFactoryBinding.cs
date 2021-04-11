namespace DpdtInject.Injector.Src.Bind
{
    public interface IToFactoryBinding
    {
        IScopeBinding WithPayload<TPayload>();
    }
}
