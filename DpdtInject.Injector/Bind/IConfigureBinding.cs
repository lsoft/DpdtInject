namespace DpdtInject.Injector.Bind
{
    public interface IConfigureBinding
    {
        IConfigureBinding Configure(
            ConstructorArgument argument
            );
    }
}