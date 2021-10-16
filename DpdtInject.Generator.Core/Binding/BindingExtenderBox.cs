using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Injector.Src.Excp;

namespace DpdtInject.Generator.Core.Binding
{
    /// <summary>
    /// Bindings grouped by its bind-from clauses.
    /// If there are many bind-from clauses in a single binding,
    /// the binding will be put into appropriate (many) groups.
    /// </summary>
    public class BindingExtenderBox
    {
        private readonly Dictionary<ITypeSymbol, BindingExtenderGroup> _groups;

        public IReadOnlyDictionary<ITypeSymbol, BindingExtenderGroup> Groups => _groups;


        public BindingExtenderBox(
            IReadOnlyList<BindingExtender> bindingExtenders
            )
        {
            if (bindingExtenders is null)
            {
                throw new ArgumentNullException(nameof(bindingExtenders));
            }

            _groups = new Dictionary<ITypeSymbol, BindingExtenderGroup>(
                TypeSymbolEqualityComparer.Entity
                );

            foreach (var extender in bindingExtenders)
            {
                foreach (var bindFromType in extender.BindingContainer.BindFromTypes)
                {
                    if (!_groups.ContainsKey(bindFromType))
                    {
                        _groups[bindFromType] = new BindingExtenderGroup(bindFromType);
                    }

                    _groups[bindFromType].Add(
                        extender
                        );
                }
            }
        }

        /// <summary>
        /// Provide a binding list by constructor argument.
        /// </summary>
        public bool TryGetChildren(
            DetectedMethodArgument constructorArgument,
            out IReadOnlyList<ExtenderAndTypePair> result
            )
        {
            if (constructorArgument is null)
            {
                throw new ArgumentNullException(nameof(constructorArgument));
            }

            var rresult = new List<ExtenderAndTypePair>();

            if (constructorArgument.Type is null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"constructorArgument.Type is null somehow"
                    );
            }

            if (!_groups.TryGetValue(constructorArgument.Type, out var @group))
            {
                var unwrappedType = constructorArgument.GetUnwrappedType();

                if (!_groups.TryGetValue(unwrappedType, out group))
                {
                    result = new List<ExtenderAndTypePair>();
                    return false;
                }
            }

            if (group is null)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.InternalError,
                    $"Something wrong with the {nameof(BindingExtenderBox)}"
                    );
            }

            foreach (var childBindingContainer in group.BindingExtenders)
            {
                rresult.Add(
                    new ExtenderAndTypePair(
                        childBindingContainer,
                        constructorArgument
                        )
                    );
            }

            result = rresult;
            return rresult.Count > 0;
        }

        /// <summary>
        /// Provide all children of the specific binding container.
        /// </summary>
        /// <param name="bindingContainer">A binding container whose children we want to retrieve.</param>
        /// <param name="tolerateMissingChildren">Should this method tolerate unknown (unresolvable) children?</param>
        /// <param name="result">Found children.</param>
        /// <returns>true if children has been found.</returns>
        public bool TryGetChildren(
            IBindingContainer bindingContainer,
            bool tolerateMissingChildren,
            out IReadOnlyList<ExtenderAndTypePair> result
            )
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            var rresult = new List<ExtenderAndTypePair>();

            foreach (var ca in bindingContainer.ConstructorArguments.Where(ca => !ca.DefineInBindNode))
            {
                if (ca.Type is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"constructorArgument.Type is null somehow"
                        );
                }

                if (Groups.TryGetValue(ca.Type, out var group))
                {
                    foreach (var childBindingContainer in group.BindingExtenders)
                    {
                        rresult.Add(
                            new ExtenderAndTypePair(
                                childBindingContainer,
                                ca
                                )
                            );
                    }
                }
                else
                {
                    //for at least one constructor argument there is no bindings
                    //(probably it's a binding misconfiguration or these bindings are in parent cluster)
                    if(!tolerateMissingChildren)
                    {
                        result = new List<ExtenderAndTypePair>();
                        return false;
                    }
                }
            }

            result = rresult;
            return true;
        }
    }
}
