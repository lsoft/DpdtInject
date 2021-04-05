using DpdtInject.Generator.Core;
using DpdtInject.Generator.Core.TypeInfo;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Profiler
{
    public class ProfilerTypeInfoContainer : TypeInfoContainer
    {
        public ProfilerTypeInfoContainer(
            Compilation compilation
            ) : base(compilation)
        {

        }

        protected override void AddSourcesInternal(ModificationDescription[] modificationDescriptions)
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
