using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace DpdtInject.Generator.Graph
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