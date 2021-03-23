﻿using DpdtInject.Generator.Core;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Extension.Container.Component
{
    public class ExtensionTypeInfoContainer : TypeInfoContainer
    {
        /// <inheritdoc />
        public ExtensionTypeInfoContainer(
            Compilation compilation
            )
            : base(compilation)
        {
        }

        /// <inheritdoc />
        public override void AddSources(
            ModificationDescription[] modificationDescriptions
            )
        {
            //nothing to do
        }
    }
}
