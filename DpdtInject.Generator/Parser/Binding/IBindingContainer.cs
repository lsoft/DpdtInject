using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace DpdtInject.Generator.Parser.Binding
{
    public interface IBindingContainer
    {
        string Name
        {
            get;
        }

        IReadOnlyList<ITypeSymbol> BindFromTypes
        {
            get;
        }

        IReadOnlyCollection<string> FromTypeFullNames
        {
            get;
        }

        ITypeSymbol BindToType
        {
            get;
        }

        string TargetRepresentation
        {
            get;
        }

        IReadOnlyList<DetectedConstructorArgument> ConstructorArguments
        {
            get;
        }
        IReadOnlyCollection<ITypeSymbol> NotBindConstructorArgumentTypes
        {
            get;
        }

        BindScopeEnum Scope
        {
            get;
        }


        bool IsConditional
        {
            get;
        }

        ArgumentSyntax? WhenArgumentClause
        {
            get;
        }

        //bool AtLeastOneChildIsConditional
        //{
        //    get;
        //    set;
        //}

        string GetFromTypeFullNamesCombined(string separator = "_");

        string PrepareInstanceContainerCode(
            string instanceContainerCode,
            GeneratorsContainer container
            );
    }
}