using System;
using DpdtInject.Extension.Helper;
using DpdtInject.Generator.Core.Binding.Xml;

namespace DpdtInject.Extension.UI.ViewModel.Add.Inner
{
    public class TargetMethodViewModel : BaseViewModel
    {
        private bool _isChecked;

        public IMethodBindContainer MethodBindContainer
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
            IMethodBindContainer mbc
            )
        {
            if (mbc is null)
            {
                throw new ArgumentNullException(nameof(mbc));
            }

            MethodBindContainer = mbc;

            VisualRepresentationClassMethod = $"{mbc.ClusterTypeInfo.Name}.{mbc.MethodDeclaration.MethodName}";
            VisualRepresentationNamespace = $"({mbc.ClusterTypeInfo.FullNamespaceDisplayName})";
        }
    }
}
