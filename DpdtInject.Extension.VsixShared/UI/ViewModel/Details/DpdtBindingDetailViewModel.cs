using DpdtInject.Extension.Helper;
using DpdtInject.Extension.Shared.Dto;
using System.Windows;

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

        public bool ConventionalBinding
        {
            get;
        }

        public Visibility UnbindVisibility
        {
            get
            {
                return ConventionalBinding ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public DpdtBindingDetailViewModel(
            IDpdtBindingDetail target
            )
        {
            _target = target;

            BindScope = target.BindScope;
            ConditionalBinding = target.ConditionalBinding;
            ConventionalBinding = target.ConventionalBinding;
        }

    }
}
