using DpdtInject.Injector;
using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.CustomScope;
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



        public TRequestedType Get<TRequestedType>(
            CustomScopeObject? scope = null
            )
        {
            try
            {
                if(scope is null)
                {
                    return Cluster.Get<TRequestedType>();
                }

                return Cluster.Get<TRequestedType>(scope);
            }
            catch (InvalidCastException)
            {
                var clusterSuffix = string.Empty;
                if (!Cluster.IsRootCluster)
                {
                    clusterSuffix = $" in the cluster [{Cluster.DeclaredClusterType.FullName}]";
                }

                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for [{typeof(TRequestedType).FullName}]{clusterSuffix}",
                    typeof(TRequestedType).FullName!
                    );
            }
        }

        public List<TRequestedType> GetAll<TRequestedType>(
            CustomScopeObject? scope = null
            )
        {
            try
            {
                if (scope is null)
                {
                    return Cluster.GetAll<TRequestedType>();
                }

                return Cluster.GetAll<TRequestedType>(scope);
            }
            catch (InvalidCastException)
            {
                var clusterSuffix = string.Empty;
                if (!Cluster.IsRootCluster)
                {
                    clusterSuffix = $" in the cluster [{Cluster.DeclaredClusterType.FullName}]";
                }

                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for [{typeof(TRequestedType).FullName}]{clusterSuffix}",
                    typeof(TRequestedType).FullName!
                    );
            }
        }

        public object Get(
            Type requestedType,
            CustomScopeObject? scope = null
            )
        {
            if (scope is null)
            {
                return Cluster.Get(requestedType);
            }

            return Cluster.Get(requestedType, scope);
        }

        public IEnumerable<object> GetAll(
            Type requestedType,
            CustomScopeObject? scope = null
            )
        {
            if (scope is null)
            {
                return Cluster.GetAll(requestedType);
            }

            return Cluster.GetAll(requestedType, scope);
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

            public T Get<T>(
                CustomScopeObject? scope = null
                )
            {
                return _beautifier.Get<T>(scope);
            }

            public IReadOnlyList<T> GetAll<T>(
                CustomScopeObject? scope = null
                )
            {
                return _beautifier.GetAll<T>(scope);
            }

            public object Get(
                Type requestedType,
                CustomScopeObject? scope = null
                )
            {
                return _beautifier.Get(requestedType, scope);
            }

            public IReadOnlyList<object> GetAll(
                Type requestedType,
                CustomScopeObject? scope = null
                )
            {
                return _beautifier.GetAll(requestedType, scope).ToList();
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

            public T Get<T>(
                CustomScopeObject? scope = null
                )
            {
                return _beautifier.Get<T>(scope);
            }

            public List<T> GetAll<T>(
                CustomScopeObject? scope = null
                )
            {
                return _beautifier.GetAll<T>(scope);
            }

            public object Get(
                Type requestedType,
                CustomScopeObject? scope = null
                )
            {
                return _beautifier.Get(requestedType, scope);
            }

            public List<object> GetAll(
                Type requestedType,
                CustomScopeObject? scope = null
                )
            {
                return _beautifier.GetAll(requestedType, scope).ToList();
            }
        }
    }
}
