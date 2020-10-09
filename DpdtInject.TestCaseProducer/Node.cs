using System;
using System.Collections.Generic;

namespace DpdtInject.TestCaseProducer
{
    public class Node
    {
        public Node? Parent
        {
            get;
        }
        public int Index 
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

        public string ResolvedInstanceName
        {
            get;
        }

        public Node(Node? parent, int index)
        {
            Parent = parent;
            Index = index;
            InterfaceName = $"IInterface{index}";
            ClassName = $"Class{index}";
            PropertyName = $"Argument{index}";
            ArgumentName = $"argument{index}";
            Children = new List<Node>();
            ResolvedInstanceName = $"resolvedInstance{Index}";
        }

        public string GetDpdtResolutionCode(BindResolveTypeEnum type)
        {
            switch (type)
            {
                case BindResolveTypeEnum.GenericSingleton:
                case BindResolveTypeEnum.GenericTransient:
                    return $@"
var {ResolvedInstanceName} = cluster.Get<{InterfaceName}>();
";
                case BindResolveTypeEnum.NonGenericSingleton:
                    return $@"
var {ResolvedInstanceName} = cluster.Get(typeof({InterfaceName}));
";
                default:
                    throw new ArgumentOutOfRangeException(type.ToString());
            }
        }

        public string GetDryIocResolutionCode(BindResolveTypeEnum type)
        {
            switch (type)
            {
                case BindResolveTypeEnum.GenericSingleton:
                case BindResolveTypeEnum.GenericTransient:
                    return $@"
var {ResolvedInstanceName} = container.Resolve<{InterfaceName}>();
";
                case BindResolveTypeEnum.NonGenericSingleton:
                    return $@"
var {ResolvedInstanceName} = container.Resolve(typeof({InterfaceName}));
";
                default:
                    throw new ArgumentOutOfRangeException(type.ToString());
            }
        }

        public string GetMicroresolverResolutionCode(BindResolveTypeEnum type)
        {
            switch (type)
            {
                case BindResolveTypeEnum.GenericSingleton:
                case BindResolveTypeEnum.GenericTransient:
                    return $@"
var {ResolvedInstanceName} = container.Resolve<{InterfaceName}>();
";
                case BindResolveTypeEnum.NonGenericSingleton:
                    return $@"
var {ResolvedInstanceName} = container.Resolve(typeof({InterfaceName}));
";
                default:
                    throw new ArgumentOutOfRangeException(type.ToString());
            }
        }




        public string GetDpdtBindCode(BindResolveTypeEnum type)
        {
            var suffix = GetSuffix(type);

            return $@"
Bind<{InterfaceName}>()
    .To<{ClassName}>()
    .With{suffix}Scope()
    ;
";
        }

        public string GetDryIocBindCode(BindResolveTypeEnum type)
        {
            var suffix = GetSuffix(type);


            return $@"
container.Register<{InterfaceName}, {ClassName}>(Reuse.{suffix});
";
        }

        public string GetMicroresolverBindCode(BindResolveTypeEnum type)
        {
            var suffix = GetSuffix(type);

            return $@"
container.Register<{InterfaceName}, {ClassName}>(Lifestyle.{suffix});
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
    int ChildCount {{ get; }}
}}

public class {ClassName} : {InterfaceName}
{{
    public int ChildCount => {arguments.Count};


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


        private static string GetSuffix(BindResolveTypeEnum type)
        {
            var suffix = "";
            switch (type)
            {
                case BindResolveTypeEnum.GenericTransient:
                    suffix = "Transient";
                    break;
                case BindResolveTypeEnum.GenericSingleton:
                case BindResolveTypeEnum.NonGenericSingleton:
                    suffix = "Singleton";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(type.ToString());
            }

            return suffix;
        }

    }

}
