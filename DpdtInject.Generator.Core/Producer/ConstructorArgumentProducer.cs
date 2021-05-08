using DpdtInject.Generator.Core.Binding;
using System;
using System.Linq;
using DpdtInject.Injector.Src.Bind.Settings.CrossCluster;
using DpdtInject.Injector.Src.Excp;
using DpdtInject.Injector.Src.Bind.Settings;

namespace DpdtInject.Generator.Core.Producer
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

        public DetectedMethodArgument ConstructorArgument
        {
            get;
        }


        public ConstructorArgumentProducer(
            ClusterBindings clusterBindings,
            BindingContainerExtender bindingExtender,
            DetectedMethodArgument constructorArgument
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
                        $"({ConstructorArgument.Type!.ToGlobalDisplayString()})({ConstructorArgument.Body})"
                        );
                //return
                //    new ConstructorArgumentProduct(
                //        $"{ConstructorArgument.Name}: ({ConstructorArgument.Type!.ToGlobalDisplayString()})({ConstructorArgument.Body})"
                //        );
            }
            if (ConstructorArgument.HasExplicitDefaultValue)
            {
                return
                    new ConstructorArgumentProduct(
                        $"({ConstructorArgument.Type!.ToGlobalDisplayString()})({ConstructorArgument.GetExplicitValueCodeRepresentation()})"
                        );
                //return
                //    new ConstructorArgumentProduct(
                //        $"{ConstructorArgument.Name}: ({ConstructorArgument.Type!.ToGlobalDisplayString()})({ConstructorArgument.GetExplicitValueCodeRepresentation()})"
                //        );
            }

            //check for own cluster can resolve
            var clusterCanGetChildren = ClusterBindings.Box.TryGetChildren(
                ConstructorArgument,
                out var pairs
                );

            var crossClusterSetting = CrossClusterSettingEnum.OnlyLocal;
            if (BindingExtender.BindingContainer.Settings.TryGetSettingInScope<CrossClusterSettings>(CrossClusterSettings.ScopeConstant, out var setting))
            {
                crossClusterSetting = setting.Setting;
            }

            if (crossClusterSetting == CrossClusterSettingEnum.OnlyLocal && !clusterCanGetChildren)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for {ConstructorArgument.Type!.ToGlobalDisplayString()}, consider to relax cross-cluster restriction (setting)",
                    ConstructorArgument.Type!.ToGlobalDisplayString()
                    );
            }
            if (crossClusterSetting == CrossClusterSettingEnum.MustBeCrossCluster && clusterCanGetChildren)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.LocalBindingFound,
                    $"Local binding for {ConstructorArgument.Type!.ToGlobalDisplayString()} has been found, but it's forbidden by specific setting, consider to relax cross-cluster restriction (setting)",
                    ConstructorArgument.Type!.ToGlobalDisplayString()
                    );
            }

            var totalContainerCount = pairs.Count;
            var conditionlessContainerCount = pairs.Count(p => !p.BindingExtender.BindingContainer.IsConditional);

            if (conditionlessContainerCount > 1)
            {
                return
                    new ConstructorArgumentProduct(
                            $"RaiseTooManyBindingException<{ConstructorArgument.Type!.ToGlobalDisplayString()}>()"
                        );
                //return
                //    new ConstructorArgumentProduct(
                //            $"{ConstructorArgument.Name}: RaiseTooManyBindingException<{ConstructorArgument.Type!.ToGlobalDisplayString()}>()"
                //        );
            }

            if (clusterCanGetChildren)
            {
                if (BindingExtender.NeedToProcessResolutionContext)
                {
                    return
                        new ConstructorArgumentProduct(
                                $"GetFromLocalUnsafely<{ConstructorArgument.Type!.ToGlobalDisplayString()}>( resolutionTarget, \"{ConstructorArgument.Name}\" )"
                            );

                    //return
                    //    new ConstructorArgumentProduct(
                    //            $"{ConstructorArgument.Name}: GetFromLocalUnsafely<{ConstructorArgument.Type!.ToGlobalDisplayString()}>( resolutionTarget, \"{ConstructorArgument.Name}\" )"
                    //        );
                }
                else
                {
                    return
                        new ConstructorArgumentProduct(
                                $"GetFromLocalUnsafely<{ConstructorArgument.Type!.ToGlobalDisplayString()}>()"
                            );

                    //return
                    //    new ConstructorArgumentProduct(
                    //            $"{ConstructorArgument.Name}: GetFromLocalUnsafely<{ConstructorArgument.Type!.ToGlobalDisplayString()}>()"
                    //        );
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
                            $"GetFromParent<{ConstructorArgument.Type!.ToGlobalDisplayString()}>( resolutionTarget, \"{ConstructorArgument.Name}\"  )"
                        );
                    //new ConstructorArgumentProduct(
                    //        $"{ConstructorArgument.Name}: GetFromParent<{ConstructorArgument.Type!.ToGlobalDisplayString()}>( resolutionTarget, \"{ConstructorArgument.Name}\"  )"
                    //    );
            }
        }
    }
}
