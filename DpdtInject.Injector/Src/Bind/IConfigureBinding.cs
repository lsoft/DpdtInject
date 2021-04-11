namespace DpdtInject.Injector.Src.Bind
{
    public interface IConfigureBinding
    {
        IConfigureBinding Configure(
            ConstructorArgument argument
            );
    }
}
