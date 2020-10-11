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
        public const int BindCount = 50;
        public const ScopeTypeEnum Scope = ScopeTypeEnum.Singleton;
        public const ResolveTypeEnum ResolveType = ResolveTypeEnum.Fast;

        public static int Seed =
            //BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
            -624853138;

        static void Main(string[] args)
        {
            var createdNodes =
                //GeneratePlainNodes(Seed);
                GenerateNodesInTree(Seed);

            var targetDirectory = @"../../../../DpdtInject.Tests.Performance/TimeConsume/BigTree0";
            var nameSpace = "DpdtInject.Tests.Performance.TimeConsume.BigTree0";

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
        public const string TestPrefix = ""{ResolveType}{Scope}{BindCount}"";

        public override void Load()
        {{
#region bind code
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetDpdtBindCode(Scope)))};
#endregion
        }}

        public static void ResolveDpdt({clusterClassName} cluster)
        {{
#region resolution code
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetDpdtResolutionCode(ResolveType)))};
#endregion
        }}

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
        public const string TestPrefix = ""{ResolveType}{Scope}{BindCount}"";

        public static void Bind(Container container)
        {{
#region bind code
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetDryIocBindCode(Scope)))};
#endregion
        }}

        public static void Resolve(Container container)
        {{
#region resolution code
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetDryIocResolutionCode(ResolveType)))};
#endregion
        }}

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
        public const string TestPrefix = ""{ResolveType}{Scope}{BindCount}"";

        public static void Bind(ObjectResolver container)
        {{
#region bind code
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetMicroresolverBindCode(Scope)))};
#endregion
        }}

        public static void Resolve(ObjectResolver container)
        {{
#region resolution code
            {string.Join(Environment.NewLine, createdNodes.Select(cn => cn.GetMicroresolverResolutionCode(ResolveType)))};
#endregion
        }}

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

        private static List<Node> GeneratePlainNodes(
            int seed
            )
        {
            var rnd = new Random(
                seed
                );

            var createdNodes = new List<Node>
            {
                new Node(null, 0)
            };

            for (var i = 1; i < BindCount; i++)
            {
                var newNode = new Node(null, i);
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
                new Node(null, nodeIndex++)
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
