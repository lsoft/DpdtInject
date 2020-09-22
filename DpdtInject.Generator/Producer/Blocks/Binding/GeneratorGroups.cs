using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Binding
{
    public class GeneratorGroup
    {
        private readonly List<Generator> _generators;

        public ITypeSymbol BindFrom
        {
            get;
        }

        public IReadOnlyList<Generator> Generators => _generators;

        public GeneratorGroup(
            ITypeSymbol bindFrom,
            List<Generator> generators
            )
        {
            if (bindFrom is null)
            {
                throw new ArgumentNullException(nameof(bindFrom));
            }

            if (generators is null)
            {
                throw new ArgumentNullException(nameof(generators));
            }

            BindFrom = bindFrom;
            _generators = generators;
        }
    }

}
