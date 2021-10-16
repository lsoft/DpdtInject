﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DpdtInject.Generator.Core.Producer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DpdtInject.Injector.Src.Bind.Settings;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Generator.Core.Binding.Settings.Constructor;

namespace DpdtInject.Generator.Core.Binding
{
    [DebuggerDisplay("{BindFromTypes[0].Name} -> {TargetRepresentation}")]
    public class BindingContainerWithInstance : BaseBindingContainer
    {

        public override IReadOnlyList<DetectedMethodArgument> ConstructorArguments
        {
            get;
        }
        
        public override string TargetRepresentation
        {
            get
            {
                return BindToType.ToGlobalDisplayString();
            }
        }


        public BindingContainerWithInstance(
            BindingContainerTypes types,
            IReadOnlyList<DetectedMethodArgument> constructorArguments,
            BindScopeEnum scope,
            ExpressionStatementSyntax expressionNode,
            ArgumentSyntax? whenArgumentClause,
            IReadOnlyList<IDefinedSetting> settings,
            bool isConventional
            ) : base(types, scope, expressionNode, whenArgumentClause, null, settings, isConventional)
        {
            if (constructorArguments is null)
            {
                throw new ArgumentNullException(nameof(constructorArguments));
            }
            
            ConstructorArguments = constructorArguments;
        }
    }
}
