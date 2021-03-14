using DpdtInject.Generator.Binding;
using DpdtInject.Injector.Bind.Settings;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DpdtInject.Generator.Graph
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
            BindingExtenderBox box
            )
        {
            if (box is null)
            {
                throw new ArgumentNullException(nameof(box));
            }

            var cycles = new HashSet<CycleFoundException>(
                new OrderIndependentCycleFoundEqualityComparer()
                );

            foreach (var pair in box.Groups.Shuffle())
            {
                var group = pair.Value;
                foreach (var bindingExtender in group.BindingExtenders.Shuffle())
                {
                    var doCircularCheck = true;
                    if (bindingExtender.BindingContainer.TryGetSettingInScope<CircularSetting>(out var setting))
                    {
                        doCircularCheck = setting.DoCircularCheck;
                    }

                    if (!doCircularCheck)
                    {
                        continue;
                    }

                    try
                    {
                        var used = new Subgraph();

                        CheckForCyclesInternal(
                            ref used,
                            box,
                            bindingExtender.BindingContainer
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
            BindingExtenderBox box,
            IBindingContainer bindingContainer
            )
        {
            if (used is null)
            {
                throw new ArgumentNullException(nameof(used));
            }

            if (box is null)
            {
                throw new ArgumentNullException(nameof(box));
            }

            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            used.AppendOrFailIfExists(
                bindingContainer,
                !bindingContainer.IsConditional
                );

            if(box.TryGetChildren(bindingContainer, true, out var pairs))
            {
                foreach(var pair in pairs)
                {
                    var used2 = used.Clone();

                    CheckForCyclesInternal(
                        ref used2,
                        box,
                        pair.BindingExtender.BindingContainer
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

                foreach (var bindingContainer in obj.CycleList.Skip(1))
                {
                    result ^= bindingContainer.BindToType.ToDisplayString().GetHashCode();
                }

                return result;
            }
        }

    }
}
