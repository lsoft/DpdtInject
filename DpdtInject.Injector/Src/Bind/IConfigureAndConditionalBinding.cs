using DpdtInject.Injector.Src.Bind.Settings;

namespace DpdtInject.Injector.Src.Bind
{
    public interface IConfigureAndConditionalBinding :
        IConditionalBinding, IConfigureBinding
    {

        IConfigureAndConditionalBinding Setup<T>()
            where T : ISetting;
    }
}
