using DpdtInject.Injector.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests
{
    public class FakeModule<T> : DpdtModule
        where T : DpdtModule
    {
        public T1 Get<T1>()
        {
            throw new NotImplementedException("This method should not be executed");
        }


        public List<T1> GetAll<T1>()
        {
            throw new NotImplementedException("This method should not be executed");
        }

        public override void Load()
        {
            throw new NotImplementedException("This method should not be executed");
        }
    }
}
