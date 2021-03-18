using DpdtInject.Generator.Core.Binding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Core.Graph
{
    internal class CycleFoundException : System.Exception
    {
        public IReadOnlyList<IBindingContainer> CycleList
        {
            get;
        }
        public bool StrictConculsion
        {
            get;
        }

        public CycleFoundException(
            List<IBindingContainer> cycleList,
            bool strictConculsion
            )
        {
            if (cycleList is null)
            {
                throw new ArgumentNullException(nameof(cycleList));
            }

            CycleList = cycleList;
            StrictConculsion = strictConculsion;
        }

        internal object GetStringRepresentation()
        {
            return
                string.Join(" -> ", CycleList.Select(r => "[" + r.TargetRepresentation + "]"));
        }
    }
}