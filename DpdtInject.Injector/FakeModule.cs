using DpdtInject.Injector.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector
{
    public class FakeModule  : IDisposable
    {
        public bool IsRegisteredFrom<T>()
        {
            throw new NotImplementedException("This method should not be executed");
        }

        public object Get(Type requestedType)
        {
            throw new NotImplementedException("This method should not be executed");
        }

        public IEnumerable<object> GetAll(Type requestedType)
        {
            throw new NotImplementedException("This method should not be executed");
        }

        public T1 Get<T1>()
        {
            throw new NotImplementedException("This method should not be executed");
        }


        public List<T1> GetAll<T1>()
        {
            throw new NotImplementedException("This method should not be executed");
        }

        public void Dispose()
        {
            throw new NotImplementedException("This method should not be executed");
        }
    }

    public class FakeModule<T>  : IDisposable
        where T : DpdtModule
    {
        public bool IsRegisteredFrom<T>()
        {
            throw new NotImplementedException("This method should not be executed");
        }


        public object Get(Type requestedType)
        {
            throw new NotImplementedException("This method should not be executed");
        }

        public IEnumerable<object> GetAll(Type requestedType)
        {
            throw new NotImplementedException("This method should not be executed");
        }

        public T1 Get<T1>()
        {
            throw new NotImplementedException("This method should not be executed");
        }


        public List<T1> GetAll<T1>()
        {
            throw new NotImplementedException("This method should not be executed");
        }

        public void Dispose()
        {
            throw new NotImplementedException("This method should not be executed");
        }
    }
}
