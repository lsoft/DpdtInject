using System.Collections.Generic;

namespace DpdtInject.Injector.Module.Bind
{
    public interface IBindConfiguration
    {
        BindNode BindNode
        {
            get;
        }

        //DpdtIdempotentStatusEnum IdempotentStatus
        //{
        //    get;
        //}

        object? GetConstant();

        IReadOnlyDictionary<string, ConstructorArgument> GetConstructorArguments();

    }
}
