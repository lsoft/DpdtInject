using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DpdtInject.Generator.Producer.Blocks.Binding.Graph
{
    public class Subgraph
    {
        private readonly HashSet<Generator> _used;
        private readonly List<Generator> _usedInList;

        private bool _idempotent = true;

        public Subgraph()
        {
            _used = new HashSet<Generator>(
                );
            _usedInList = new List<Generator>();
        }

        private Subgraph(Subgraph subgraph)
        {
            _used = new HashSet<Generator>(subgraph._used, subgraph._used.Comparer);
            _usedInList = new List<Generator>(subgraph._usedInList);
            _idempotent = subgraph._idempotent;
        }

        public void AppendOrFailIfExists(
            Generator generator,
            bool idempotent
            )
        {
            if (_used.Contains(generator))
            {
                var cycleList = new List<Generator>(_usedInList);
                cycleList.Add(generator);

                throw new CycleFoundException(
                    cycleList,
                    _idempotent
                    );
            }

            _used.Add(generator);
            _usedInList.Add(generator);
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
