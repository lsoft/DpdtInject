﻿using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.BindExtractor;
using DpdtInject.Generator.Binding;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.Producer.Product;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Generator.Producer.ClassProducer
{
    internal class FactoryProducer : IClassProducer
    {
        private readonly ConstructorArgumentDetector _constructorArgumentDetector;

        private readonly List<DetectedConstructorArgument> _unknowns = new();
        private readonly BindingContainerTypes _types;
        private readonly ITypeSymbol _factoryPayloadType;

        public FactoryProducer(
            BindingContainerTypes types,
            ITypeSymbol factoryPayloadType
            )
        {
            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (factoryPayloadType is null)
            {
                throw new ArgumentNullException(nameof(factoryPayloadType));
            }

            _types = types;
            _factoryPayloadType = factoryPayloadType;

            _constructorArgumentDetector = new ConstructorArgumentDetector(
                new BindConstructorChooser(
                    )
                );
        }

        public ProducedClassProduct GenerateProduct(
            )
        {
            var methodProducts = ScanForMethodsToImplement();

            var result = new ProducedClassProduct(
                _types.BindToType,
                $@"
namespace {_types.BindToType.ContainingNamespace.ToDisplayString()}
{{
    public partial class {_types.BindToType.Name} : {_types.BindFromTypes[0].ToDisplayString()}
    {{
        {_unknowns.Join(u => $"private readonly {u.Type!.ToDisplayString()} {u.Name};")}

        public {_types.BindToType.Name}(
            {_unknowns.Join(u => $"{u.Type!.ToDisplayString()} {u.Name}", ",")}
            )
        {{
            {_unknowns.Join(u => $"this.{u.Name} = {u.Name};")}
        }}

        {methodProducts.Join(mp => mp.MethodBody)}
    }}
}}
");
            return result;
        }

        private List<MethodProduct> ScanForMethodsToImplement(
            )
        {
            var result = new List<MethodProduct>();
            var declaredMethods = _types.BindFromTypes[0].GetMembers().FindAll(m => m.Kind == SymbolKind.Method);

            foreach (IMethodSymbol declaredMethod in declaredMethods)
            {
                var implementedMethod = _types.BindToType.FindImplementationForInterfaceMember(declaredMethod);
                if(!(implementedMethod is null))
                {
                    continue;
                }

                //method is not implemented in the proto class
                if(!_factoryPayloadType.CanBeCastedTo(declaredMethod.ReturnType.ToDisplayString()))
                {
                    //return type is not a factory payload
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectReturnType,
                        $"Factory [{_types.BindToType.ToDisplayString()}] contains a method [{declaredMethod.Name}] with return type [{declaredMethod.ReturnType.ToDisplayString()}] that cannot be produced by casting a payload [{_factoryPayloadType.ToDisplayString()}]",
                        _types.BindToType.ToDisplayString()
                        );
                }

                var declaredMethodProduct = GetMethodProduct(declaredMethod);

                result.Add(declaredMethodProduct);
            }

            return result;
        }

        private MethodProduct GetMethodProduct(
            IMethodSymbol methodSymbol
            )
        {
            if (methodSymbol is null)
            {
                throw new ArgumentNullException(nameof(methodSymbol));
            }

            var extractor = new ConstructorArgumentFromMethodExtractor();
            var constructorArguments = extractor.GetConstructorArguments(methodSymbol);

            var appended = _constructorArgumentDetector.AppendUnknown(
                (INamedTypeSymbol)_factoryPayloadType,
                ref constructorArguments
                );

            for(var i = constructorArguments.Count - appended; i < constructorArguments.Count; i++)
            {
                var constructorArgument = constructorArguments[i];

                if (_unknowns.Any(u => u.Name == constructorArgument.Name && SymbolEqualityComparer.Default.Equals(u.Type, constructorArgument.Type)))
                {
                    //this unknown has been stored already, skip it
                    continue;
                }

                if (_unknowns.Any(u => u.Name == constructorArgument.Name && !SymbolEqualityComparer.Default.Equals(u.Type, constructorArgument.Type)))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.CannotBuildFactory,
                        $"Cannot instanciate factory [{_types.BindToType.ToDisplayString()}] because of 2 or more constructors contains same parameter name [{constructorArgument.Name}] with different types",
                        _types.BindToType.ToDisplayString()
                        );
                }

                _unknowns.Add(constructorArgument);
            }

            var usedConstructorArguments =
                constructorArguments
                    .Take(constructorArguments.Count - appended)
                    .ToList()
                    ;
            var unknownConstructorArguments =
                constructorArguments
                    .Skip(constructorArguments.Count - appended)
                    .ToList()
                    ;

            var caCombined = new List<string>();
            caCombined.AddRange(usedConstructorArguments.Select(cafm => $"{cafm.Name}: {cafm.GetUsageSyntax()}"));
            caCombined.AddRange(unknownConstructorArguments.Select(cau => $"{cau.Name}: this.{cau.GetUsageSyntax()}"));

            return new MethodProduct(
                methodSymbol.Name,
                methodSymbol.ReturnType,
                (methodName, returnType) =>
                {
                    return $@"
public {returnType.ToDisplayString()} {methodName}({usedConstructorArguments.Join(cafm => cafm.GetDeclarationSyntax(), ",")})
{{
    return new {_factoryPayloadType.ToDisplayString()}(
        {caCombined.Join(cac => cac, ",")}
        );
}}
";
                }
                );
        }
    }

}
