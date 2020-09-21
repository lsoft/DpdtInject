using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer;
using DpdtInject.Generator.Properties;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Module.RContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Parser.Binding
{
    [DebuggerDisplay("{BindFromTypes[0].Name} -> {TargetRepresentation}")]
    public class ConstantBindingContainer : IBindingContainer
    {
        private readonly ITypeSymbol _constTypeSymbol;
        private readonly ArgumentSyntax _constantSyntax;

        public string Name
        {
            get;
        }
        
        public IReadOnlyList<ITypeSymbol> BindFromTypes
        {
            get;
        }

        public ITypeSymbol BindToType => _constTypeSymbol;

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

        public string TargetRepresentation
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return $"constant[{_constTypeSymbol.GetFullName()}]";
                }

                return $"constant[{Name} : {_constTypeSymbol.GetFullName()}]";
            }
        }

        public bool AtLeastOneChildIsConditional
        {
            get;
            set;
        }


        public ConstantBindingContainer(
            string name,
            IReadOnlyList<ITypeSymbol> bindFromTypes,
            ITypeSymbol constTypeSymbol,
            ArgumentSyntax constantSyntax,
            BindScopeEnum scope,
            ArgumentSyntax? whenArgumentClause
            )
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (bindFromTypes is null)
            {
                throw new ArgumentNullException(nameof(bindFromTypes));
            }

            if (constTypeSymbol is null)
            {
                throw new ArgumentNullException(nameof(constTypeSymbol));
            }

            if (constantSyntax is null)
            {
                throw new ArgumentNullException(nameof(constantSyntax));
            }
            Name = name;
            BindFromTypes = bindFromTypes;
            _constTypeSymbol = constTypeSymbol;
            _constantSyntax = constantSyntax;
            ConstructorArguments = new List<DetectedConstructorArgument>();
            NotBindConstructorArgumentTypes = new HashSet<ITypeSymbol>();
            Scope = scope;
            WhenArgumentClause = whenArgumentClause;
            FromTypeFullNames = new HashSet<string>(BindFromTypes.ConvertAll(b => b.GetFullName()));
        }

        public string GetFromTypeFullNamesCombined(string separator = "_") => string.Join(separator, FromTypeFullNames);

        public string PrepareInstanceContainerCode(
            string instanceContainerCode,
            GeneratorsContainer container
            )
        {
            if (instanceContainerCode is null)
            {
                throw new ArgumentNullException(nameof(instanceContainerCode));
            }

            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var result = instanceContainerCode
                .CheckAndReplace(nameof(FakeTarget), _constTypeSymbol.GetFullName())
                .CheckAndReplace("//GENERATOR: init constant", $"{nameof(ConstantInstanceContainer.Instance)} = {_constantSyntax.GetText()};")
                //.CheckAndReplace("//GENERATOR: argument methods", string.Join(Environment.NewLine, ConstructorArguments.Where(ca => !ca.DefineInBindNode).Select(ca => ca.GetRetrieveConstructorArgumentClause(container, this))))
                //.CheckAndReplace("//GENERATOR: apply arguments", string.Join(",", ConstructorArguments.Select(ca => ca.GetApplyConstructorClause(container))))
                .CheckAndReplace("//GENERATOR: predicate", (WhenArgumentClause?.ToString() ?? "rc => true"))
                ;

            return result;
        }
    }
}
