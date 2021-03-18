using DpdtInject.Generator.Core.Binding;
using System.Collections.Generic;

namespace DpdtInject.Generator.Core.Graph
{
    public class Subgraph
    {
        private readonly HashSet<IBindingContainer> _used;
        private readonly List<IBindingContainer> _usedInList;

        private bool _idempotent = true;

        public Subgraph()
        {
            _used = new HashSet<IBindingContainer>(
                );
            _usedInList = new List<IBindingContainer>();
        }

        private Subgraph(Subgraph subgraph)
        {
            _used = new HashSet<IBindingContainer>(subgraph._used, subgraph._used.Comparer);
            _usedInList = new List<IBindingContainer>(subgraph._usedInList);
            _idempotent = subgraph._idempotent;
        }

        public void AppendOrFailIfExists(
            IBindingContainer bindingContainer,
            bool idempotent
            )
        {
            if (_used.Contains(bindingContainer))
            {
                var cycleList = new List<IBindingContainer>(_usedInList);
                cycleList.Add(bindingContainer);

                throw new CycleFoundException(
                    cycleList,
                    _idempotent
                    );
            }

            _used.Add(bindingContainer);
            _usedInList.Add(bindingContainer);
            _idempotent &= idempotent;
        }

        public Subgraph Clone()
        {
            return new(
                this
                );
        }
    }
}
