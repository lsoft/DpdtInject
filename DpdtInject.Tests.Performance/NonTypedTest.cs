using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DpdtInject.Injector.Module;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DpdtInject.Tests.Performance
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class NonTypedTest
    {
        private readonly System.Type _incomingType;
        private readonly Provider _provider;
        private readonly Dictionary<Type, Type> _dict1;
        private readonly Dictionary<Type, MethodInfo> _dict2;
        private readonly Dictionary<Type, Func<object>> _dict3;
        private readonly BicycleMap _bicycle;

        public NonTypedTest()
        {
            _incomingType = typeof(IA);
            _provider = new Provider();

            _dict1 = new Dictionary<Type, Type>()
            {
                [typeof(IA)] = typeof(IProvider<IA>),
                [typeof(IB)] = typeof(IProvider<IB>),
            };

            _dict2 = new Dictionary<Type, MethodInfo>()
            {
                [typeof(IA)] = typeof(IProvider<IA>).GetMethod(nameof(IProvider<IA>.Get), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic),
                [typeof(IB)] = typeof(IProvider<IB>).GetMethod(nameof(IProvider<IB>.Get), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic),
            };

            _dict3 = new Dictionary<Type, Func<object>>()
            {
                [typeof(IA)] = () => ((IProvider<IA>)_provider).Get(),
                [typeof(IB)] = () => ((IProvider<IB>)_provider).Get(),
            };

            _bicycle = new BicycleMap(
                (typeof(IA), () => ((IProvider<IA>)_provider).Get()),
                (typeof(IB), () => ((IProvider<IB>)_provider).Get())
                );
        }

        [Benchmark]
        public void MakeGenericType()
        {
            var targetType = typeof(IProvider<>).MakeGenericType(_incomingType);

            var r = targetType.InvokeMember(
                nameof(IProvider<object>.Get),
                System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public ,//| System.Reflection.BindingFlags.NonPublic,
                null, _provider, null
                );
        }

        [Benchmark]
        public void FromDict1GenericType()
        {
            var targetType = _dict1[_incomingType];

            var r = targetType.InvokeMember(
                nameof(IProvider<object>.Get),
                System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public ,//| System.Reflection.BindingFlags.NonPublic,
                null, _provider, null
                );
        }

        [Benchmark]
        public void FromDict2GenericType()
        {
            var targetMethod = _dict2[_incomingType];

            var r = targetMethod.Invoke(
                _provider,
                //System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public,//| System.Reflection.BindingFlags.NonPublic,
                null
                );
        }

        [Benchmark]
        public void FromDict3GenericType()
        {
            var targetFunc = _dict3[_incomingType];

            var r = targetFunc();
        }

        [Benchmark]
        public void FromDictBicycleGenericType()
        {
            var targetFunc = _bicycle.GetGet(_incomingType);

            var r = targetFunc();
        }


        public class BicycleMap
        {
            private class HashTuple
            {
                public Type Type;
                public Func<object> Factory;

                public HashTuple(Type type, Func<object> factory)
                    //: this()
                {
                    Type = type;
                    Factory = factory;
                }

                //public HashTuple()
                //{
                //    Type = typeof(NonUsedClass);
                //    Factory = null;
                //}
            }

            private readonly int _length;
            private readonly List<HashTuple>[] _table;

            public BicycleMap(
                params (Type, Func<object>)[] pairs
                )
            {
                _length = GetPower2Length(pairs.Length);
                _table = new List<HashTuple>[_length];

                foreach (var pair in pairs)
                {
                    var type = pair.Item1;
                    var func = pair.Item2;

                    var index = CalculateIndex(type);

                    if (_table[index] is null)
                    {
                        _table[index] = new List<HashTuple>();
                    }

                    _table[index].Add(new HashTuple(type, func));
                }
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Func<object> GetGet(Type requestedType)
            {
                if (requestedType is null)
                {
                    throw new ArgumentNullException(nameof(requestedType));
                }

                var index = CalculateIndex(requestedType);

                var list = _table[index];

                if (list is null)
                {
                    return null;
                }

                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];

                    if (item.Type == requestedType)
                    {
                        return item.Factory;
                    }
                }

                return null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private int CalculateIndex(Type type)
            {
                var hashCode = type.GetHashCode();
                var index = Math.Abs(hashCode) % _length;
                return index;
            }

            private static int GetPower2Length(int number)
            {
                var log = Math.Log(number, 2);
                var power = (int)Math.Ceiling(log);

                return (int)Math.Pow(2, power);
            }

            //private class NonUsedClass { }
        }


        #region subject

        public class Provider : IProvider<IA>, IProvider<IB>
        {
            IA IProvider<IA>.Get()
            {
                return new A();
            }

            IB IProvider<IB>.Get()
            {
                return new B();
            }
        }


        public interface IProvider<T>
        {
            T Get();
        }

        public interface IA { }
        public class A : IA { }

        public interface IB { }
        public class B : IB { }

        public interface INot { }

        #endregion
    }

}
