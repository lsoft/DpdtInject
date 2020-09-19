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
            string fromType,
            string toType
            )
        {
            if (fromType is null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }

            return $"new {typeof(ResolutionFrame).FullName}( typeof({fromType}), typeof({toType}) )";
        }

        public static string GetNewFrameClause(
            string fromType,
            string toType,
            string constructorArgumentName
            )
        {
            if (fromType is null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }

            if (toType is null)
            {
                throw new ArgumentNullException(nameof(toType));
            }

            if (constructorArgumentName is null)
            {
                throw new ArgumentNullException(nameof(constructorArgumentName));
            }

            return $"new {typeof(ResolutionFrame).FullName}(typeof({fromType}), typeof({toType}), \"{constructorArgumentName}\")";
        }
    }
}
