using DpdtInject.Injector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Core.Producer
{
    public interface IWritable
    {
        void Write(IndentedTextWriter2 writer, ShortTypeNameGenerator sng);
    }

}
