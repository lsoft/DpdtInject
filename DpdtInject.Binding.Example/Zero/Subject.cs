using System;
using System.Collections.Generic;
using System.Text;

namespace DpdtInject.Binding.Example.Zero
{

    public interface IA
    {
    }

    public class A : IA
    {
    }

    public interface IB
    {
        string Message { get; }

        IA A { get; }
    }

    public class B : IB
    {
        public string Message { get; }
        public IA A { get; }

        public B(string message, IA a)
        {
            Message = message;
            A = a;
        }
    }

    public interface IC
    {
        IB B { get; }
    }


    public class C : IC
    {
        public IB B { get; }

        public C(IB b)
        {
            B = b;
        }
    }
}
