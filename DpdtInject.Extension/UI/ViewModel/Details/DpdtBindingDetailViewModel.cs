using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;

namespace DpdtInject.Extension.UI.ViewModel.Details
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
            IDpdtBindingDetail target
            )
        {
            _target = target;

            BindScope = target.BindScope;
            ConditionalBinding = target.ConditionalBinding;
        }

    }
}
