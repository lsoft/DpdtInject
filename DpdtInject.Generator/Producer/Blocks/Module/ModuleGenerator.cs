using DpdtInject.Generator.Beautify;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Cluster;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Injector;
using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Compilation;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Threading;

namespace DpdtInject.Generator.Producer.Blocks.Module
{
    internal class ModuleGenerator
    {
        private readonly Compilation _compilation;

        public INamedTypeSymbol ModuleSymbol
        {
            get;
        }

        public string ModuleTypeNamespace => ModuleSymbol.ContainingNamespace.GetFullName();

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
            InstanceContainerGeneratorsContainer container
            )
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var clusterGeneratorTree = new ClusterGeneratorTree(
                container.GeneratorTree.Joint.ConvertTo<ClusterGeneratorTreeJoint, ClusterGenerator>(
                    joint =>
                    {
                        return new ClusterGeneratorTreeJoint(
                            new ClusterGenerator(
                                _compilation,
                                joint
                                )
                            );
                    })
                );

            var temporalyFirstClusterGenerator = clusterGeneratorTree.Joint.JointPayload;

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

{clusterGeneratorTree.GenerateUsingClauses()}

#endregion

namespace {ModuleTypeNamespace}
{{
//#nullable enable
    public partial class {ModuleTypeName} : {nameof(DpdtModule)}
    {{
        private static long _instanceCount = 0L;

        {clusterGeneratorTree.GenerateClusterDeclarationClauses()}

        private readonly {typeof(ReinventedContainer).FullName} _typeContainerGet;
        private readonly {typeof(ReinventedContainer).FullName} _typeContainerGetAll;

        public {ModuleTypeName}()
        {{
            if(Interlocked.Increment(ref _instanceCount) > 1L)
            {{
                throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, ""Module should not be instanciated more that once. This is a Dpdt's design axiom."");
            }}

            {clusterGeneratorTree.GenerateClusterAssignClauses()}

            _typeContainerGet = {ClusterGenerator.ClusterDefaultInstanceName}.TypeContainerGet;
            _typeContainerGetAll = {ClusterGenerator.ClusterDefaultInstanceName}.TypeContainerGetAll;
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
            var result = _typeContainerGet.{nameof(ReinventedContainer.GetGetObject)}(requestedType);

            return result;
        }}
        public IEnumerable<object> GetAll({typeof(Type).FullName} requestedType)
        {{
            var result = _typeContainerGetAll.{nameof(ReinventedContainer.GetGetObject)}(requestedType);

            return (IEnumerable<object>)result;
        }}

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
            return (({nameof(IBindingProvider)})cluster).{nameof(IBindingProvider.TypeContainerGet)}.{nameof(ReinventedContainer.GetGetObject)}(requestedType);
        }}
        public IEnumerable<object> GetAll<TCluster>(System.Type requestedType)
        {{
            var cluster = (({nameof(IClusterProvider<object>)}<TCluster>)_superCluster).{nameof(IClusterProvider<object>.GetCluster)}();
            return (IEnumerable<object>)(({nameof(IBindingProvider)})cluster).{nameof(IBindingProvider.TypeContainerGetAll)}.{nameof(ReinventedContainer.GetGetObject)}(requestedType);
        }}

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
