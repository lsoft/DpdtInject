using DpdtInject.Generator.ArgumentWrapper;
using DpdtInject.Generator.Beautify;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
using DpdtInject.Generator.Tree;
using DpdtInject.Injector;
using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module;
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
        public const string ClusterCustomInstanceName = "_cluster";

        private readonly List<ClusterInterfaceGenerator> _interfaceSection;
        private readonly Compilation _compilation;

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

                return $"{ClusterCustomInstanceName}{this.GetHashCode()}";
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
                        new ClusterInterfaceGenerator(this, bindFromType, DpdtArgumentWrapperTypeEnum.None, generators)
                        );

                    foreach (var (wrapperType, key) in bindFromType.GenerateWrapperTypes(_compilation))
                    {
                        _interfaceSection.Add(
                            new ClusterInterfaceGenerator(this, key, wrapperType, generators)
                            );
                    }
                }
            }
        }

        public string GetCombinedInterfaces()
        {
            if (_interfaceSection.Count == 0)
            {
                return string.Empty;
            }

            return "," + string.Join(",", _interfaceSection.Select(j => j.InterfaceSection));
        }

        public string GetCombinedImplementationSection()
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
