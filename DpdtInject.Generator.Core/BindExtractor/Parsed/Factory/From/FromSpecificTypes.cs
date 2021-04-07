using DpdtInject.Generator.Core.Helpers;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DpdtInject.Generator.Core.BindExtractor.Parsed.Factory.From
{
    public class FromSpecificTypes : IFromTypesProvider
    {
        private readonly bool _strictMode;

        public IReadOnlyList<ITypeSymbol> SpecificTypes
        {
            get;
        }

        public FromSpecificTypes(
            IReadOnlyList<ITypeSymbol> specificTypes,
            bool strictMode
            )
        {
            if (specificTypes is null)
            {
                throw new ArgumentNullException(nameof(specificTypes));
            }

            SpecificTypes = specificTypes;
            _strictMode = strictMode;
        }

        public ImmutableArray<ITypeSymbol> GetBindFromTypes(ITypeSymbol type)
        {
            var result = new List<ITypeSymbol>();

            foreach (var specificType in SpecificTypes)
            {
                if (_strictMode || type.CanBeCastedTo(specificType))
                {
                    result.Add(specificType);
                }
            }

            return result.ToImmutableArray<ITypeSymbol>();
        }
    }
}
