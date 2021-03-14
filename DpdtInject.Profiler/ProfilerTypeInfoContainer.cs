using DpdtInject.Generator;
using DpdtInject.Generator.TypeInfo;
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

        public override void AddSources(ModificationDescription[] modificationDescriptions)
        {
            //nothing to do
        }
    }
}
