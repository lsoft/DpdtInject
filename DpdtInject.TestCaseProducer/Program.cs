using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DpdtInject.TestCaseProducer
{
    class Program
    {
        public const int BindCount = 500;

        public static int Seed =
            //BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
            -624853138;

        static void Main(string[] args)
        {
            var createdNodes =
                //GeneratePlainNodes(Seed);
                GenerateNodesInTree(Seed);
                //GenerateNodesInColumn(Seed);

            var rootTargetDirectory = @"../../../../DpdtInject.Tests.Performance/TimeConsume/BigTree0";
            var rootNameSpace = "DpdtInject.Tests.Performance.TimeConsume.BigTree0";

            if (!Directory.Exists(rootTargetDirectory))
            {
                Directory.CreateDirectory(rootTargetDirectory);
            }

            foreach (var scope in new[] { ScopeTypeEnum.Singleton, ScopeTypeEnum.Transient })
            {
                var targetDirectory = rootTargetDirectory + "/" + scope.ToString();
                var nameSpace = rootNameSpace + "." + scope.ToString();
                
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }


                #region subject

                var subjectFileName = "Subject.cs";
                var subjectFilePath = Path.Combine(targetDirectory, subjectFileName);

                var subjectSourceCode = $@"
//seed: {Seed}
using System;
using System.Collections.Generic;
using System.Text;

namespace {nameSpace}
{{
    {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetSubjectCode()))};
}}
";

                subjectSourceCode = SyntaxFactory.ParseCompilationUnit(subjectSourceCode).NormalizeWhitespace().GetText().ToString();

                File.WriteAllText(
                    subjectFilePath,
                    subjectSourceCode
                    );

                #endregion

                var nodeGroups = (
                    from node in createdNodes
                    group node by (createdNodes.IndexOf(node) / 50)
                    into nodegroup
                    select nodegroup
                    ).ToList();

                #region dpdt cluster

                var clusterFileName = "DpdtCluster.cs";
                var clusterFilePath = Path.Combine(targetDirectory, clusterFileName);
                var clusterClassName = "TimeConsumeBigTree0_Cluster";

                var dpdtClusterCode = $@"
//seed: {Seed}
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace {nameSpace}
{{
    public partial class {clusterClassName} : DefaultCluster
    {{
        public const int BindCount = {BindCount};
        public const string BindCountString = ""{BindCount}"";
        public const string GenericTestName = ""Dpdt.{ResolveTypeEnum.Generic}{scope}{BindCount}"";
        public const string NonGenericTestName = ""Dpdt.{ResolveTypeEnum.NonGeneric}{scope}{BindCount}"";
        public const string FastTestName = ""Dpdt.{ResolveTypeEnum.Fast}{scope}{BindCount}"";

        public override void Load()
        {{
#region bind code
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetDpdtBindCode(scope)))};
#endregion
        }}

#region resolution code

        public static void ResolveGeneric({clusterClassName} cluster)
        {{
            {string.Join(Environment.NewLine, nodeGroups.Select(g => $"ResolveGeneric{g.Key}(cluster);"))}
        }}

        {string.Join(Environment.NewLine, nodeGroups.Select(g => $"public static void ResolveGeneric{g.Key}({clusterClassName} cluster){{ {string.Join(Environment.NewLine, g.Select(cn => cn.GetDpdtResolutionCode(ResolveTypeEnum.Generic)))} }}"))}



        public static void ResolveNonGeneric({clusterClassName} cluster)
        {{
            {string.Join(Environment.NewLine, nodeGroups.Select(g => $"ResolveNonGeneric{g.Key}(cluster);"))}
        }}

        {string.Join(Environment.NewLine, nodeGroups.Select(g => $"public static void ResolveNonGeneric{g.Key}({clusterClassName} cluster){{ {string.Join(Environment.NewLine, g.Select(cn => cn.GetDpdtResolutionCode(ResolveTypeEnum.NonGeneric)))} }}"))}



        public static void ResolveFast({clusterClassName} cluster)
        {{
            {string.Join(Environment.NewLine, nodeGroups.Select(g => $"ResolveFast{g.Key}(cluster);"))}
        }}

        {string.Join(Environment.NewLine, nodeGroups.Select(g => $"public static void ResolveFast{g.Key}({clusterClassName} cluster){{ {string.Join(Environment.NewLine, g.Select(cn => cn.GetDpdtResolutionCode(ResolveTypeEnum.Fast)))} }}"))}

#endregion

    }}
}}
";


                dpdtClusterCode = SyntaxFactory.ParseCompilationUnit(dpdtClusterCode).NormalizeWhitespace().GetText().ToString();

                File.WriteAllText(
                    clusterFilePath,
                    dpdtClusterCode
                    );

                #endregion

                #region dryioc related

                var dryiocFileName = "DryIocRelated.cs";
                var dryiocFilePath = Path.Combine(targetDirectory, dryiocFileName);
                var dryiocClassName = "DryIocRelated";

                var dryiocCode = $@"
