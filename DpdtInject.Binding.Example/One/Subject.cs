using System;
using System.Collections.Generic;
using System.Text;

namespace DpdtInject.Binding.Example.One
{
    public interface IA
    {
        string Message
        {
            get;
        }
    }

    public class A1 : IA
    {
        public string Message
        {
            get;
        }
        public A1(string message)
        {
            Message = message;
        }

        public string GetMessage()
        {
            return Message;
        }
    }

    public class A2 : IA
    {
        public string Message
        {
            get;
        }

        public A2()
        {
            Message = "empty message";
        }

        public string GetMessage()
        {
            return Message;
        }
    }
}
