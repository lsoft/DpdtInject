using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Reporter;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Binding.Graph
{
    public class CycleChecker
    {
        private readonly GeneratorCluster _cluster;

        public CycleChecker(
            GeneratorCluster cluster
            )
        {
            _cluster = cluster;
        }

        public void CheckForCycles(
            IDiagnosticReporter reporter
            )
        {
            if (reporter is null)
            {
                throw new ArgumentNullException(nameof(reporter));
            }

            var cycles = new HashSet<CycleFoundException>(
                new OrderIndependentCycleFoundEqualityComparer()
                );

            foreach (var (wrapperType, wrapperSymbol) in _cluster.GetRegisteredKeys(true).Shuffle())
            {
                try
                {
                    var used = new Subgraph();

                    CheckForCyclesInternal(
                        ref used,
                        wrapperSymbol
                        );
                }
                catch (CycleFoundException cfe)
                {
                    if (!cycles.Contains(cfe))
                    {
                        cycles.Add(cfe);
                    }
                }
            }

            foreach (var cycle in cycles)
            {
                if (cycle.StrictConculsion)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.CircularDependency,
                        $"A circular dependency was found: [{cycle.GetStringRepresentation()}]"
                        );
                }
                else
                {
                    reporter.ReportWarning(
                        $"Perhaps a circular dependency was found",
                        $"Perhaps a circular dependency was found, please take a look: [{cycle.GetStringRepresentation()}]"
                        );
                }
            }
        }

        private void CheckForCyclesInternal(
            ref Subgraph used,
            ITypeSymbol requestedType
            )
        {
            if (!_cluster.TryGetRegisteredGeneratorGroups(requestedType, true, out var groups))
            {
                return;
            }

            foreach (var group in groups)
            {
                foreach (var generator in group.Generators)
                {
                    var used2 = used.Clone();

                    used2.AppendOrFailIfExists(
                        requestedType,
                        !generator.BindingContainer.IsConditional
                        );

                    foreach (var constructorArgument in generator.BindingContainer.ConstructorArguments.Where(ca => !ca.DefineInBindNode).Shuffle())
                    {
                        if (constructorArgument.Type is null)
                        {
                            throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"constructorArgument.Type is null somehow");
                        }

                        CheckForCyclesInternal(
                            ref used2,
                            constructorArgument.Type
                            );
                    }
                }
            }
        }

        private class OrderIndependentCycleFoundEqualityComparer : IEqualityComparer<CycleFoundException>
        {
            public bool Equals([AllowNull] CycleFoundException x, [AllowNull] CycleFoundException y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null)
                {
                    return false;
                }
                if (y is null)
                {
                    return false;
                }

                return GetHashCode(x) == GetHashCode(y);
            }

            public int GetHashCode([DisallowNull] CycleFoundException obj)
            {
                var result = obj.StrictConculsion ? int.MaxValue : 0;

                foreach (var type in obj.CycleList.Skip(1))
                {
                    result ^= type.GetFullName().GetHashCode();
                }

                return result;
            }
        }

    }
}
