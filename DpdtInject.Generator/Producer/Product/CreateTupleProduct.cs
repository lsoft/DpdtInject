using DpdtInject.Injector;
using DpdtInject.Injector.Helper;
using Microsoft.CodeAnalysis;
using System;
using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace DpdtInject.Generator.Producer.Product
{
    public class CreateTupleProduct : IWritable
    {
        private readonly IWritablePair[] _tuplesParts;

        public CreateTupleProduct(
            params IWritablePair[] tuplesParts
            )
        {
            _tuplesParts = tuplesParts;
        }

        public void Write(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.Write("new System.Tuple<");

            _tuplesParts.IterateWithLastSignal(
                t => t.WriteItem0(writer, sng),
                () => writer.Write(", ")
                );

            writer.Write(">(");

            _tuplesParts.IterateWithLastSignal(
                t => t.WriteItem1(writer, sng),
                () => writer.Write(", ")
                );

            writer.Write(")");
        }
    }

    public interface IWritablePair
    {
        void WriteItem0(IndentedTextWriter2 writer, ShortTypeNameGenerator sng);
        void WriteItem1(IndentedTextWriter2 writer, ShortTypeNameGenerator sng);
    }

    public class TypeTypePair : IWritablePair
    {
        private readonly ITypeSymbol _item0;
        private readonly ITypeSymbol _item1;

        public TypeTypePair(ITypeSymbol item0, ITypeSymbol item1)
        {
            if (item0 is null)
            {
                throw new ArgumentNullException(nameof(item0));
            }

            if (item1 is null)
            {
                throw new ArgumentNullException(nameof(item1));
            }

            _item0 = item0;
            _item1 = item1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteItem0(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.Write($"{sng.GetShortName(_item0)}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteItem1(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.Write($"typeof({sng.GetShortName(_item1)})");
        }
    }

    public class TypeStringPair : IWritablePair
    {
        private readonly ITypeSymbol _item0;
        private readonly string _item1;

        public TypeStringPair(ITypeSymbol item0, string item1)
        {
            if (item0 is null)
            {
                throw new ArgumentNullException(nameof(item0));
            }

            if (item1 is null)
            {
                throw new ArgumentNullException(nameof(item1));
            }

            _item0 = item0;
            _item1 = item1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteItem0(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.Write($"{sng.GetShortName(_item0)}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteItem1(IndentedTextWriter2 writer, ShortTypeNameGenerator sng)
        {
            writer.Write(_item1);
        }
    }

}
