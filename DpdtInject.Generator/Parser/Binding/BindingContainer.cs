using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Properties;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Parser.Binding
{
    public class BindingContainer

    {
        public IReadOnlyList<ITypeSymbol> BindFromTypes
        {
            get;
        }

        public ITypeSymbol BindToType
        {
            get;
        }

        public IReadOnlyList<DetectedConstructorArgument> ConstructorArguments
        {
            get;
        }

        public BindScopeEnum Scope
        {
            get;
        }

        public InstanceContainerGenerator InstanceContainerGenerator
        {
            get;
        }

        public IReadOnlyCollection<string> FromTypeFullNames
        {
            get;
        }

        public IReadOnlyCollection<string> FromTypeNames
        {
            get;
        }

        public bool IsConditional
        {
            get;
            private set;
        } = false;

        public string TargetTypeName => BindToType.Name;

        public string TargetTypeFullName => BindToType.GetFullName();

        public BindingContainer(
            IReadOnlyList<ITypeSymbol> bindFromTypes,
            ITypeSymbol bindToType,
            IReadOnlyList<DetectedConstructorArgument> constructorArguments,
            BindScopeEnum scope
            )
        {
            if (bindFromTypes is null)
            {
                throw new ArgumentNullException(nameof(bindFromTypes));
            }

            if (bindToType is null)
            {
                throw new ArgumentNullException(nameof(bindToType));
            }

            if (constructorArguments is null)
            {
                throw new ArgumentNullException(nameof(constructorArguments));
            }

            BindFromTypes = bindFromTypes;
            BindToType = bindToType;
            ConstructorArguments = constructorArguments;
            Scope = scope;
            FromTypeFullNames = new HashSet<string>(BindFromTypes.ConvertAll(b => b.GetFullName()));
            FromTypeNames = new HashSet<string>(BindFromTypes.ConvertAll(b => b.Name));

            InstanceContainerGenerator = new InstanceContainerGenerator(
                FromTypeNames,
                TargetTypeName,
                TargetTypeFullName,
                ConstructorArguments
                );
        }
    }
}
