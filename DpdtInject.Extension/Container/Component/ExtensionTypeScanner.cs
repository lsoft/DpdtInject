using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Generator;
using DpdtInject.Generator.BindExtractor;
using DpdtInject.Generator.Helpers;
using DpdtInject.Generator.TypeInfo;
using DpdtInject.Generator.TypeScanner;
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
