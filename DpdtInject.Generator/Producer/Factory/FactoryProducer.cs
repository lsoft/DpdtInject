using DpdtInject.Generator.BindExtractor;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Parser.Binding;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Producer.Factory
{
    internal class FactoryProducer
    {
        private readonly ITypeInfoProvider _typeInfoProvider;
        private readonly ConstructorArgumentDetector _constructorArgumentDetector;

        private readonly List<DetectedConstructorArgument> _unknowns = new List<DetectedConstructorArgument>();

        public ClusterBindings ClusterBindings
        {
            get;
        }


        public FactoryProducer(
            ITypeInfoProvider typeInfoProvider,
            ClusterBindings clusterBindings
            )
        {
            if (typeInfoProvider is null)
            {
                throw new ArgumentNullException(nameof(typeInfoProvider));
            }

            if (clusterBindings is null)
            {
                throw new ArgumentNullException(nameof(clusterBindings));
            }

            _typeInfoProvider = typeInfoProvider;
            ClusterBindings = clusterBindings;

            _constructorArgumentDetector = new ConstructorArgumentDetector(
                new BindConstructorChooser(
                    )
                );

        }

        internal IEnumerable<FactoryProduct> Produce()
        {
            foreach(var bindingContainer in ClusterBindings.BindingContainers)
            {
                if (!bindingContainer.ToFactory)
                {
                    continue;
                }

                var factoryProduct = GenerateFactoryBody(bindingContainer);
                yield return factoryProduct;
            }
        }

        private FactoryProduct GenerateFactoryBody(
            IBindingContainer bindingContainer
            )
        {
            var methodProducts = ScanForMethodsToImplement(bindingContainer);

            var result = new FactoryProduct(
                bindingContainer.BindToType,
                $@"
namespace {bindingContainer.BindToType.ContainingNamespace.ToDisplayString()}
{{
    public partial class {bindingContainer.BindToType.Name} : {bindingContainer.BindFromTypes[0].ToDisplayString()}
    {{
        {_unknowns.Join(u => $"private readonly {u.Type!.ToDisplayString()} {GetStableArgumentName(u)};")}

        public {bindingContainer.BindToType.Name}(
            {_unknowns.Join(u => $"{u.Type!.ToDisplayString()} {GetStableArgumentName(u)}", ",")}
            )
        {{
            {_unknowns.Join(u => $"this.{GetStableArgumentName(u)} = {GetStableArgumentName(u)};")}
        }}

        {methodProducts.Join(mp => mp.MethodBody)}
    }}
}}
");
            return result;
        }

        private List<MethodProduct> ScanForMethodsToImplement(
            IBindingContainer bindingContainer
            )
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            var result = new List<MethodProduct>();
            var declaredMethods = bindingContainer.BindFromTypes[0].GetMembers().FindAll(m => m.Kind == SymbolKind.Method);

            foreach (IMethodSymbol declaredMethod in declaredMethods)
            {
                var implementedeMethod = bindingContainer.BindToType.FindImplementationForInterfaceMember(declaredMethod);
                if(!(implementedeMethod is null))
                {
                    continue;
                }

                //method is not implemented in the proto class
                if(!bindingContainer.FactoryPayloadType!.CanBeCastedTo(declaredMethod.ReturnType.ToDisplayString()))
                {
                    //return type is not a factory payload
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectReturnType,
                        $"Factory [{bindingContainer.BindToType.ToDisplayString()}] contains a method [{declaredMethod.Name}] with return type [{declaredMethod.ReturnType.ToDisplayString()}] that cannot be produced by casting a payload [{bindingContainer.FactoryPayloadType!.ToDisplayString()}]",
                        bindingContainer.BindToType.ToDisplayString()
                        );
                }

                var declaredMethodProduct = GetMethodProduct(bindingContainer, declaredMethod);

                result.Add(declaredMethodProduct);
            }

            return result;
        }

        public MethodProduct GetMethodProduct(
            IBindingContainer bindingContainer,
            IMethodSymbol methodSymbol
            )
        {
            if (bindingContainer is null)
            {
                throw new ArgumentNullException(nameof(bindingContainer));
            }

            if (methodSymbol is null)
            {
                throw new ArgumentNullException(nameof(methodSymbol));
            }

            var extractor = new ConstructorArgumentFromMethodExtractor();
            var constructorArguments = extractor.GetConstructorArguments(methodSymbol);

            var appended = _constructorArgumentDetector.AppendUnknown(
                (INamedTypeSymbol)bindingContainer.FactoryPayloadType!,
                ref constructorArguments
                );

            for(var i = constructorArguments.Count - appended; i < constructorArguments.Count; i++)
            {
                var constructorArgument = constructorArguments[i];

                if (_unknowns.Any(u => SymbolEqualityComparer.Default.Equals(u.Type, constructorArgument.Type)))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.CannotBuildFactory,
                        $"Cannot instanciate factory [{bindingContainer.BindToType.ToDisplayString()}] because of 2 or more constructors contains same type [{constructorArgument.Type!.ToDisplayString()}]",
                        bindingContainer.BindToType.ToDisplayString()
                        );
                }

                _unknowns.Add(constructorArgument);
            }

            var caCombined = new List<string>();
            caCombined.AddRange(
                constructorArguments
                    .Take(constructorArguments.Count - appended)
                    .Select(cafm => $"{cafm.Name}: {cafm.Name}")
                    );
            caCombined.AddRange(
                constructorArguments
                    .Skip(constructorArguments.Count - appended)
                    .Select(cau => $"{cau.Name}: {GetStableArgumentName(cau)}")
                );

            return new MethodProduct(
                methodSymbol.Name,
                methodSymbol.ReturnType,
                (methodName, returnType) =>
                {
                    return $@"
public {returnType.ToDisplayString()} {methodName}({constructorArguments.Join(cafm => cafm.GetDeclarationSyntax(), ",")})
{{
    return new {bindingContainer.FactoryPayloadType!.ToDisplayString()}(
        {caCombined.Join(cac => cac, ",")}
        );
}}
";
                }
                );
        }


        private static string GetStableArgumentName(DetectedConstructorArgument dca)
        {
            return $"{dca.Name}{dca.GetHashCode()}";
        }

    }

}
