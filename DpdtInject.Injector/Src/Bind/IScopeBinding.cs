namespace DpdtInject.Injector.Bind
{
    public interface IScopeBinding
    {
        IConfigureAndConditionalBinding WithTransientScope();

        IConfigureAndConditionalBinding WithSingletonScope();

        IConfigureAndConditionalBinding WithCustomScope();
    }

}