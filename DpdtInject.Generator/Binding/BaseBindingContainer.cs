using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Generator.Parser.Binding
{
    public abstract class BaseBindingContainer : IBindingContainer
    {
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

        public ArgumentSyntax? ConstantSyntax
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
            IReadOnlyList<ITypeSymbol> bindFromTypes,
            ITypeSymbol bindToType,
            BindScopeEnum scope,
            ArgumentSyntax? whenArgumentClause,
            ArgumentSyntax? constantSyntax
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

            if(scope == BindScopeEnum.Constant && constantSyntax is null)
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Misconfiguration between scope nad constant syntax");
            }
            if(scope != BindScopeEnum.Constant && !(constantSyntax is null))
            {
                throw new DpdtException(DpdtExceptionTypeEnum.InternalError, $"Misconfiguration between scope nad constant syntax");
            }

            BindFromTypes = bindFromTypes;
            BindToType = bindToType;
            Scope = scope;
            WhenArgumentClause = whenArgumentClause;
            ConstantSyntax = constantSyntax;
            FromTypeFullNames = new HashSet<string>(BindFromTypes.ConvertAll(b => b.ToDisplayString()));
        }

        public string GetFromTypeFullNamesCombined(string separator = "_") => string.Join(separator, FromTypeFullNames);

        public string GetStableSuffix()
        {
            return this.GetHashCode().ToString();
        }

        public bool IsRegisteredFrom(ITypeSymbol bindFrom)
        {
            if (bindFrom is null)
            {
                throw new ArgumentNullException(nameof(bindFrom));
            }

            return BindFromTypes.Any(t => SymbolEqualityComparer.Default.Equals(t, bindFrom));
        }

        //public abstract string PrepareInstanceContainerCode(
        //    ClusterGeneratorTreeJoint clusterGeneratorJoint
        //    );

        //public string GetContainerStableClassName()
        //{
        //    switch (Scope)
        //    {
        //        case Injector.Module.Bind.BindScopeEnum.Transient:
        //            return $"{string.Join("_", GetFromTypeFullNamesCombined().EscapeSpecialTypeSymbols())}_{BindToType.GetFullName().EscapeSpecialTypeSymbols()}_{nameof(TransientInstanceContainer)}_{this.GetHashCode()}";
        //        case Injector.Module.Bind.BindScopeEnum.Singleton:
        //            return $"{string.Join("_", GetFromTypeFullNamesCombined().EscapeSpecialTypeSymbols())}_{BindToType.GetFullName().EscapeSpecialTypeSymbols()}_{nameof(SingletonInstanceContainer)}_{this.GetHashCode()}";
        //        case Injector.Module.Bind.BindScopeEnum.Constant:
        //            return $"{string.Join("_", GetFromTypeFullNamesCombined().EscapeSpecialTypeSymbols())}_{nameof(ConstantInstanceContainer)}_{this.GetHashCode()}";
        //        case Injector.Module.Bind.BindScopeEnum.Custom:
        //            return $"{string.Join("_", GetFromTypeFullNamesCombined().EscapeSpecialTypeSymbols())}_{BindToType.GetFullName().EscapeSpecialTypeSymbols()}_{nameof(CustomInstanceContainer)}_{this.GetHashCode()}";
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}

        //public void GetInstanceContainerBody(out string className, out string resource)
        //{
        //    switch (Scope)
        //    {
        //        case Injector.Module.Bind.BindScopeEnum.Constant:
        //            className = nameof(ConstantInstanceContainer);
        //            resource = Resources.ConstantInstanceContainer;
        //            break;
        //        case Injector.Module.Bind.BindScopeEnum.Transient:
        //            className = nameof(TransientInstanceContainer);
        //            resource = Resources.TransientInstanceContainer;
        //            break;
        //        case Injector.Module.Bind.BindScopeEnum.Singleton:
        //            className = nameof(SingletonInstanceContainer);
        //            resource = Resources.SingletonInstanceContainer;
        //            break;
        //        case Injector.Module.Bind.BindScopeEnum.Custom:
        //            className = nameof(CustomInstanceContainer);
        //            resource = Resources.CustomInstanceContainer;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(Scope.ToString());
        //    }
        //}
    }
}
