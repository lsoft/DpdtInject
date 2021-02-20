using System;

namespace DpdtInject.Injector
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class DpdtBindingMethodAttribute : Attribute
    {

    }
}
