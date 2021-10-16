﻿using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Generator.Core.BindExtractor;
using DpdtInject.Generator.Core.Binding;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.Producer.ClassProducer.Product;
using DpdtInject.Generator.Core.Producer.Product;
using Microsoft.CodeAnalysis;
using DpdtInject.Injector.Src.Excp;
using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Generator.Core.Producer.ClassProducer
{
    internal class FactoryProducer : IClassProducer
    {
        private readonly ConstructorArgumentDetector _constructorArgumentDetector;

        private readonly List<DetectedMethodArgument> _unknowns = new();
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
                BindConstructorChooser.Instance
                );
        }

        public IProducedClassProduct GenerateProduct(
            )
        {
            var methodProducts = ScanForMethodsToImplement();

            var result = new FactoryClassProduct(
                _types.BindFromTypes[0],
                _types.BindToType,
                methodProducts,
                _unknowns
                );

            return result;
        }

        private List<IMethodProduct> ScanForMethodsToImplement(
            )
        {
            var result = new List<IMethodProduct>();
            var declaredMethods = _types.BindFromTypes[0].GetMembers().FindAll(m => m.Kind == SymbolKind.Method);

            foreach (IMethodSymbol declaredMethod in declaredMethods)
            {
                var implementedMethod = _types.BindToType.FindImplementationForInterfaceMember(declaredMethod);
                if(!(implementedMethod is null))
                {
                    continue;
                }

                //method is not implemented in the proto class
                if(!_factoryPayloadType.CanBeCastedTo(declaredMethod.ReturnType))
                {
                    //return type is not a factory payload
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.IncorrectBinding_IncorrectReturnType,
                        $"Factory [{_types.BindToType.ToGlobalDisplayString()}] contains a method [{declaredMethod.Name}] with return type [{declaredMethod.ReturnType.ToGlobalDisplayString()}] that cannot be produced by casting a payload [{_factoryPayloadType.ToGlobalDisplayString()}]",
                        _types.BindToType.ToGlobalDisplayString()
                        );
                }

                var declaredMethodProduct = GetMethodProduct(declaredMethod);

                result.Add(declaredMethodProduct);
            }

            return result;
        }

        private IMethodProduct GetMethodProduct(
            IMethodSymbol methodSymbol
            )
        {
            if (methodSymbol is null)
            {
                throw new ArgumentNullException(nameof(methodSymbol));
            }

            var extractor = new MethodArgumentExtractor();
            var constructorArguments = extractor.GetMethodArguments(methodSymbol);

            var appended = _constructorArgumentDetector.ChooseConstructorAndAppendUnknownArguments(
                (INamedTypeSymbol)_factoryPayloadType,
                null,
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
                        $"Cannot instanciate factory [{_types.BindToType.ToGlobalDisplayString()}] because of 2 or more constructors contains same parameter name [{constructorArgument.Name}] with different types",
                        _types.BindToType.ToGlobalDisplayString()
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

            return MethodProductFactory.Create(
                methodSymbol.Name,
                new TypeMethodResult(methodSymbol.ReturnType),
                (methodName, returnType) =>
                {
                    return $@"
        public {returnType} {methodName}({usedConstructorArguments.Join(cafm => cafm.GetDeclarationSyntax(), ",")})
        {{
            return new {_factoryPayloadType.ToGlobalDisplayString()}(
                {caCombined.Join(cac => cac, ",")}
                );
        }}
";
                }
                );
        }
    }

}
