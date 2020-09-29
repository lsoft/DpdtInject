using DpdtInject.Injector;
using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.CustomScope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Beautify
{
    public class CustomScopeBeautifier : IBeautifier
    {
        private readonly CustomScopeObject _customScope;

        public FakeCluster Cluster
        {
            get;
        }

        public CustomScopeListBeautifier List
        {
            get;
        }
        public CustomScopeReadOnlyListBeautifier ReadOnlyList
        {
            get;
        }

        IListBeautifier IBeautifier.List => List;

        IReadOnlyListBeautifier IBeautifier.ReadOnlyList => ReadOnlyList;


        public CustomScopeBeautifier(
            ICustomScopeFactory customScopeFactory,
            FakeCluster cluster
            )
        {
            if (customScopeFactory is null)
            {
                throw new ArgumentNullException(nameof(customScopeFactory));
            }

            if (cluster is null)
            {
                throw new ArgumentNullException(nameof(cluster));
            }

            _customScope = customScopeFactory.CreateCustomScope();

            Cluster = cluster;
            List = new CustomScopeListBeautifier(this);
            ReadOnlyList = new CustomScopeReadOnlyListBeautifier(this);
        }


        public bool IsRegisteredFrom<TRequestedType>()
        {
            return Cluster.IsRegisteredFrom<TRequestedType>();
        }



        public TRequestedType Get<TRequestedType>(
            )
        {
            try
            {
                return Cluster.Get<TRequestedType>(_customScope);
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
            )
        {
            try
            {
                return Cluster.GetAll<TRequestedType>(_customScope);
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
            Type requestedType
            )
        {
            return Cluster.Get(requestedType, _customScope);
        }

        public IEnumerable<object> GetAll(
            Type requestedType
            )
        {
            return Cluster.GetAll(requestedType, _customScope);
        }


        public sealed class CustomScopeReadOnlyListBeautifier : IReadOnlyListBeautifier
        {
            private readonly CustomScopeBeautifier _beautifier;

            public CustomScopeReadOnlyListBeautifier(
                CustomScopeBeautifier beautifier
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
                )
            {
                return _beautifier.Get<T>();
            }

            public IReadOnlyList<T> GetAll<T>(
                )
            {
                return _beautifier.GetAll<T>();
            }

            public object Get(
                Type requestedType
                )
            {
                return _beautifier.Get(requestedType);
            }

            public IReadOnlyList<object> GetAll(
                Type requestedType
                )
            {
                return _beautifier.GetAll(requestedType).ToList();
            }
        }

        public sealed class CustomScopeListBeautifier : IListBeautifier
        {
            private readonly CustomScopeBeautifier _beautifier;

            public CustomScopeListBeautifier(
                CustomScopeBeautifier beautifier
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
                )
            {
                return _beautifier.Get<T>();
            }

            public List<T> GetAll<T>(
                )
            {
                return _beautifier.GetAll<T>();
            }

            public object Get(
                Type requestedType
                )
            {
                return _beautifier.Get(requestedType);
            }

            public List<object> GetAll(
                Type requestedType
                )
            {
                return _beautifier.GetAll(requestedType).ToList();
            }
        }
    }
}
