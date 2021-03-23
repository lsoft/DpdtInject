using DpdtInject.Extension.Helper;
using System;

namespace DpdtInject.Extension.UI.ViewModel.AddClusterMethod
{
    public class ClusterClassViewModel : BaseViewModel
    {
        public string ClusterName
        {
            get;
        }

        public ClusterClassViewModel(
            string clusterName
            )
        {
            if (clusterName is null)
            {
                throw new ArgumentNullException(nameof(clusterName));
            }

            ClusterName = clusterName;
        }
    }
}
