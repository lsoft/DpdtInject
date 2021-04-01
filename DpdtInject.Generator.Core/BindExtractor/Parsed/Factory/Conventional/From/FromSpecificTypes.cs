using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.Producer;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.Conventional.From
{
    public class FromSpecificTypes : IFromTypesProvider
    {
        public IReadOnlyList<ITypeSymbol> SpecificTypes
        {
            get;
        }

        public FromSpecificTypes(
            IReadOnlyList<ITypeSymbol> specificTypes
            )
        {
            if (specificTypes is null)
            {
                throw new ArgumentNullException(nameof(specificTypes));
            }

            SpecificTypes = specificTypes;
        }

        public ImmutableArray<ITypeSymbol> GetBindFromTypes(INamedTypeSymbol type)
        {
            var result = new List<ITypeSymbol>();

            foreach (var specificType in SpecificTypes)
            {
                if (type.CanBeCastedTo(specificType))
                {
                    result.Add(specificType);
                }
            }

            return result.ToImmutableArray<ITypeSymbol>();
        }
    }
}
