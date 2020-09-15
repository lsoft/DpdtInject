using System;
using System.Collections.Generic;
using System.Text;

namespace DpdtInject.Binding.Example.Zero
{
    public interface IA2
    {
        string GetMessage();
    }

    public interface IA1
    {
        string Message
        {
            get;
        }
    }

    public class A : IA1, IA2
    {
        public string Message
        {
            get;
        }

        public A()
        {
            Message = "no message";
        }

        //public A(string message)
        //{
        //    Message = message;
        //}

        public string GetMessage()
        {
            return Message;
        }
    }

    public interface IB
    {
        IA1 A
        {
            get;
        }
    }

    public class B : IB
    {
        public IA1 A
        {
            get;
        }

        public B(IA1 a)
        {
            A = a;
        }
    }

    public interface IC
    {
        IB B
        {
            get;
        }

    }

    public class C : IC
    {
        public IB B
        {
            get;
        }

        public C(IB b)
        {
            B = b;
        }

    }
}
