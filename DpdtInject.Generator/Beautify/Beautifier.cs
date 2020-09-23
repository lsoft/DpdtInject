using DpdtInject.Injector;
using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Excp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Beautify
{
    public sealed class Beautifier : IBeautifier
    {
        public FakeCluster Cluster
        {
            get;
        }

        public ListBeautifier List
        {
            get;
        }
        public ReadOnlyListBeautifier ReadOnlyList
        {
            get;
        }

        IListBeautifier IBeautifier.List => List;

        IReadOnlyListBeautifier IBeautifier.ReadOnlyList => ReadOnlyList;

        public Beautifier(
            FakeCluster cluster
            )
        {
            if (cluster is null)
            {
                throw new ArgumentNullException(nameof(cluster));
            }

            Cluster = cluster;
            List = new ListBeautifier(this);
            ReadOnlyList = new ReadOnlyListBeautifier(this);
        }

        public bool IsRegisteredFrom<TRequestedType>()
        {
            return Cluster.IsRegisteredFrom<TRequestedType>();
        }

        public TRequestedType Get<TRequestedType>()
        {
            try
            {
                return Cluster.Get<TRequestedType>();
            }
            catch (InvalidCastException)
            {
                var clusterSuffix = string.Empty;
                if (!Cluster.IsRootCluster)
                {
                    clusterSuffix = $" in the cluster [{Cluster.DeclaredClusterType}]";
                }

                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for [{typeof(TRequestedType).FullName}]{clusterSuffix}",
                    typeof(TRequestedType).FullName!
                    );
            }
        }

        public List<TRequestedType> GetAll<TRequestedType>()
        {
            try
            {
                return Cluster.GetAll<TRequestedType>();
            }
            catch (InvalidCastException)
            {
                var clusterSuffix = string.Empty;
                if (!Cluster.IsRootCluster)
                {
                    clusterSuffix = $" in the cluster [{Cluster.DeclaredClusterType}]";
                }

                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for [{typeof(TRequestedType).FullName}]{clusterSuffix}",
                    typeof(TRequestedType).FullName!
                    );
            }
        }

        public object Get(Type requestedType)
        {
            return Cluster.Get(requestedType);
        }

        public IEnumerable<object> GetAll(Type requestedType)
        {
            return Cluster.GetAll(requestedType);
        }


        public sealed class ReadOnlyListBeautifier : IReadOnlyListBeautifier
        {
            private readonly Beautifier _beautifier;

            public ReadOnlyListBeautifier(
                Beautifier beautifier
                )
            {
                if (beautifier is null)
                {
                    throw new ArgumentNullException(nameof(beautifier));
                }

                _beautifier = beautifier;
            }

            public bool IsRegisteredFrom<T>()
            {
                return _beautifier.IsRegisteredFrom<T>();
            }

            public T Get<T>()
            {
                return _beautifier.Get<T>();
            }

            public IReadOnlyList<T> GetAll<T>()
            {
                return _beautifier.GetAll<T>();
            }

            public object Get(Type requestedType)
            {
                return _beautifier.Get(requestedType);
            }

            public IReadOnlyList<object> GetAll(Type requestedType)
            {
                return _beautifier.GetAll(requestedType).ToList();
            }
        }

        public sealed class ListBeautifier : IListBeautifier
        {
            private readonly Beautifier _beautifier;

            public ListBeautifier(
                Beautifier beautifier
                )
            {
                if (beautifier is null)
                {
                    throw new ArgumentNullException(nameof(beautifier));
                }

                _beautifier = beautifier;
            }

            public bool IsRegisteredFrom<T>()
            {
                return _beautifier.IsRegisteredFrom<T>();
            }

            public T Get<T>()
            {
                return _beautifier.Get<T>();
            }

            public List<T> GetAll<T>()
            {
                return _beautifier.GetAll<T>();
            }

            public object Get(Type requestedType)
            {
                return _beautifier.Get(requestedType);
            }

            public List<object> GetAll(Type requestedType)
            {
                return _beautifier.GetAll(requestedType).ToList();
            }
        }
    }
}
