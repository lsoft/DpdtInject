using DpdtInject.Generator.Core.Binding;

namespace DpdtInject.Generator.Core.Graph
{
    public interface ICycleChecker
    {
        void CheckForCycles(
            BindingExtenderBox box
            );
    }

}