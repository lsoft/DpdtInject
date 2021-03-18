using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Generator.Core;
using DpdtInject.Generator.Core.BindExtractor;
using DpdtInject.Generator.Core.Helpers;
using DpdtInject.Generator.Core.TypeInfo;
using DpdtInject.Generator.Core.TypeScanner;
using DpdtInject.Injector.Helper;
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
