//seed: -624853138
using System;
using System.Collections.Generic;
using System.Text;

namespace DpdtInject.Tests.Performance.TimeConsume.BigTree0
{
    public interface IInterface0
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class0 : IInterface0
    {
        public int ChildCount => 1;
        public IInterface1 Argument1
        {
            get;
        }

        public Class0(IInterface1 argument1)
        {
            Argument1 = argument1;
        }
    }

    public interface IInterface1
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class1 : IInterface1
    {
        public int ChildCount => 4;
        public IInterface2 Argument2
        {
            get;
        }

        public IInterface4 Argument4
        {
            get;
        }

        public IInterface5 Argument5
        {
            get;
        }

        public IInterface6 Argument6
        {
            get;
        }

        public Class1(IInterface2 argument2, IInterface4 argument4, IInterface5 argument5, IInterface6 argument6)
        {
            Argument2 = argument2;
            Argument4 = argument4;
            Argument5 = argument5;
            Argument6 = argument6;
        }
    }

    public interface IInterface2
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class2 : IInterface2
    {
        public int ChildCount => 1;
        public IInterface7 Argument7
        {
            get;
        }

        public Class2(IInterface7 argument7)
        {
            Argument7 = argument7;
        }
    }

    public interface IInterface3
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class3 : IInterface3
    {
        public int ChildCount => 1;
        public IInterface9 Argument9
        {
            get;
        }

        public Class3(IInterface9 argument9)
        {
            Argument9 = argument9;
        }
    }

    public interface IInterface4
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class4 : IInterface4
    {
        public int ChildCount => 0;
        public Class4()
        {
        }
    }

    public interface IInterface5
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class5 : IInterface5
    {
        public int ChildCount => 0;
        public Class5()
        {
        }
    }

    public interface IInterface6
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class6 : IInterface6
    {
        public int ChildCount => 1;
        public IInterface8 Argument8
        {
            get;
        }

        public Class6(IInterface8 argument8)
        {
            Argument8 = argument8;
        }
    }

    public interface IInterface7
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class7 : IInterface7
    {
        public int ChildCount => 0;
        public Class7()
        {
        }
    }

    public interface IInterface8
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class8 : IInterface8
    {
        public int ChildCount => 0;
        public Class8()
        {
        }
    }

    public interface IInterface9
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class9 : IInterface9
    {
        public int ChildCount => 0;
        public Class9()
        {
        }
    }

    ;
}