using DpdtInject.Injector.Bind.Settings;

namespace DpdtInject.Injector.Bind
{
    public interface IConfigureAndConditionalBinding :
        IConditionalBinding, IConfigureBinding
    {

        IConfigureAndConditionalBinding Setup<T>()
            where T : ISetting;
    }
}