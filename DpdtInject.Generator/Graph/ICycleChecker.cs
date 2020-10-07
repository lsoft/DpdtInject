using DpdtInject.Generator.Binding;
using DpdtInject.Injector;
using DpdtInject.Injector.Compilation;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Graph
{
    public interface ICycleChecker
    {
        void CheckForCycles(
            BindingExtenderBox box
            );
    }

}