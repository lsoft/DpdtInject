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
    public class InstanceContainerGeneratorGroup
    {
        private readonly List<InstanceContainerGenerator> _generators;

        public ITypeSymbol BindFrom
        {
            get;
        }

        public IReadOnlyList<InstanceContainerGenerator> Generators => _generators;

        public InstanceContainerGeneratorGroup(
            ITypeSymbol bindFrom,
            List<InstanceContainerGenerator> generators
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
