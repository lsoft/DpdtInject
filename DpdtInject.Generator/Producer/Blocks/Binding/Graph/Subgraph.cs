using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DpdtInject.Generator.Producer.Blocks.Binding.Graph
{
    public class Subgraph
    {
        private readonly HashSet<ITypeSymbol> _used;
        private readonly List<ITypeSymbol> _usedInList;

        private bool _idempotent = true;

        public Subgraph()
        {
            _used = new HashSet<ITypeSymbol>(
                new TypeSymbolEqualityComparer()
                );
            _usedInList = new List<ITypeSymbol>();
        }

        private Subgraph(Subgraph subgraph)
        {
            _used = new HashSet<ITypeSymbol>(subgraph._used, subgraph._used.Comparer);
            _usedInList = new List<ITypeSymbol>(subgraph._usedInList);
            _idempotent = subgraph._idempotent;
        }

        public void AppendOrFailIfExists(
            ITypeSymbol node,
            bool idempotent
            )
        {
            if (_used.Contains(node))
            {
                var cycleList = new List<ITypeSymbol>(_usedInList);
                cycleList.Add(node);

                throw new CycleFoundException(
                    cycleList,
                    _idempotent
                    );
            }

            _used.Add(node);
            _usedInList.Add(node);
            _idempotent &= idempotent;
        }

        public Subgraph Clone()
        {
            return new Subgraph(
                this
                );
        }
    }
}
