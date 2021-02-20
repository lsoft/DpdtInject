using System;

namespace DpdtInject.Injector
{

    /// <summary>
    /// An attribute for marking the methods of DpdtCluster that contains a binding code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class DpdtBindingMethodAttribute : Attribute
    {

    }
}
