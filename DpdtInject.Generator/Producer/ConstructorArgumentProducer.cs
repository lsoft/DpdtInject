using DpdtInject.Generator.Binding;
using DpdtInject.Injector.Bind.Settings;
using DpdtInject.Injector.Excp;
using System;
using System.Linq;

namespace DpdtInject.Generator.Producer
{
    public class ConstructorArgumentProducer
    {
        public ClusterBindings ClusterBindings
        {
            get;
        }

        public BindingContainerExtender BindingExtender
        {
            get;
        }

        public DetectedConstructorArgument ConstructorArgument
        {
            get;
        }


        public ConstructorArgumentProducer(
            ClusterBindings clusterBindings,
            BindingContainerExtender bindingExtender,
            DetectedConstructorArgument constructorArgument
            )
        {
            if (clusterBindings is null)
            {
                throw new ArgumentNullException(nameof(clusterBindings));
            }

            if (bindingExtender is null)
            {
                throw new ArgumentNullException(nameof(bindingExtender));
            }

            if (constructorArgument is null)
            {
                throw new ArgumentNullException(nameof(constructorArgument));
            }

            ClusterBindings = clusterBindings;
            BindingExtender = bindingExtender;
            ConstructorArgument = constructorArgument;
        }

        public ConstructorArgumentProduct Produce(out UnknownTypeProduct? utp)
        {
            utp = null;

            //DefineInBindNode should be checked BEFORE than HasExplicitDefaultValue
            if (ConstructorArgument.DefineInBindNode)
            {
                return 
                    new ConstructorArgumentProduct(
                        $"{ConstructorArgument.Name}: {ConstructorArgument.Body}"
                        );
            }
            if (ConstructorArgument.HasExplicitDefaultValue)
            {
                return ConstructorArgumentProduct.Empty;
            }

            //check for own cluster can resolve
            var clusterCanGetChildren = ClusterBindings.Box.TryGetChildren(
                ConstructorArgument,
                out var pairs
                );

            var crossClusterSetting = CrossClusterSettingEnum.OnlyLocal;
            if (BindingExtender.BindingContainer.TryGetSettingInScope<CrossClusterSettings>(out var setting))
            {
                crossClusterSetting = setting.Setting;
            }

            if (crossClusterSetting == CrossClusterSettingEnum.OnlyLocal && !clusterCanGetChildren)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for {ConstructorArgument.Type!.ToDisplayString()}, consider to relax cross-cluster restriction (setting)",
                    ConstructorArgument.Type.ToDisplayString()
                    );
            }
            if (crossClusterSetting == CrossClusterSettingEnum.MustBeCrossCluster && clusterCanGetChildren)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.LocalBindingFound,
                    $"Local binding for {ConstructorArgument.Type!.ToDisplayString()} has been found, but it's forbidden by specific setting, consider to relax cross-cluster restriction (setting)",
                    ConstructorArgument.Type.ToDisplayString()
                    );
            }

            var totalContainerCount = pairs.Count;
            var conditionlessContainerCount = pairs.Count(p => !p.BindingExtender.BindingContainer.IsConditional);

            if (conditionlessContainerCount > 1)
            {
                return
                    new ConstructorArgumentProduct(
                            $"{ConstructorArgument.Name}: RaiseTooManyBindingException<{ConstructorArgument.Type!.ToDisplayString()}>()"
                        );
            }

            if (clusterCanGetChildren)
            {
                if (BindingExtender.NeedToProcessResolutionContext)
                {
                    //actually it's not faster but a bit slower
                    //return
                    //    new ConstructorArgumentProduct(
                    //        $"{ConstructorArgument.Name}: GetFast(default({ConstructorArgument.Type!.ToDisplayString()}), resolutionTarget, \"{ConstructorArgument.Name}\" )"
                    //        );

                    return
                        new ConstructorArgumentProduct(
                                $"{ConstructorArgument.Name}: GetFromLocalUnsafely<{ConstructorArgument.Type!.ToDisplayString()}>( resolutionTarget, \"{ConstructorArgument.Name}\" )"
                            );
                }
                else
                {
                    //actually it's not faster but a bit slower
                    //return
                    //    new ConstructorArgumentProduct(
                    //        $"{ConstructorArgument.Name}: GetFast(default({ConstructorArgument.Type!.ToDisplayString()}))"
                    //        );

                    return
                        new ConstructorArgumentProduct(
                                $"{ConstructorArgument.Name}: GetFromLocalUnsafely<{ConstructorArgument.Type!.ToDisplayString()}>()"
                            );
                }
            }
            else
            {
                //cluster has no bindings, refer to parent cluster

                utp = new UnknownTypeProduct(
                    ConstructorArgument.Type!
                    );

                return
                    new ConstructorArgumentProduct(
                            $"{ConstructorArgument.Name}: GetFromParent<{ConstructorArgument.Type!.ToDisplayString()}>( resolutionTarget, \"{ConstructorArgument.Name}\"  )"
                        );
            }
        }
    }
}
