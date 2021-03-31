using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.Options;
using DpdtInject.Extension.Shared;
using DpdtInject.Extension.Shared.Dto;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.Producer;
using Microsoft.VisualStudio.Language.CodeLens;
using Microsoft.VisualStudio.Utilities;

namespace DpdtInject.Extension
{
    [Export(typeof(ICodeLensCallbackListener))]
    [ContentType("CSharp")]
    public class DpdtCodeLensListener : ICodeLensCallbackListener, IDpdtCodeLensListener
    {
        private readonly ContainerAndScanner _containerAndScanner;

        [ImportingConstructor]
        public DpdtCodeLensListener(
            ContainerAndScanner containerAndScanner
            )
        {
            if (containerAndScanner is null)
            {
                throw new ArgumentNullException(nameof(containerAndScanner));
            }

            _containerAndScanner = containerAndScanner;
        }

        public async Task<bool> IsEnabled(
            )
        {
            var opts = GeneralOptions.Instance;
            return opts.Enabled;
        }

        public async Task<DpdtBindingReferenceSet> GetReferenceSet(
            CodeLensTarget target
            )
        {
            var opts = GeneralOptions.Instance;
            if (!opts.Enabled)
            {
                return DpdtBindingReferenceSet.GetDisabled(target);
            }

            var solutionBind = _containerAndScanner.Binds;
            if (solutionBind == null)
            {
                return DpdtBindingReferenceSet.GetInProgress(target);
            }

            var list = new List<DpdtBindingTarget>();
            foreach (var ppair in solutionBind.Dict)
            {
                var projectBind = ppair.Value;

                foreach (var cpair in projectBind.DictByDisplayString)
                {
                    var clusterBind = cpair.Value;

                    foreach (var mpair in clusterBind.Dict)
                    {
                        var methodBind = mpair.Value;

                        foreach (var bindingContainer in methodBind.BindingContainers)
                        {
                            var bindingFound = bindingContainer.BindToType.GetFullyQualifiedName() == target.FullyQualifiedName;
                            if (bindingFound)
                            {
                                list.Add(
                                    new DpdtBindingTarget(
                                        bindingContainer.Identifier,
                                        new DpdtClusterDetail(
                                            clusterBind.ClusterType.ContainingNamespace.ToDisplayString(),
                                            clusterBind.ClusterType.Name,
                                            mpair.Key
                                            ),
                                        new DpdtBindingDetail(
                                            bindingContainer.Scope.ToString(),
                                            bindingContainer.IsConditional,
                                            bindingContainer.IsConventional
                                            )
                                        )
                                    );
                            }
                        }
                    }
                }
            }

            return DpdtBindingReferenceSet.GetWithResults(target, list);
        }

        public int GetVisualStudioPid() => Process.GetCurrentProcess().Id;

    }
}
