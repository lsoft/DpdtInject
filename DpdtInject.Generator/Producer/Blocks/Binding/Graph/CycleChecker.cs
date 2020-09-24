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
    public class CycleChecker : ICycleChecker
    {
        private readonly IDiagnosticReporter _reporter;

        public CycleChecker(
            IDiagnosticReporter reporter
            )
        {
            if (reporter is null)
            {
                throw new ArgumentNullException(nameof(reporter));
            }

            _reporter = reporter;
        }

        public void CheckForCycles(
            InstanceContainerGeneratorTreeJoint joint
            )
        {
            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            var cycles = new HashSet<CycleFoundException>(
                new OrderIndependentCycleFoundEqualityComparer()
                );

            foreach (var point2 in joint.GeneratePoints2().Shuffle())
            {
                try
                {
                    var used = new Subgraph();

                    CheckForCyclesInternal(
                        ref used,
                        point2
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
                    _reporter.ReportWarning(
                        $"Perhaps a circular dependency was found",
                        $"Perhaps a circular dependency was found, please take a look: [{cycle.GetStringRepresentation()}]"
                        );
                }
            }
        }

        private void CheckForCyclesInternal(
            ref Subgraph used,
            Point2 point2
            )
        {
            if (used is null)
            {
                throw new ArgumentNullException(nameof(used));
            }

            if (point2 is null)
            {
                throw new ArgumentNullException(nameof(point2));
            }

            used.AppendOrFailIfExists(
                point2.Generator,
                !point2.Generator.BindingContainer.IsConditional
                );

            foreach (var ca in point2.Generator.BindingContainer.ConstructorArguments.Where(ca => !ca.DefineInBindNode).Shuffle())
            {
                if (ca.Type is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"constructorArgument.Type is null somehow"
                        );
                }

                var childPoint3 = new Point3(
                    point2.Joint,
                    point2.Generator,
                    ca.Type
                    );

                if (childPoint3.TryFindChildren(out var childrenPoint2))
                {
                    foreach (var childPoint2 in childrenPoint2)
                    {
                        var used2 = used.Clone();

                        CheckForCyclesInternal(
                            ref used2,
                            childPoint2
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

                foreach (var generator in obj.CycleList.Skip(1))
                {
                    result ^= generator.BindingContainer.BindToType.GetFullName().GetHashCode();
                }

                return result;
            }
        }

    }
}
