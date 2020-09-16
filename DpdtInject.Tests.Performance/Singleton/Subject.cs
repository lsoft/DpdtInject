using System;
using System.Collections.Generic;
using System.Text;

namespace DpdtInject.Tests.Performance.Singleton
{
    public interface IA
    {
        string Message
        {
            get;
        }
    }

    public class A : IA
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

        //public string GetMessage()
        //{
        //    return Message;
        //}
    }

    public interface IB
    {
        IA A
        {
            get;
        }
    }

    public class B : IB
    {
        public IA A
        {
            get;
        }

        public B(IA a)
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
