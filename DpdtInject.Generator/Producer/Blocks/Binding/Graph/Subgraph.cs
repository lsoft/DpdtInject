using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DpdtInject.Generator.Producer.Blocks.Binding.Graph
{
    public class Subgraph
    {
        private readonly HashSet<InstanceContainerGenerator> _used;
        private readonly List<InstanceContainerGenerator> _usedInList;

        private bool _idempotent = true;

        public Subgraph()
        {
            _used = new HashSet<InstanceContainerGenerator>(
                );
            _usedInList = new List<InstanceContainerGenerator>();
        }

        private Subgraph(Subgraph subgraph)
        {
            _used = new HashSet<InstanceContainerGenerator>(subgraph._used, subgraph._used.Comparer);
            _usedInList = new List<InstanceContainerGenerator>(subgraph._usedInList);
            _idempotent = subgraph._idempotent;
        }

        public void AppendOrFailIfExists(
            InstanceContainerGenerator generator,
            bool idempotent
            )
        {
            if (_used.Contains(generator))
            {
                var cycleList = new List<InstanceContainerGenerator>(_usedInList);
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
