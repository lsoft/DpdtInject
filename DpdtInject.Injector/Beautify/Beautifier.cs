using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.CustomScope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Beautify
{
#nullable disable

    public sealed class Beautifier : IBeautifier
    {
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

        public Beautifier(
            ICluster cluster
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
            )
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
                return Cluster.GetAll<TRequestedType>();
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
            return Cluster.Get(requestedType);
        }

        public IEnumerable<object> GetAll(
            Type requestedType
            )
        {
            return Cluster.GetAll(requestedType);
        }
    }
}
