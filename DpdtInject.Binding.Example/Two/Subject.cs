using System;
using System.Collections.Generic;
using System.Text;

namespace DpdtInject.Binding.Example.Two
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

        //public A()
        //{
        //    Message = "no message";
        //}

        public A(string message)
        {
            Message = message;
        }

        public string GetMessage()
        {
            return Message;
        }
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

}
