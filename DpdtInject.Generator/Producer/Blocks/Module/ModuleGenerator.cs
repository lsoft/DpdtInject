using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Cluster;
using DpdtInject.Injector;
using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Module.CustomScope;
using DpdtInject.Injector.Module.RContext;
using DpdtInject.Injector.Reinvented;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Module
{
    internal class ModuleGenerator : IModuleGenerator
    {
        private readonly Compilation _compilation;

        public INamedTypeSymbol ModuleSymbol
        {
            get;
        }

        public string ModuleTypeNamespace => ModuleSymbol.ContainingNamespace.ToDisplayString();

        public string ModuleTypeName => ModuleSymbol.Name;

        public ModuleGenerator(
            Compilation compilation,
            INamedTypeSymbol moduleSymbol
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (moduleSymbol is null)
            {
                throw new ArgumentNullException(nameof(moduleSymbol));
            }
            _compilation = compilation;
            ModuleSymbol = moduleSymbol;
        }

        public string GenerateModuleBody(
            IInstanceContainerGeneratorsContainer container
            )
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var clusterGeneratorTree = new ClusterGeneratorTree(
                container.GeneratorTree.Joint.ConvertTo2<ClusterGeneratorTreeJoint, ClusterGenerator>(
                    (parentJoint, toConvertJoint) =>
                    {
                        return new ClusterGeneratorTreeJoint(
                            parentJoint,
                            new ClusterGenerator(
                                _compilation,
                                toConvertJoint
                                )
                            );
                    })
                );

            var customBindCount = container.Generators.Count(g => g.BindingContainer.Scope == BindScopeEnum.Custom);

            var result = @$"
#nullable disable

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.Bind;
using {typeof(ResolutionContext).Namespace};
using {typeof(ResolutionFrame).Namespace};
using {typeof(FixedSizeFactoryContainer).Namespace};
using {typeof(CustomScopeObject).Namespace};

{clusterGeneratorTree.GenerateUsingClauses()}

#endregion

namespace {ModuleTypeNamespace}
{{
//#nullable enable
    public partial class {ModuleTypeName} : {nameof(DpdtModule)}
    {{
        private static long _instanceCount = 0L;

        {clusterGeneratorTree.GenerateClusterDeclarationClauses()}

        private readonly {nameof(FixedSizeFactoryContainer)} _typeContainerGet;
        private readonly {nameof(FixedSizeFactoryContainer)} _typeContainerGetAll;
        private readonly {nameof(FixedSizeFactoryContainerCustomScope)} _typeContainerGetCustomScope;
        private readonly {nameof(FixedSizeFactoryContainerCustomScope)} _typeContainerGetAllCustomScope;

        public {ModuleTypeName}()
        {{
            if(Interlocked.Increment(ref _instanceCount) > 1L)
            {{
                throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, ""Module should not be instanciated more that once. This is a Dpdt's design axiom."");
            }}

            {clusterGeneratorTree.GenerateClusterAssignClauses()}

            _typeContainerGet = {ClusterGenerator.ClusterDefaultInstanceName}.TypeContainerGet;
            _typeContainerGetAll = {ClusterGenerator.ClusterDefaultInstanceName}.TypeContainerGetAll;
            _typeContainerGetCustomScope = {ClusterGenerator.ClusterDefaultInstanceName}.TypeContainerGetCustomScope;
            _typeContainerGetAllCustomScope = {ClusterGenerator.ClusterDefaultInstanceName}.TypeContainerGetAllCustomScope;
        }}

        public {nameof(CustomScopeObject)} CreateCustomScope()
        {{
            return CreateCustomScope(
                {customBindCount}
                );
        }}


        public override void Dispose()
        {{
            {clusterGeneratorTree.GenerateClusterDisposeClauses()}
        }}

#region Get Beautifiers

        public {typeof(IBeautifier).FullName} GetBeautifier()
        {{
            return (({nameof(IBindingProvider)}){ClusterGenerator.ClusterDefaultInstanceName}).{nameof(FakeCluster.Beautifier)};
        }}

        public {typeof(IBeautifier).FullName} GetBeautifier<TCluster>()
        {{
            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).GetCluster();
            return (({nameof(IBindingProvider)})cluster).{nameof(FakeCluster.Beautifier)};
        }}


#endregion


#region default cluster

        public bool IsRegisteredFrom<TRequestedType>()
        {{
            return {ClusterGenerator.ClusterDefaultInstanceName} is {nameof(IBindingProvider<object>)}<TRequestedType>;
        }}

#region embedded scope

        public TRequestedType Get<TRequestedType>()
        {{
            return (({nameof(IBindingProvider<object>)}<TRequestedType>){ClusterGenerator.ClusterDefaultInstanceName}).Get();
        }}
        public List<TRequestedType> GetAll<TRequestedType>()
        {{
            return (({nameof(IBindingProvider<object>)}<TRequestedType>){ClusterGenerator.ClusterDefaultInstanceName}).GetAll();
        }}

        public object Get({typeof(Type).FullName} requestedType)
        {{
            var result = _typeContainerGet.{nameof(FixedSizeFactoryContainer.GetGetObject)}(requestedType);

            return result;
        }}
        public IEnumerable<object> GetAll({typeof(Type).FullName} requestedType)
        {{
            var result = _typeContainerGetAll.{nameof(FixedSizeFactoryContainer.GetGetObject)}(requestedType);

            return (IEnumerable<object>)result;
        }}

#endregion


#region custom scope

        public TRequestedType Get<TRequestedType>({nameof(CustomScopeObject)} scope)
        {{
            return (({nameof(IBindingProvider<object>)}<TRequestedType>){ClusterGenerator.ClusterDefaultInstanceName}).Get(scope);
        }}
        public List<TRequestedType> GetAll<TRequestedType>({nameof(CustomScopeObject)} scope)
        {{
            return (({nameof(IBindingProvider<object>)}<TRequestedType>){ClusterGenerator.ClusterDefaultInstanceName}).GetAll(scope);
        }}

        public object Get({typeof(Type).FullName} requestedType, {nameof(CustomScopeObject)} scope)
        {{
            var result = _typeContainerGetCustomScope.{nameof(FixedSizeFactoryContainerCustomScope.GetGetObject)}(requestedType, scope);

            return result;
        }}
        public IEnumerable<object> GetAll({typeof(Type).FullName} requestedType, {nameof(CustomScopeObject)} scope)
        {{
            var result = _typeContainerGetAllCustomScope.{nameof(FixedSizeFactoryContainerCustomScope.GetGetObject)}(requestedType, scope);

            return (IEnumerable<object>)result;
        }}

#endregion


#endregion

#region custom clusters

        public bool IsRegisteredFrom<TCluster, TRequestedType>()
        {{
            if(_superCluster is {nameof(IClusterProvider<object>)}<TCluster>)
            {{
                return false;
            }}

            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
            return cluster is {nameof(IBindingProvider<object>)}<TRequestedType>;
        }}

#region embedded scope

        public TRequestedType Get<TCluster, TRequestedType>()
        {{
            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
            return (({nameof(IBindingProvider<object>)}<TRequestedType>)cluster).Get();
        }}
        public List<TRequestedType> GetAll<TCluster, TRequestedType>()
        {{
            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
            return (({nameof(IBindingProvider<object>)}<TRequestedType>)cluster).GetAll();
        }}


        public object Get<TCluster>(System.Type requestedType)
        {{
            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
            return (({nameof(IBindingProvider)})cluster).{nameof(IBindingProvider.TypeContainerGet)}.{nameof(FixedSizeFactoryContainer.GetGetObject)}(requestedType);
        }}
        public IEnumerable<object> GetAll<TCluster>(System.Type requestedType)
        {{
            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
            return (IEnumerable<object>)(({nameof(IBindingProvider)})cluster).{nameof(IBindingProvider.TypeContainerGetAll)}.{nameof(FixedSizeFactoryContainer.GetGetObject)}(requestedType);
        }}

#endregion


#region custom scope

        public TRequestedType Get<TCluster, TRequestedType>({nameof(CustomScopeObject)} scope)
        {{
            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
            return (({nameof(IBindingProvider<object>)}<TRequestedType>)cluster).Get(scope);
        }}
        public List<TRequestedType> GetAll<TCluster, TRequestedType>({nameof(CustomScopeObject)} scope)
        {{
            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
            return (({nameof(IBindingProvider<object>)}<TRequestedType>)cluster).GetAll(scope);
        }}


        public object Get<TCluster>(System.Type requestedType, {nameof(CustomScopeObject)} scope)
        {{
            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
            return (({nameof(IBindingProvider)})cluster).{nameof(IBindingProvider.TypeContainerGetCustomScope)}.{nameof(FixedSizeFactoryContainerCustomScope.GetGetObject)}(requestedType, scope);
        }}
        public IEnumerable<object> GetAll<TCluster>(System.Type requestedType, {nameof(CustomScopeObject)} scope)
        {{
            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
            return (IEnumerable<object>)(({nameof(IBindingProvider)})cluster).{nameof(IBindingProvider.TypeContainerGetAllCustomScope)}.{nameof(FixedSizeFactoryContainerCustomScope.GetGetObject)}(requestedType, scope);
        }}

#endregion

#endregion

#region Clusters

        {clusterGeneratorTree.GenerateSuperClusterBody()}

        {clusterGeneratorTree.GenerateClusterBodies()}

//#nullable disable

#endregion


    }}

}}
";
            return result;
        }

    }
}
