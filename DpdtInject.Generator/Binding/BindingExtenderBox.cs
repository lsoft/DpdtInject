using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Binding
{
    public class BindingExtenderBox
    {
        private Dictionary<ITypeSymbol, BindingContainerGroup> _groups;

        public IReadOnlyDictionary<ITypeSymbol, BindingContainerGroup> Groups => _groups;


        public BindingExtenderBox(
            IReadOnlyList<BindingContainerExtender> bindingExtenders
            )
        {
            if (bindingExtenders is null)
            {
                throw new ArgumentNullException(nameof(bindingExtenders));
            }

            _groups = new Dictionary<ITypeSymbol, BindingContainerGroup>(
                new TypeSymbolEqualityComparer()
                );

            foreach (var extender in bindingExtenders)
            {
                foreach (var bindFromType in extender.BindingContainer.BindFromTypes)
                {
                    if (!_groups.ContainsKey(bindFromType))
                    {
                        _groups[bindFromType] = new BindingContainerGroup(bindFromType);
                    }

                    _groups[bindFromType].Add(
                        extender
                        );
                }
            }
        }

        public bool TryGetChildren(
            IBindingContainer bindingContainer,
            DetectedConstructorArgument constructorArgument,
            out IReadOnlyList<ExtenderAndTypePair> result
            )
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

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

            BindingContainerGroup? group;
            if (!Groups.TryGetValue(constructorArgument.Type, out group))
            {
                var unwrappedType = constructorArgument.GetUnwrappedType();

                if (!Groups.TryGetValue(unwrappedType, out group))
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

    public class ExtenderAndTypePair
    {
        public BindingContainerExtender BindingExtender
        {
            get;
        }
        
        public DetectedConstructorArgument ConstructorArgument
        {
            get;
        }

        public ExtenderAndTypePair(
            BindingContainerExtender bindingExtender,
            DetectedConstructorArgument constructorArgument
            )
        {
            if (bindingExtender is null)
            {
                throw new ArgumentNullException(nameof(bindingExtender));
            }

            if (constructorArgument is null)
            {
                throw new ArgumentNullException(nameof(constructorArgument));
            }

            BindingExtender = bindingExtender;
            ConstructorArgument = constructorArgument;
        }

    }
}
