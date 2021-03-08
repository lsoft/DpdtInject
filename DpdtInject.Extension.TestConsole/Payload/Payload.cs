using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Extension.TestConsole.Payload
{
    public class BindPayload0
    {

    }

    public class BindPayload1
    {
        //
    }

    public class NaivePayload
    {
        
    }

    public class NonGenericClass : Dictionary<string, long>
    {
        public NonGenericClass(
            KeyValuePair<int, uint> a
            )
        {

        }
    }

    public class GenericClass<T> : Dictionary<string, long>
    {
        public GenericClass(
            KeyValuePair<int, uint> a,
            T b
            )
        {

        }
    }

    namespace NewNamespace1
    {
        public interface ISomeInterface0
        {
        }
        public interface ISomeInterface1
        {
        }
        public interface ISomeInterface2 : ISomeInterface1, ISomeInterface0
        {
        }

        public class TwoBaseClass
        {
        }

        namespace NewNamespace2
        {

            public class TwoConstructorPayload : TwoBaseClass, ISomeInterface2
            {
                public TwoConstructorPayload()
                {
                }

                internal TwoConstructorPayload(
                    int a,
                    string b
                    )
                {
                }

                protected TwoConstructorPayload(
                    string b
                    )
                {
                }
            }
        }
    }

}
