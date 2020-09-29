using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.CustomScope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Beautify
{
    public class CustomScopeBeautifier : IBeautifier
    {
        private readonly CustomScopeObject _customScope;

        public ICluster Cluster
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


        public CustomScopeBeautifier(
            ICustomScopeFactory customScopeFactory,
            ICluster cluster
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
            List = new ListBeautifier(this);
            ReadOnlyList = new ReadOnlyListBeautifier(this);
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
    }

}
