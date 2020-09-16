using DpdtInject.Injector.Module.RContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Producer.RContext
{
    public static class ResolutionFrameGenerator
    {
        public static string GetNewFrameClause(
            string constructorType
            )
        {
            if (constructorType is null)
            {
                throw new ArgumentNullException(nameof(constructorType));
            }

            return $"new {typeof(ResolutionFrame).FullName}(typeof({constructorType}))";
        }

        public static string GetNewFrameClause(
            string type,
            string constructorArgumentName
            )
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (constructorArgumentName is null)
            {
                throw new ArgumentNullException(nameof(constructorArgumentName));
            }

            return $"new {typeof(ResolutionFrame).FullName}(typeof({type}), \"{constructorArgumentName}\")";
        }
    }
}
