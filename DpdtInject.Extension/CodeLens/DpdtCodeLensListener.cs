using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.Options;
using DpdtInject.Extension.Shared;
using DpdtInject.Extension.Shared.Dto;
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

        public Task<bool> IsEnabled(
            )
        {
            var opts = GeneralOptions.Instance;
            return Task.FromResult(opts.Enabled);
        }

        public Task<DpdtBindingReferenceSet> GetReferenceSet(
            CodeLensTarget target
            )
        {
            var opts = GeneralOptions.Instance;
            if (!opts.Enabled)
            {
                return Task.FromResult(DpdtBindingReferenceSet.GetDisabled(target));
            }

            var solutionBind = _containerAndScanner.Binds;
            if (solutionBind == null)
            {
                return Task.FromResult(DpdtBindingReferenceSet.GetInProgress(target));
            }

            var list = new List<DpdtBindingTarget>();
            foreach (var clusterBind in solutionBind.ClusterBindContainers)
            {
                foreach (var mpair in clusterBind.GetMethodBindContainerDict())
                {
                    var methodBind = mpair.Value;

                    foreach (var binding in methodBind.Bindings)
                    {
                        var bindingFound = binding.BindToType.FullyQualifiedName == target.FullyQualifiedName;
                        if (bindingFound)
                        {
                            var bt = new DpdtBindingTarget(
                                new DpdtBindingIdentifier(
                                    binding.UniqueUnstableIdentifier
                                    ),
                                new DpdtClusterDetail(
                                    clusterBind.ClusterTypeInfo.FullNamespaceDisplayName,
                                    clusterBind.ClusterTypeInfo.Name,
                                    mpair.Key
                                    ),
                                new DpdtBindingDetail(
                                    binding.ScopeString,
                                    binding.IsConditional,
                                    binding.IsConventional
                                    )
                                );

                            list.Add(bt);
                        }
                    }
                }
            }

            return Task.FromResult(DpdtBindingReferenceSet.GetWithResults(target, list));
        }

        public int GetVisualStudioPid() => Process.GetCurrentProcess().Id;

    }
}
