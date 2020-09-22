using DpdtInject.Generator.Beautify;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector;
using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.Bind;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Producer.Blocks.Cluster
{
    public class ClusterGenerator
    {
        public const string ClusterDefaultInstanceName = "_defaultCluster";

        private readonly List<ClusterInterfaceGenerator> _interfaceSection;
        private readonly Compilation _compilation;

        public string ClusterClassName
        {
            get;
        }

        public TreeJoint<InstanceContainerGeneratorCluster> Joint
        {
            get;
        }
        public string ClusterStableInstanceName
        {
            get
            {
                if (Joint.JointPayload.IsRootCluster)
                {
                    return ClusterDefaultInstanceName;
                }

                return $"{ClusterDefaultInstanceName}{this.GetHashCode()}";
            }
        }

        public ClusterGenerator(
            Compilation compilation,
            TreeJoint<InstanceContainerGeneratorCluster> joint
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }
            _compilation = compilation;
            Joint = joint;

            _interfaceSection = new List<ClusterInterfaceGenerator>();

            foreach (var (_, bindFromType) in Joint.JointPayload.GetRegisteredKeys(false))
            {
                if (Joint.JointPayload.TryGetRegisteredGeneratorGroups(bindFromType, false, out var groups))
                {
                    var generators = groups.Collapse(
                        group => group.Generators
                        );

                    _interfaceSection.Add(
                        new ClusterInterfaceGenerator(bindFromType, DpdtArgumentWrapperTypeEnum.None, generators)
                        );

                    foreach (var (wrapperType, key) in bindFromType.GenerateWrapperTypes(_compilation))
                    {
                        _interfaceSection.Add(
                            new ClusterInterfaceGenerator(key, wrapperType, generators)
                            );
                    }
                }
            }

            if(Joint.JointPayload.IsRootCluster)
            {
                ClusterClassName = $"DefaultCluster";
            }
            else
            {
                ClusterClassName = $"Cluster{GetHashCode()}";
            }
        }

        public string GenerateClusterBody(
            )
        {
            var beautifyGenerator = new BeautifyGenerator(
                ClusterClassName
                );

            return $@"
private class {ClusterClassName} : {nameof(IDisposable)}, {nameof(IBindingProvider)}
    {GetCombinedInterfaces()}
{{
    private readonly {beautifyGenerator.ClassName} _beautifier;

    public {typeof(ReinventedContainer).FullName} TypeContainerGet
    {{
        get;
    }}

    public {typeof(ReinventedContainer).FullName} TypeContainerGetAll
    {{
        get;
    }}

    public System.Type DeclaredClusterType => {Joint.JointPayload.PrepareDeclaredClusterType()};

    public bool IsRootCluster => DeclaredClusterType is null;

    public {typeof(IBeautifier).FullName} Beautifier => _beautifier;

    public {ClusterClassName}()
    {{
        TypeContainerGet = new {typeof(ReinventedContainer).FullName}(
            {Joint.JointPayload.GetReinventedContainerArgument("Get")}
            );
        TypeContainerGetAll = new {typeof(ReinventedContainer).FullName}(
            {Joint.JointPayload.GetReinventedContainerArgument("GetAll")}
            );

        _beautifier = new {beautifyGenerator.ClassName}(
            this
            );
    }}

    public bool IsRegisteredFrom<TRequestedType>()
    {{
        return this is {nameof(IBindingProvider<object>)}<TRequestedType>;
    }}

    public TRequestedType Get<TRequestedType>()
    {{
        return (({nameof(IBindingProvider<object>)}<TRequestedType>)this).Get();
    }}

    public List<TRequestedType> GetAll<TRequestedType>()
    {{
        return (({nameof(IBindingProvider<object>)}<TRequestedType>)this).GetAll();
    }}

    public object Get({typeof(Type).FullName} requestedType)
    {{
        var result = TypeContainerGet.{nameof(ReinventedContainer.GetGetObject)}(requestedType);

        return result;
    }}
    public IEnumerable<object> GetAll({typeof(Type).FullName} requestedType)
    {{
        var result = TypeContainerGetAll.{nameof(ReinventedContainer.GetGetObject)}(requestedType);

        return (IEnumerable<object>)result;
    }}

#region Beautify

    {beautifyGenerator.GenerateBeautifierBody()}

#endregion

    public void Dispose()
    {{
        {Joint.JointPayload.Generators.Where(icg => icg.BindingContainer.Scope.In(BindScopeEnum.Singleton)).Join(sc => sc.DisposeClause + ";")}
    }}

    {GetCombinedImplementationSection()}

#region Instance Containers
    {Joint.JointPayload.Generators.Join(sc => sc.GetClassBody(Joint.JointPayload))}
#endregion

}}
";
        }

        private string GetCombinedInterfaces()
        {
            if (_interfaceSection.Count == 0)
            {
                return string.Empty;
            }

            return "," + string.Join(",", _interfaceSection.Select(j => j.InterfaceSection));
        }

        private string GetCombinedImplementationSection()
        {
            if (_interfaceSection.Count == 0)
            {
                return string.Empty;
            }

            return
                string.Join(Environment.NewLine, _interfaceSection.Select(i => i.ResolutionFrameSection))
                + string.Join(
                    Environment.NewLine,
                    _interfaceSection.Select(j =>
                        j.GetExplicitImplementationSection
                        + Environment.NewLine
                        + j.GetImplementationSection
                        + Environment.NewLine
                        + j.GetAllImplementationSection
                        + Environment.NewLine
                        + j.GetAllExplicitImplementationSection
                        )
                    )
                ;
        }

    }

}
