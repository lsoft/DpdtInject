using System.Windows.Threading;
using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;

namespace DpdtInject.Extension.ViewModel.Details
{
    public class DpdtBindingDetailViewModel : BaseViewModel, IDpdtBindingDetail
    {
        private readonly IDpdtBindingDetail _target;

        /// <inheritdoc />
        public string BindScope
        {
            get;
        }

        /// <inheritdoc />
        public bool ConditionalBinding
        {
            get;
        }


        public DpdtBindingDetailViewModel(
            Dispatcher dispatcher,
            IDpdtBindingDetail target
            ) : base(dispatcher)
        {
            _target = target;

            BindScope = target.BindScope;
            ConditionalBinding = target.ConditionalBinding;
        }

    }
}
