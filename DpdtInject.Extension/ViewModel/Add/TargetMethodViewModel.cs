using System;
using System.Windows.Threading;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.Helper;

namespace DpdtInject.Extension.ViewModel.Add
{
    public class TargetMethodViewModel : BaseViewModel
    {
        private bool _isChecked;

        public MethodBindContainer MethodBindContainer
        {
            get;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public string VisualRepresentationClassMethod
        {
            get;
        }

        public string VisualRepresentationNamespace
        {
            get;
        }

        /// <inheritdoc />
        public TargetMethodViewModel(
            Dispatcher dispatcher,
            MethodBindContainer mbc
            )
            : base(dispatcher)
        {
            if (mbc is null)
            {
                throw new ArgumentNullException(nameof(mbc));
            }

            MethodBindContainer = mbc;

            VisualRepresentationClassMethod = $"{mbc.ClusterType.Name}.{mbc.MethodSyntax.Identifier.Text}";
            VisualRepresentationNamespace = $"({mbc.ClusterType.ContainingNamespace.ToDisplayString()})";
        }
    }
}
