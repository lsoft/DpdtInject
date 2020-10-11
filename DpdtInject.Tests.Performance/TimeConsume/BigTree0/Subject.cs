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
        public int ChildCount => 4;
        public IInterface1 Argument1
        {
            get;
        }

        public IInterface18 Argument18
        {
            get;
        }

        public IInterface23 Argument23
        {
            get;
        }

        public IInterface39 Argument39
        {
            get;
        }

        public Class0(IInterface1 argument1, IInterface18 argument18, IInterface23 argument23, IInterface39 argument39)
        {
            Argument1 = argument1;
            Argument18 = argument18;
            Argument23 = argument23;
            Argument39 = argument39;
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
        public int ChildCount => 5;
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

        public IInterface20 Argument20
        {
            get;
        }

        public Class1(IInterface2 argument2, IInterface4 argument4, IInterface5 argument5, IInterface6 argument6, IInterface20 argument20)
        {
            Argument2 = argument2;
            Argument4 = argument4;
            Argument5 = argument5;
            Argument6 = argument6;
            Argument20 = argument20;
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
        public int ChildCount => 2;
        public IInterface9 Argument9
        {
            get;
        }

        public IInterface22 Argument22
        {
            get;
        }

        public Class3(IInterface9 argument9, IInterface22 argument22)
        {
            Argument9 = argument9;
            Argument22 = argument22;
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
        public int ChildCount => 2;
        public IInterface12 Argument12
        {
            get;
        }

        public IInterface41 Argument41
        {
            get;
        }

        public Class4(IInterface12 argument12, IInterface41 argument41)
        {
            Argument12 = argument12;
            Argument41 = argument41;
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
        public int ChildCount => 2;
        public IInterface11 Argument11
        {
            get;
        }

        public IInterface43 Argument43
        {
            get;
        }

        public Class5(IInterface11 argument11, IInterface43 argument43)
        {
            Argument11 = argument11;
            Argument43 = argument43;
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
        public int ChildCount => 2;
        public IInterface8 Argument8
        {
            get;
        }

        public IInterface25 Argument25
        {
            get;
        }

        public Class6(IInterface8 argument8, IInterface25 argument25)
        {
            Argument8 = argument8;
            Argument25 = argument25;
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
        public int ChildCount => 1;
        public IInterface10 Argument10
        {
            get;
        }

        public Class7(IInterface10 argument10)
        {
            Argument10 = argument10;
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
        public int ChildCount => 3;
        public IInterface19 Argument19
        {
            get;
        }

        public IInterface35 Argument35
        {
            get;
        }

        public IInterface44 Argument44
        {
            get;
        }

        public Class9(IInterface19 argument19, IInterface35 argument35, IInterface44 argument44)
        {
            Argument19 = argument19;
            Argument35 = argument35;
            Argument44 = argument44;
        }
    }

    public interface IInterface10
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class10 : IInterface10
    {
        public int ChildCount => 0;
        public Class10()
        {
        }
    }

    public interface IInterface11
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class11 : IInterface11
    {
        public int ChildCount => 3;
        public IInterface15 Argument15
        {
            get;
        }

        public IInterface21 Argument21
        {
            get;
        }

        public IInterface38 Argument38
        {
            get;
        }

        public Class11(IInterface15 argument15, IInterface21 argument21, IInterface38 argument38)
        {
            Argument15 = argument15;
            Argument21 = argument21;
            Argument38 = argument38;
        }
    }

    public interface IInterface12
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class12 : IInterface12
    {
        public int ChildCount => 3;
        public IInterface13 Argument13
        {
            get;
        }

        public IInterface27 Argument27
        {
            get;
        }

        public IInterface37 Argument37
        {
            get;
        }

        public Class12(IInterface13 argument13, IInterface27 argument27, IInterface37 argument37)
        {
            Argument13 = argument13;
            Argument27 = argument27;
            Argument37 = argument37;
        }
    }

    public interface IInterface13
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class13 : IInterface13
    {
        public int ChildCount => 1;
        public IInterface16 Argument16
        {
            get;
        }

        public Class13(IInterface16 argument16)
        {
            Argument16 = argument16;
        }
    }

    public interface IInterface14
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class14 : IInterface14
    {
        public int ChildCount => 1;
        public IInterface40 Argument40
        {
            get;
        }

        public Class14(IInterface40 argument40)
        {
            Argument40 = argument40;
        }
    }

    public interface IInterface15
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class15 : IInterface15
    {
        public int ChildCount => 3;
        public IInterface17 Argument17
        {
            get;
        }

        public IInterface31 Argument31
        {
            get;
        }

        public IInterface36 Argument36
        {
            get;
        }

        public Class15(IInterface17 argument17, IInterface31 argument31, IInterface36 argument36)
        {
            Argument17 = argument17;
            Argument31 = argument31;
            Argument36 = argument36;
        }
    }

    public interface IInterface16
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class16 : IInterface16
    {
        public int ChildCount => 1;
        public IInterface30 Argument30
        {
            get;
        }

        public Class16(IInterface30 argument30)
        {
            Argument30 = argument30;
        }
    }

    public interface IInterface17
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class17 : IInterface17
    {
        public int ChildCount => 1;
        public IInterface28 Argument28
        {
            get;
        }

        public Class17(IInterface28 argument28)
        {
            Argument28 = argument28;
        }
    }

    public interface IInterface18
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class18 : IInterface18
    {
        public int ChildCount => 2;
        public IInterface42 Argument42
        {
            get;
        }

        public IInterface46 Argument46
        {
            get;
        }

        public Class18(IInterface42 argument42, IInterface46 argument46)
        {
            Argument42 = argument42;
            Argument46 = argument46;
        }
    }

    public interface IInterface19
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class19 : IInterface19
    {
        public int ChildCount => 1;
        public IInterface33 Argument33
        {
            get;
        }

        public Class19(IInterface33 argument33)
        {
            Argument33 = argument33;
        }
    }

    public interface IInterface20
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class20 : IInterface20
    {
        public int ChildCount => 0;
        public Class20()
        {
        }
    }

    public interface IInterface21
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class21 : IInterface21
    {
        public int ChildCount => 0;
        public Class21()
        {
        }
    }

    public interface IInterface22
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class22 : IInterface22
    {
        public int ChildCount => 0;
        public Class22()
        {
        }
    }

    public interface IInterface23
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class23 : IInterface23
    {
        public int ChildCount => 2;
        public IInterface24 Argument24
        {
            get;
        }

        public IInterface26 Argument26
        {
            get;
        }

        public Class23(IInterface24 argument24, IInterface26 argument26)
        {
            Argument24 = argument24;
            Argument26 = argument26;
        }
    }

    public interface IInterface24
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class24 : IInterface24
    {
        public int ChildCount => 1;
        public IInterface29 Argument29
        {
            get;
        }

        public Class24(IInterface29 argument29)
        {
            Argument29 = argument29;
        }
    }

    public interface IInterface25
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class25 : IInterface25
    {
        public int ChildCount => 1;
        public IInterface34 Argument34
        {
            get;
        }

        public Class25(IInterface34 argument34)
        {
            Argument34 = argument34;
        }
    }

    public interface IInterface26
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class26 : IInterface26
    {
        public int ChildCount => 1;
        public IInterface32 Argument32
        {
            get;
        }

        public Class26(IInterface32 argument32)
        {
            Argument32 = argument32;
        }
    }

    public interface IInterface27
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class27 : IInterface27
    {
        public int ChildCount => 0;
        public Class27()
        {
        }
    }

    public interface IInterface28
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class28 : IInterface28
    {
        public int ChildCount => 0;
        public Class28()
        {
        }
    }

    public interface IInterface29
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class29 : IInterface29
    {
        public int ChildCount => 0;
        public Class29()
        {
        }
    }

    public interface IInterface30
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class30 : IInterface30
    {
        public int ChildCount => 0;
        public Class30()
        {
        }
    }

    public interface IInterface31
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class31 : IInterface31
    {
        public int ChildCount => 0;
        public Class31()
        {
        }
    }

    public interface IInterface32
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class32 : IInterface32
    {
        public int ChildCount => 0;
        public Class32()
        {
        }
    }

    public interface IInterface33
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class33 : IInterface33
    {
        public int ChildCount => 0;
        public Class33()
        {
        }
    }

    public interface IInterface34
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class34 : IInterface34
    {
        public int ChildCount => 0;
        public Class34()
        {
        }
    }

    public interface IInterface35
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class35 : IInterface35
    {
        public int ChildCount => 1;
        public IInterface47 Argument47
        {
            get;
        }

        public Class35(IInterface47 argument47)
        {
            Argument47 = argument47;
        }
    }

    public interface IInterface36
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class36 : IInterface36
    {
        public int ChildCount => 0;
        public Class36()
        {
        }
    }

    public interface IInterface37
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class37 : IInterface37
    {
        public int ChildCount => 0;
        public Class37()
        {
        }
    }

    public interface IInterface38
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class38 : IInterface38
    {
        public int ChildCount => 0;
        public Class38()
        {
        }
    }

    public interface IInterface39
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class39 : IInterface39
    {
        public int ChildCount => 0;
        public Class39()
        {
        }
    }

    public interface IInterface40
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class40 : IInterface40
    {
        public int ChildCount => 0;
        public Class40()
        {
        }
    }

    public interface IInterface41
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class41 : IInterface41
    {
        public int ChildCount => 0;
        public Class41()
        {
        }
    }

    public interface IInterface42
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class42 : IInterface42
    {
        public int ChildCount => 0;
        public Class42()
        {
        }
    }

    public interface IInterface43
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class43 : IInterface43
    {
        public int ChildCount => 1;
        public IInterface45 Argument45
        {
            get;
        }

        public Class43(IInterface45 argument45)
        {
            Argument45 = argument45;
        }
    }

    public interface IInterface44
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class44 : IInterface44
    {
        public int ChildCount => 0;
        public Class44()
        {
        }
    }

    public interface IInterface45
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class45 : IInterface45
    {
        public int ChildCount => 1;
        public IInterface49 Argument49
        {
            get;
        }

        public Class45(IInterface49 argument49)
        {
            Argument49 = argument49;
        }
    }

    public interface IInterface46
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class46 : IInterface46
    {
        public int ChildCount => 0;
        public Class46()
        {
        }
    }

    public interface IInterface47
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class47 : IInterface47
    {
        public int ChildCount => 0;
        public Class47()
        {
        }
    }

    public interface IInterface48
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class48 : IInterface48
    {
        public int ChildCount => 0;
        public Class48()
        {
        }
    }

    public interface IInterface49
    {
        int ChildCount
        {
            get;
        }
    }

    public class Class49 : IInterface49
    {
        public int ChildCount => 0;
        public Class49()
        {
        }
    }

    ;
}