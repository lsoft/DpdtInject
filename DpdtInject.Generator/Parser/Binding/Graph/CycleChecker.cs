using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Reporter;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DpdtInject.Generator.Parser.Binding.Graph
{
    public class CycleChecker
    {
        private readonly BindingContainerGroups _groups;

        public CycleChecker(
            BindingContainerGroups groups
            )
        {
            _groups = groups;
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

            foreach (var bindFromType in _groups.BindGroups.Keys.Shuffle())
            {
                try
                {
                    var used = new Subgraph();

                    CheckForCyclesInternal(
                        ref used,
                        bindFromType
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
            foreach (var bindingProcessor in _groups.BindGroups[requestedType])
            {
                var used2 = used.Clone();

                used2.AppendOrFailIfExists(
                    requestedType,
                    !bindingProcessor.IsConditional
                    );

                foreach (var constructorArgument in bindingProcessor.ConstructorArguments.Where(ca => !ca.DefineInBindNode).Shuffle())
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
