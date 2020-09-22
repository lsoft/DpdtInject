using DpdtInject.Generator.Tree;
using DpdtInject.Injector.Excp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DpdtInject.Generator.Producer.Blocks.Binding
{
    public class InstanceContainerGeneratorTree

    {
        public InstanceContainerGeneratorTreeJoint Joint
        {
            get;
        }

        public InstanceContainerGeneratorCluster JointPayload => Joint.JointPayload;

        public InstanceContainerGeneratorTree(
            InstanceContainerGeneratorTreeJoint joint
            )
        {
            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            Joint = joint;
        }

        internal void Apply(
            Action<InstanceContainerGeneratorTreeJoint> action
            )
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Action<TreeJoint<InstanceContainerGeneratorCluster>> action2 = (payload) => action((InstanceContainerGeneratorTreeJoint)payload);

            Joint.Apply(action2);
        }

        public void Apply(
            Action<InstanceContainerGeneratorCluster> action
            )
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Joint.Apply(action);
        }

        public bool TryFindInItsParents(
            Func<InstanceContainerGeneratorCluster, bool> predicate,
            [NotNullWhen(true)] out InstanceContainerGeneratorTreeJoint? foundJoint
            )
        {
            var r = 
                Joint.TryFindInItsParents(
                    predicate,
                    out var foundJoint2
                    );

            if(!r)
            {
                foundJoint = null;
                return false;
            }

            foundJoint = (InstanceContainerGeneratorTreeJoint)foundJoint2!;
            return true;
        }

        internal void BuildFlags()
        {
            CheckForAtLeastOneChildIsConditionalInternal(
                );
        }

        private void CheckForAtLeastOneChildIsConditionalInternal(
            )
        {
            foreach (var point2 in Joint.GeneratePoints2())
            {
                var itselfOrAtLeastOneChildIsConditional = CheckForItselfOrAtLeastOneChildIsConditionalInternal2(
                    point2,
                    new HashSet<InstanceContainerGenerator>()
                    );
                point2.Generator.ItselfOrAtLeastOneChildIsConditional = itselfOrAtLeastOneChildIsConditional;
            }
        }
        private bool CheckForItselfOrAtLeastOneChildIsConditionalInternal2(
            Point2 point2,
            HashSet<InstanceContainerGenerator> used
            )
        {
            if (point2 is null)
            {
                throw new ArgumentNullException(nameof(point2));
            }

            if (used is null)
            {
                throw new ArgumentNullException(nameof(used));
            }

            if(used.Contains(point2.Generator))
            {
                //found cycle, skip this subtree as circular
                return false;
            }
            used.Add(point2.Generator);

            if(point2.Generator.BindingContainer.IsConditional)
            {
                return true;
            }

            foreach (var ca in point2.Generator.BindingContainer.ConstructorArguments.Where(ca => !ca.DefineInBindNode))
            {
                if (ca.Type is null)
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"constructorArgument.Type is null somehow"
                        );
                }

                var childPoint3 = new Point3(
                    point2.Joint,
                    point2.Generator,
                    ca.Type
                    );

                if (childPoint3.TryFindChildren(out var childrenPoint2))
                {
                    foreach (var childPoint2 in childrenPoint2)
                    {
                        if(CheckForItselfOrAtLeastOneChildIsConditionalInternal2(
                            childPoint2,
                            new HashSet<InstanceContainerGenerator>(used)
                            ))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    public class InstanceContainerGeneratorTreeJoint : TreeJoint<InstanceContainerGeneratorCluster>
    {
        public InstanceContainerGeneratorTreeJoint(InstanceContainerGeneratorCluster jointPayload)
            : base(jointPayload)
        {
        }

        public IReadOnlyList<Point2> GeneratePoints2(
            )
        {
            var result = new List<Point2>();

            this.Apply(
                joint =>
                {
                    foreach (var pair in joint.JointPayload.GeneratorGroups)
                    {
                        foreach (var generator in pair.Value.Generators)
                        {
                            var point2 = new Point2(
                                this,
                                generator
                                );

                            result.Add(point2);
                        }
                    }
                });

            return result;
        }

        internal IReadOnlyList<Point3> GenerateRegisteredTypePoints()
        {
            var result = new List<Point3>();

            this.Apply(
                joint =>
                {
                    foreach (var pair in joint.JointPayload.GeneratorGroups)
                    {
                        foreach (var generator in pair.Value.Generators)
                        {
                            foreach (var bindFrom in generator.BindFromTypes)
                            {
                                var point3 = new Point3(
                                    this,
                                    generator,
                                    bindFrom
                                    );

                                result.Add(point3);
                            }
                        }
                    }
                });

            return result;
        }


        internal IReadOnlyList<Point3> GenerateChildPoints()
        {
            var result = new List<Point3>();

            this.Apply(
                (TreeJoint<InstanceContainerGeneratorCluster> joint) =>
                {
                    foreach (var pair in joint.JointPayload.GeneratorGroups)
                    {
                        foreach (var generator in pair.Value.Generators)
                        {
                            foreach (var ca in generator.BindingContainer.ConstructorArguments.Where(ca => !ca.DefineInBindNode))
                            {
                                if (ca.Type is null)
                                {
                                    throw new DpdtException(DpdtExceptionTypeEnum.GeneralError, $"ca.Type is null somehow");
                                }

                                var point3 = new Point3(
                                    this,
                                    generator,
                                    ca.Type
                                    );

                                result.Add(point3);
                            }
                        }
                    }
                });

            return result;
        }

        internal IReadOnlyList<InstanceContainerGeneratorTreeJoint> GenerateJoints()
        {
            var result = new List<InstanceContainerGeneratorTreeJoint>();

            GenerateJointsInternal(this, result);

            return result;
        }

        internal void GenerateJointsInternal(
            InstanceContainerGeneratorTreeJoint joint,
            List<InstanceContainerGeneratorTreeJoint> result
            )
        {
            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            result.Add(joint);

            foreach (var child in joint.Children)
            {
                GenerateJointsInternal((InstanceContainerGeneratorTreeJoint)child, result);
            }
        }

    }


    public class Point2
    {
        public TreeJoint<InstanceContainerGeneratorCluster> Joint
        {
            get;
        }

        public InstanceContainerGenerator Generator
        {
            get;
        }

        public Point2(
            TreeJoint<InstanceContainerGeneratorCluster> joint,
            InstanceContainerGenerator generator
            )
        {
            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            if (generator is null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            Joint = joint;
            Generator = generator;
        }

    }

    public class Point3
    {
        public TreeJoint<InstanceContainerGeneratorCluster> Joint
        {
            get;
        }

        public InstanceContainerGenerator Generator
        {
            get;
        }

        public ITypeSymbol TypeSymbol
        {
            get;
        }

        public Point3(
            TreeJoint<InstanceContainerGeneratorCluster> joint,
            InstanceContainerGenerator generator,
            ITypeSymbol typeSymbol
            )
        {
            if (joint is null)
            {
                throw new ArgumentNullException(nameof(joint));
            }

            if (generator is null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            if (typeSymbol is null)
            {
                throw new ArgumentNullException(nameof(typeSymbol));
            }

            Joint = joint;
            Generator = generator;
            TypeSymbol = typeSymbol;
        }

    }

    public static class GeneratorTreeHelper
    {
        public static bool TryFindChildren(
            this Point3 point3,
            out IReadOnlyList<Point2> results
            )
        {
            if (point3 is null)
            {
                throw new ArgumentNullException(nameof(point3));
            }

            var result = new List<Point2>();

            FindChildren(point3, ref result);

            results = result;
            return result.Count > 0;
        }

        private static void FindChildren(
            Point3 point3,
            ref List<Point2> results
            )
        {
            if (point3 is null)
            {
                throw new ArgumentNullException(nameof(point3));
            }

            if (results is null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            if (!point3.Joint.JointPayload.TryGetRegisteredGeneratorGroups(
                point3.TypeSymbol,
                true,
                out var groups))
            {
                //no children found in current joint, move to parent joint

                if (point3.Joint.IsRoot)
                {
                    return;
                }

                var parentPoint3 = new Point3(
                    point3.Joint.Parent!,
                    point3.Generator,
                    point3.TypeSymbol
                    );

                FindChildren(
                    parentPoint3,
                    ref results
                    );
            }
            else
            {
                //we have children in this joint (cluster), no need to scan parent joints(clusters)

                foreach (var group in groups)
                {
                    foreach (var generator in group.Generators)
                    {
                        var rItem = new Point2(
                            point3.Joint,
                            generator
                            );

                        results.Add(rItem);
                    }
                }
            }
        }
    }

}
