using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DpdtInject.TestCaseProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            var nodeIndex = 0;

            var createdNodes = new List<Node>
            {
                new Node(null, nodeIndex++)
            };

            var rnd = new Random(
                BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)
                );

            for (var i = 1; i < 1000; i++)
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


            //generate source code
            var subjectCodeBuilder = new StringBuilder();
            var bindCodeBuilder = new StringBuilder();
            var resolutionCodeBuilder = new StringBuilder();
            foreach (var node in createdNodes)
            {
                subjectCodeBuilder.AppendLine(node.GetSubjectCode());
                bindCodeBuilder.AppendLine(node.GetBindCode());
                resolutionCodeBuilder.AppendLine(node.GetResolutionCode());
            }

            var relativePath = @"DpdtInject.Tests/TimeConsume/BigTree0/TimeConsumeBigTree0Module.cs";
            var nameSpace = "DpdtInject.Tests.TimeConsume.BigTree0";
            var moduleClassName = "TimeConsumeBigTree0Module";
            var moduleTesterClassName = $"{moduleClassName}Tester";


            var sourceCode = $@"
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace {nameSpace}
{{
    public partial class {moduleClassName} : DpdtModule
    {{
        public override void Load()
        {{
            {bindCodeBuilder.ToString()}
        }}

        public partial class DefaultCluster
        {{
        }}

        public class {moduleTesterClassName}
        {{
            public void PerformModuleTesting()
            {{
                var module = new FakeModule<{moduleClassName}>();
/*
                {resolutionCodeBuilder.ToString()}
//*/
            }}
        }}

    }}

    {subjectCodeBuilder.ToString()}
}}
";

            sourceCode = SyntaxFactory.ParseCompilationUnit(sourceCode).NormalizeWhitespace().GetText().ToString();

            File.WriteAllText(
                @$"../../../../{relativePath}",
                sourceCode
                );

            //Console.WriteLine(sourceCode);
        }
    }

    public static class RndHelper
    {
        public static T GetRandom<T>(
            this Random rnd,
            IReadOnlyList<T> list
            )
        {
            return list[rnd.Next(list.Count)];
        }
    }

    public class Node
    {
        public Node? Parent
        {
            get;
        }

        public List<Node> Children
        {
            get;
        }

        public string InterfaceName
        {
            get;
        }

        public string ClassName
        {
            get;
        }

        public string PropertyName
        {
            get;
        }

        public string ArgumentName
        {
            get;
        }

        public Node(Node? parent, int index)
        {
            Parent = parent;
            InterfaceName = $"IInterface{index}";
            ClassName = $"Class{index}";
            PropertyName = $"Argument{index}";
            ArgumentName = $"argument{index}";
            Children = new List<Node>();
        }

        public string GetResolutionCode()
        {
            return $@"
{{
    var resolvedInstance = module.Get<{InterfaceName}>();
    Assert.IsNotNull(resolvedInstance);
}}
";
        }

        public string GetBindCode()
        {
            return $@"
Bind<{InterfaceName}>()
    .To<{ClassName}>()
    .WithSingletonScope()
    .InCluster<DefaultCluster>()
    ;
";
        }


        public string GetSubjectCode()
        {
            var properties = new List<string>();
            foreach (var child in Children)
            {
                properties.Add(
                    $"public {child.InterfaceName} {child.PropertyName} {{ get; }}"
                    );
            }

            var arguments = new List<string>();
            foreach(var child in Children)
            {
                arguments.Add(
                    $"{child.InterfaceName} {child.ArgumentName}"
                    );
            }

            var assigns = new List<string>();
            foreach (var child in Children)
            {
                assigns.Add(
                    $"{child.PropertyName} = {child.ArgumentName};"
                    );
            }

            return $@"
public interface {InterfaceName}
{{
}}

public class {ClassName} : {InterfaceName}
{{
    {string.Join(Environment.NewLine, properties)}

    public {ClassName}(
        {string.Join(",", arguments)}
        )
    {{
        {string.Join(Environment.NewLine, assigns)}
    }}
}}
";
        }
    }

}
