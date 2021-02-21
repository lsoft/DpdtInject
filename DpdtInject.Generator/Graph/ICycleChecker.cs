using DpdtInject.Generator.Binding;

namespace DpdtInject.Generator.Graph
{
    public interface ICycleChecker
    {
        void CheckForCycles(
            BindingExtenderBox box
            );
    }

}