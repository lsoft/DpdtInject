using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Excp;
using DpdtInject.Injector.Src;
using System;

namespace DpdtInject.Tests.DefaultValue.Custom
{
    public partial class DefaultValueCustom_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<Int_Const>()
                .To<Int_Const>()
                .WithCustomScope()
                ;
            Bind<Int_Default>()
                .To<Int_Default>()
                .WithCustomScope()
                ;



            Bind<String_Const>()
                .To<String_Const>()
                .WithCustomScope()
                ;
            Bind<String_Default>()
                .To<String_Default>()
                .WithCustomScope()
                ;



            Bind<Char_Const>()
                .To<Char_Const>()
                .WithCustomScope()
                ;
            Bind<Char_Default>()
                .To<Char_Default>()
                .WithCustomScope()
                ;



            Bind<Float_Const>()
                .To<Float_Const>()
                .WithCustomScope()
                ;
            Bind<Float_Default>()
                .To<Float_Default>()
                .WithCustomScope()
                ;



            Bind<Double_Const>()
                .To<Double_Const>()
                .WithCustomScope()
                ;
            Bind<Double_Default>()
                .To<Double_Default>()
                .WithCustomScope()
                ;



            Bind<Decimal_Const>()
                .To<Decimal_Const>()
                .WithCustomScope()
                ;
            Bind<Decimal_Default>()
                .To<Decimal_Default>()
                .WithCustomScope()
                ;




            Bind<Enum_Const>()
                .To<Enum_Const>()
                .WithSingletonScope()
                ;
            Bind<Enum_Default>()
                .To<Enum_Default>()
                .WithSingletonScope()
                ;




            Bind<Reference_Default>()
                .To<Reference_Default>()
                .WithCustomScope()
                ;
        }

        public class DefaultValueCustom_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<DefaultValueCustom_Cluster>(
                    null
                    );

                using var scope = cluster.CreateCustomScope();

                var int_const = cluster.Get<Int_Const>(scope);
                Assert.IsNotNull(int_const);
                Assert.AreEqual(Int_Const.DefValue, int_const.A);

                var int_default = cluster.Get<Int_Default>(scope);
                Assert.IsNotNull(int_default);
                Assert.AreEqual(default(int), int_default.A);



                var string_const = cluster.Get<String_Const>(scope);
                Assert.IsNotNull(string_const);
                Assert.AreEqual(String_Const.DefValue, string_const.A);

                var string_default = cluster.Get<String_Default>(scope);
                Assert.IsNotNull(string_default);
                Assert.AreEqual(default(string), string_default.A);



                var char_const = cluster.Get<Char_Const>(scope);
                Assert.IsNotNull(char_const);
                Assert.AreEqual(Char_Const.DefValue, char_const.A);

                var char_default = cluster.Get<Char_Default>(scope);
                Assert.IsNotNull(char_default);
                Assert.AreEqual(default(char), char_default.A);



                var float_const = cluster.Get<Float_Const>(scope);
                Assert.IsNotNull(float_const);
                Assert.AreEqual(Float_Const.DefValue, float_const.A);

                var float_default = cluster.Get<Float_Default>(scope);
                Assert.IsNotNull(float_default);
                Assert.AreEqual(default(float), float_default.A);



                var double_const = cluster.Get<Double_Const>(scope);
                Assert.IsNotNull(double_const);
                Assert.AreEqual(Double_Const.DefValue, double_const.A);

                var double_default = cluster.Get<Double_Default>(scope);
                Assert.IsNotNull(double_default);
                Assert.AreEqual(default(double), double_default.A);




                var decimal_const = cluster.Get<Decimal_Const>(scope);
                Assert.IsNotNull(decimal_const);
                Assert.AreEqual(Decimal_Const.DefValue, decimal_const.A);

                var decimal_default = cluster.Get<Decimal_Default>(scope);
                Assert.IsNotNull(decimal_default);
                
                Assert.AreEqual(default(decimal), decimal_default.A);




                var enum_const = cluster.Get<Enum_Const>();
                Assert.IsNotNull(enum_const);
                Assert.AreEqual(Enum_Const.DefValue, enum_const.A);

                var enum_default = cluster.Get<Enum_Default>();
                Assert.IsNotNull(enum_default);
                Assert.AreEqual(default(DefaultEnum), enum_default.A);




                var reference_default = cluster.Get<Reference_Default>(scope);
                Assert.IsNotNull(reference_default);
                Assert.AreEqual(default(object), reference_default.A);

            }
        }
    }

    public class Int_Const
    {
        public const int DefValue = 123;

        public int A
        {
            get;
        }

        public Int_Const(int a = DefValue)
        {
            A = a;
        }
    }

    public class Int_Default
    {
        public int A
        {
            get;
        }

        public Int_Default(int a = default)
        {
            A = a;
        }
    }

    public class String_Const
    {
        public const string DefValue = "123";

        public string A
        {
            get;
        }

        public String_Const(string a = DefValue)
        {
            A = a;
        }
    }

    public class String_Default
    {
        public string A
        {
            get;
        }

        public String_Default(string a = default)
        {
            A = a;
        }
    }



    public class Char_Const
    {
        public const char DefValue = 'a';

        public char A
        {
            get;
        }

        public Char_Const(char a = DefValue)
        {
            A = a;
        }
    }

    public class Char_Default
    {
        public char A
        {
            get;
        }

        public Char_Default(char a = default)
        {
            A = a;
        }
    }



    public class Float_Const
    {
        public const float DefValue = 1.234567890f;

        public float A
        {
            get;
        }

        public Float_Const(float a = DefValue)
        {
            A = a;
        }
    }

    public class Float_Default
    {
        public float A
        {
            get;
        }

        public Float_Default(float a = default)
        {
            A = a;
        }
    }



    public class Double_Const
    {
        public const double DefValue = 123456789.1234567890d;

        public double A
        {
            get;
        }

        public Double_Const(double a = DefValue)
        {
            A = a;
        }
    }

    public class Double_Default
    {
        public double A
        {
            get;
        }

        public Double_Default(double a = default)
        {
            A = a;
        }
    }



    public class Decimal_Const
    {
        public const decimal DefValue = 9876543210.1234567890M;

        public decimal A
        {
            get;
        }

        public Decimal_Const(decimal a = DefValue)
        {
            A = a;
        }
    }

    public class Decimal_Default
    {
        public decimal A
        {
            get;
        }

        public Decimal_Default(decimal a = default)
        {
            A = a;
        }
    }





    public enum DefaultEnum
    {
        Def = 0,
        MyValue = 10
    }
    public class Enum_Const
    {
        public const DefaultEnum DefValue = DefaultEnum.MyValue;

        public DefaultEnum A
        {
            get;
        }

        public Enum_Const(DefaultEnum a = DefValue)
        {
            A = a;
        }
    }

    public class Enum_Default
    {
        public DefaultEnum A
        {
            get;
        }

        public Enum_Default(DefaultEnum a = default)
        {
            A = a;
        }
    }





    public class Reference_Default
    {
        public object A
        {
            get;
        }

        public Reference_Default(object a = default)
        {
            A = a;
        }
    }
}
