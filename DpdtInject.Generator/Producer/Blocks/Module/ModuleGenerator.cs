using DpdtInject.Generator.Beautify;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Cluster;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Injector;
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

            var beautifyGenerator = new BeautifyGenerator(
                ModuleTypeName
                );

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

{temporalyFirstClusterGenerator.Joint.JointPayload.Generators.Join(sc => sc.Usings.Join(c => c))}

#endregion

namespace {ModuleTypeNamespace}
{{
//#nullable enable
    public partial class {ModuleTypeName} : {nameof(DpdtModule)}
    {{
        private static long _instanceCount = 0L;

        private readonly {temporalyFirstClusterGenerator.ClusterClassName} _cluster;
        private readonly {typeof(ReinventedContainer).FullName} _typeContainerGet;
        private readonly {typeof(ReinventedContainer).FullName} _typeContainerGetAll;

        private readonly {beautifyGenerator.ClassName} _beautifier;

        public {beautifyGenerator.ClassName} Beautifier => _beautifier;

        public {ModuleTypeName}()
        {{
            if(Interlocked.Increment(ref _instanceCount) > 1L)
            {{
                throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, ""Module should not be instanciated more that once. This is a Dpdt's design axiom."");
            }}

            _cluster = new {temporalyFirstClusterGenerator.ClusterClassName}(
                );

            _typeContainerGet = new {typeof(ReinventedContainer).FullName}(
                {temporalyFirstClusterGenerator.Joint.JointPayload.GetReinventedContainerArgument("Get")}
                );
            _typeContainerGetAll = new {typeof(ReinventedContainer).FullName}(
                {temporalyFirstClusterGenerator.Joint.JointPayload.GetReinventedContainerArgument("GetAll")}
                );

            _beautifier = new {beautifyGenerator.ClassName}(
                this
                );
        }}


        public override void Dispose()
        {{
            _cluster.Dispose();
        }}

        public bool IsRegisteredFrom<T>()
        {{
            return _cluster is {nameof(IClusterProvider<object>)}<T>;
        }}


        public T Get<T>()
        {{
            return (({nameof(IClusterProvider<object>)}<T>)_cluster).Get();
        }}
        public List<T> GetAll<T>()
        {{
            return (({nameof(IClusterProvider<object>)}<T>)_cluster).GetAll();
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

#region Beautify

        {beautifyGenerator.GenerateBeautifierBody()}

#endregion

#region Clusters

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