//seed: {Seed}
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DryIoc;

namespace {nameSpace}
{{
    public static class {dryiocClassName}
    {{
        public const int BindCount = {BindCount};
        public const string BindCountString = ""{BindCount}"";
        public const string GenericTestName = ""DryIoc.{ResolveTypeEnum.Generic}{scope}{BindCount}"";
        public const string NonGenericTestName = ""DryIoc.{ResolveTypeEnum.NonGeneric}{scope}{BindCount}"";

        public static void Bind(Container container)
        {{
#region bind code
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetDryIocBindCode(scope)))};
#endregion
        }}

#region resolution code
        public static void ResolveGeneric(Container container)
        {{
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetDryIocResolutionCode(ResolveTypeEnum.Generic)))};
        }}

        public static void ResolveNonGeneric(Container container)
        {{
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetDryIocResolutionCode(ResolveTypeEnum.NonGeneric)))};
        }}
#endregion
    }}
}}
";

                dryiocCode = SyntaxFactory.ParseCompilationUnit(dryiocCode).NormalizeWhitespace().GetText().ToString();

                File.WriteAllText(
                    dryiocFilePath,
                    dryiocCode
                    );

                #endregion

                #region microresolver related

                var mrFileName = "MicroResolverRelated.cs";
                var mrFilePath = Path.Combine(targetDirectory, mrFileName);
                var mrClassName = "MicroResolverRelated";

                var mrCode = $@"
//seed: {Seed}
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MicroResolver;

namespace {nameSpace}
{{
    public static class {mrClassName}
    {{
        public const int BindCount = {BindCount};
        public const string BindCountString = ""{BindCount}"";
        public const string GenericTestName = ""Microresolver.{ResolveTypeEnum.Generic}{scope}{BindCount}"";
        public const string NonGenericTestName = ""Microresolver.{ResolveTypeEnum.NonGeneric}{scope}{BindCount}"";

        public static void Bind(ObjectResolver container)
        {{
#region bind code
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetMicroresolverBindCode(scope)))};
#endregion
        }}

#region resolution code
        public static void ResolveGeneric(ObjectResolver container)
        {{
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetMicroresolverResolutionCode(ResolveTypeEnum.Generic)))};
        }}

        public static void ResolveNonGeneric(ObjectResolver container)
        {{
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetMicroresolverResolutionCode(ResolveTypeEnum.NonGeneric)))};
        }}
#endregion
    }}
}}
";

                mrCode = SyntaxFactory.ParseCompilationUnit(mrCode).NormalizeWhitespace().GetText().ToString();

                File.WriteAllText(
                    mrFilePath,
                    mrCode
                    );

                #endregion
            }
        }

        private static List<Node> GeneratePlainNodes(
            int seed
            )
        {
            var createdNodes = new List<Node>
            {
                new(null, 0)
            };

            for (var i = 1; i < BindCount; i++)
            {
                var newNode = new Node(null, i);
                createdNodes.Add(newNode);
            }

            return createdNodes;
        }

        private static List<Node> GenerateNodesInColumn(
            int seed
            )
        {
            var createdNodes = new List<Node>
            {
                new(null, 0)
            };

            for (var i = 1; i < BindCount; i++)
            {
                var parent = createdNodes.Last();
                var newNode = new Node(parent, i);

                parent.Children.Add(newNode);
                createdNodes.Add(newNode);
            }

            return createdNodes;
        }


        private static List<Node> GenerateNodesInTree(
            int seed
            )
        {
            var rnd = new Random(
                seed
                );

            var nodeIndex = 0;

            var createdNodes = new List<Node>
            {
                new(null, nodeIndex++)
            };

            for (var i = 1; i < BindCount; i++)
            {
                var index = rnd.Next(createdNodes.Count + 1) - 1;

                if (index < 0)
                {
                    var newNode = new Node(null, nodeIndex++);
                    createdNodes.Add(newNode);
                }
                else
                {
                    var parent = createdNodes[index];
                    var newNode = new Node(parent, nodeIndex++);

                    parent.Children.Add(newNode);
                    createdNodes.Add(newNode);
                }
            }

            return createdNodes;
        }
    }

}
