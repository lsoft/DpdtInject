using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector;
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

            ClusterClassName = $"BindCluster{GetHashCode()}";
        }

        public string GenerateClusterBody(
            )
        {
            return $@"
private class {ClusterClassName} : IDisposable
    {GetCombinedInterfaces()}
{{

    public string Name => ""{Joint.JointPayload.Name}"";

    public {ClusterClassName}()
    {{
    }}

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
