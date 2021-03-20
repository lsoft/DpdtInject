using DpdtInject.Extension.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
