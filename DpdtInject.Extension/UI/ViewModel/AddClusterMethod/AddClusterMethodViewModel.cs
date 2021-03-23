using DpdtInject.Extension.Helper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Extension.UI.ViewModel.AddClusterMethod
{
    public class AddClusterMethodViewModel : BaseViewModel
    {
        public string AdditionalFolders
        {
            get;
            set;
        }

        public ObservableCollection<ClusterClassViewModel> ClusterClassNameList
        {
            get;
        }

        public string BindingMethodName
        {
            get;
            set;
        }

        public AddClusterMethodViewModel(string additionalFolders)
        {
            ClusterClassNameList = new ObservableCollection<ClusterClassViewModel>();


            AdditionalFolders = additionalFolders;
            ClusterClassNameList.Add(new ClusterClassViewModel("Class1"));
            ClusterClassNameList.Add(new ClusterClassViewModel("Class2"));
            BindingMethodName = "DeclareBindings";
        }
    }
}
