using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using System;

namespace DpdtInject.Generator.Producer.Blocks.Binding.Graph
{
    public interface ICycleChecker
    {
        void CheckForCycles(InstanceContainerGeneratorTreeJoint joint);
    }

}