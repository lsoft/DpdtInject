using DpdtInject.Extension.Helper;

namespace DpdtInject.Extension.UI.ViewModel
{
    public abstract class ChainViewModel : BaseViewModel
    {
        protected ChainViewModel()
            : base()
        {
        }

        public abstract System.Threading.Tasks.Task StartAsync();
    }
}
