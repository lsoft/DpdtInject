using DpdtInject.Generator.Core;
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
        protected override void AddSourcesInternal(
            ModificationDescription[] modificationDescriptions
            )
        {
            //nothing to do
        }

        /// <inheritdoc />
        protected override void AddAdditionalFileInternal(
            string xmlBody
            )
        {
            //nothing to do
        }
    }
}
