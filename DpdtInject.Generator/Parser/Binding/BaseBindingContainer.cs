using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer;
using DpdtInject.Generator.Producer.Blocks.Cluster;
using DpdtInject.Generator.Properties;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Parser.Binding
{
    public abstract class BaseBindingContainer : IBindingContainer
    {
        public ITypeSymbol DeclaredClusterType
        {
            get;
        }

        public bool IsRootCluster => DeclaredClusterType.BaseType!.GetFullName() == "System.Object";

        public IReadOnlyList<ITypeSymbol> BindFromTypes
        {
            get;
        }

        public ITypeSymbol BindToType
        {
            get;
        }


        public abstract IReadOnlyList<DetectedConstructorArgument> ConstructorArguments
        {
            get;
        }

        public abstract IReadOnlyCollection<ITypeSymbol> NotBindConstructorArgumentTypes
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

        public abstract string TargetRepresentation
        {
            get;
        }
        

        public BaseBindingContainer(
            ITypeSymbol declaredClusterType,
            IReadOnlyList<ITypeSymbol> bindFromTypes,
            ITypeSymbol bindToType,
            BindScopeEnum scope,
            ArgumentSyntax? whenArgumentClause
            )
        {
            if (declaredClusterType is null)
            {
                throw new ArgumentNullException(nameof(declaredClusterType));
            }

            if (bindFromTypes is null)
            {
                throw new ArgumentNullException(nameof(bindFromTypes));
            }

            if (bindToType is null)
            {
                throw new ArgumentNullException(nameof(bindToType));
            }

            DeclaredClusterType = declaredClusterType;
            BindFromTypes = bindFromTypes;
            BindToType = bindToType;
            Scope = scope;
            WhenArgumentClause = whenArgumentClause;
            FromTypeFullNames = new HashSet<string>(BindFromTypes.ConvertAll(b => b.GetFullName()));
        }

        public string GetFromTypeFullNamesCombined(string separator = "_") => string.Join(separator, FromTypeFullNames);

        public abstract string PrepareInstanceContainerCode(
            ClusterGeneratorTreeJoint clusterGeneratorJoint
            );

        public string GetContainerStableClassName()
        {
            switch (Scope)
            {
                case Injector.Module.Bind.BindScopeEnum.Transient:
                    return $"{string.Join("_", GetFromTypeFullNamesCombined().EscapeSpecialTypeSymbols())}_{BindToType.GetFullName().EscapeSpecialTypeSymbols()}_{nameof(TransientInstanceContainer)}_{this.GetHashCode()}";
                    break;
                case Injector.Module.Bind.BindScopeEnum.Singleton:
                    return $"{string.Join("_", GetFromTypeFullNamesCombined().EscapeSpecialTypeSymbols())}_{BindToType.GetFullName().EscapeSpecialTypeSymbols()}_{nameof(SingletonInstanceContainer)}_{this.GetHashCode()}";
                    break;
                case Injector.Module.Bind.BindScopeEnum.Constant:
                    return $"{string.Join("_", GetFromTypeFullNamesCombined().EscapeSpecialTypeSymbols())}_{nameof(ConstantInstanceContainer)}_{this.GetHashCode()}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void GetInstanceContainerBody(out string className, out string resource)
        {
            switch (Scope)
            {
                case Injector.Module.Bind.BindScopeEnum.Constant:
                    className = nameof(ConstantInstanceContainer);
                    resource = Resources.ConstantInstanceContainer;
                    break;
                case Injector.Module.Bind.BindScopeEnum.Transient:
                    className = nameof(TransientInstanceContainer);
                    resource = Resources.TransientInstanceContainer;
                    break;
                case Injector.Module.Bind.BindScopeEnum.Singleton:
                    className = nameof(SingletonInstanceContainer);
                    resource = Resources.SingletonInstanceContainer;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(Scope.ToString());
            }
        }
    }
}
