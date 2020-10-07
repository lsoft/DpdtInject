using DpdtInject.Generator.Binding;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace DpdtInject.Generator.Parser.Binding
{
    public interface IBindingContainer
    {
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

        ArgumentSyntax? ConstantSyntax
        {
            get;
        }

        string GetFromTypeFullNamesCombined(string separator = "_");

        //string PrepareInstanceContainerCode(
        //    ClusterGeneratorTreeJoint clusterGeneratorJoint
        //    );

        //string GetContainerStableClassName();

        //void GetInstanceContainerBody(out string className, out string resource);

        string GetStableSuffix();

        bool IsRegisteredFrom(ITypeSymbol bindFrom);
    }
}