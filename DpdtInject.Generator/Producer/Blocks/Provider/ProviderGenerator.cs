using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.Producer.Blocks.Binding;
using DpdtInject.Generator.Producer.Blocks.Exception;
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

namespace DpdtInject.Generator.Producer.Blocks.Provider
{
    public class ProviderGenerator
    {
        private readonly List<ProviderInterfaceGenerator> _interfaceSection;
        private readonly Compilation _compilation;
        private readonly GeneratorCluster _cluster;
        
        public string ProviderClassName
        {
            get;
        }


        public ProviderGenerator(
            Compilation compilation,
            GeneratorCluster cluster
            )
        {
            if (compilation is null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (cluster is null)
            {
                throw new ArgumentNullException(nameof(cluster));
            }
            _compilation = compilation;
            _cluster = cluster;

            _interfaceSection = new List<ProviderInterfaceGenerator>();

            foreach (var (_, bindFromType) in _cluster.GetRegisteredKeys(false))
            {
                if(_cluster.TryGetRegisteredGeneratorGroups(bindFromType, false, out var groups))
                {
                    var generators = groups.Collapse(
                        group => group.Generators
                        );

                    _interfaceSection.Add(
                        new ProviderInterfaceGenerator(bindFromType, DpdtArgumentWrapperTypeEnum.None, generators)
                        );

                    foreach (var (wrapperType, key) in bindFromType.GenerateWrapperTypes(_compilation))
                    {
                        _interfaceSection.Add(
                            new ProviderInterfaceGenerator(key, wrapperType, generators)
                            );
                    }
                }
            }

            ProviderClassName = $"{nameof(Provider)}{this.GetHashCode()}";
        }

        public string GenerateProviderBody(
            )
        {
            return $@"
private class {ProviderClassName} : IDisposable
    {GetCombinedInterfaces()}
{{

    public {ProviderClassName}()
    {{
    }}

    public void Dispose()
    {{
        {_cluster.Generators.Where(icg => icg.BindingContainer.Scope.In(BindScopeEnum.Singleton)).Join(sc => sc.DisposeClause + ";")}
    }}

    {GetCombinedImplementationSection()}

#region Instance Containers
    {_cluster.Generators.Join(sc => sc.GetClassBody(_cluster))}
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
