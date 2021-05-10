using DpdtInject.Extension.Shared.Dto;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DpdtInject.Extension.Machinery.Add;
using DpdtInject.Generator.Core.Binding.Xml;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;

namespace DpdtInject.Extension.UI.ChainStep
{
    public class ChoosedParameters
    {
        public CodeLensTarget Target
        {
            get;
        }

        public INamedTypeSymbol? TargetClass
        {
            get;
            internal set;
        }

        public IMethodSymbol? ChoosedConstructor
        {
            get;
            internal set;
        }

        public List<IParameterSymbol>? ChoosedConstructorArguments
        {
            get;
            internal set;
        }

        public List<INamedTypeSymbol>? ChoosedBindsFrom
        {
            get;
            internal set;
        }

        public IMethodBindContainer? ChoosedTargetMethod
        {
            get;
            internal set;
        }

        public BindScopeEnum Scope
        {
            get;
            internal set;
        } = BindScopeEnum.Singleton;

        public bool IsConditionalBinding
        {
            get;
            internal set;
        }
        public ConstructorSettingsEnum ConstructorSetting
        {
            get;
            internal set;
        }

        public ChoosedParameters(
            CodeLensTarget target
            )
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Target = target;
            ChoosedConstructorArguments = new List<IParameterSymbol>();
        }


        public async Task InsertBindingAsync()
        {
            var newBindingInfo = new NewBindingInfo(
                ChoosedBindsFrom!,
                TargetClass!,
                ChoosedConstructor!,
                ChoosedConstructorArguments!,
                ConstructorSetting,
                Scope,
                IsConditionalBinding
                );

            var dm = new DocumentModifier(
                ChoosedTargetMethod!
                );

            await dm.DoSurgeryAsync(
                newBindingInfo
                );
        }

    }
}
