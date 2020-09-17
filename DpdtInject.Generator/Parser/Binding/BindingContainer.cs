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

        public IReadOnlyCollection<ITypeSymbol> NotBindConstructorArgumentTypes
        {
            get;
        }

        public BindScopeEnum Scope
        {
            get;
        }

        public ArgumentSyntax? WhenArgumentClause
        {
            get;
        }

        public IReadOnlyCollection<string> FromTypeFullNames
        {
            get;
        }

        public bool IsConditional => WhenArgumentClause is not null;

        public string TargetTypeFullName => BindToType.GetFullName();

        public BindingContainer(
            IReadOnlyList<ITypeSymbol> bindFromTypes,
            ITypeSymbol bindToType,
            IReadOnlyList<DetectedConstructorArgument> constructorArguments,
            BindScopeEnum scope,
            ArgumentSyntax? whenArgumentClause
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
            NotBindConstructorArgumentTypes = new HashSet<ITypeSymbol>(constructorArguments.Where(ca => !ca.DefineInBindNode).Select(ca => ca.Type!), new TypeSymbolEqualityComparer());
            Scope = scope;
            WhenArgumentClause = whenArgumentClause;
            FromTypeFullNames = new HashSet<string>(BindFromTypes.ConvertAll(b => b.GetFullName()));
        }

        public string GetFromTypeFullNamesCombined(string separator = "_") => string.Join(separator, FromTypeFullNames);



    }
}
