using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace DpdtInject.Generator
{
    internal class CycleFoundException : Exception
    {
        public IReadOnlyList<ITypeSymbol> CycleList
        {
            get;
        }
        public bool StrictConculsion
        {
            get;
        }

        public CycleFoundException(
            List<ITypeSymbol> cycleList,
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
                string.Join(" -> ", CycleList.Select(r => r.GetFullName()));
        }
    }
}