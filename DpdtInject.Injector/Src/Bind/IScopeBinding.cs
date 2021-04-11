namespace DpdtInject.Injector.Src.Bind
{
    public interface IScopeBinding
    {
        IConfigureAndConditionalBinding WithTransientScope();

        IConfigureAndConditionalBinding WithSingletonScope();

        IConfigureAndConditionalBinding WithCustomScope();
    }

}
