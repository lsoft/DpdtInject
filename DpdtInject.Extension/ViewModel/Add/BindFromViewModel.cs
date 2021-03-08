using System;
using System.Windows.Threading;
using DpdtInject.Extension.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Extension.ViewModel.Add
{
    public class BindFromViewModel : BaseViewModel
    {
        private bool _isChecked;

        public INamedTypeSymbol FromType
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

        public string VisualRepresentationTypeName
        {
            get;
        }

        public string VisualRepresentationNamespace
        {
            get;
        }


        /// <inheritdoc />
        public BindFromViewModel(
            Dispatcher dispatcher,
            INamedTypeSymbol fromType
            )
            : base(dispatcher)
        {
            if (fromType is null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }

            FromType = fromType;

            VisualRepresentationTypeName = $"{fromType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}";
            VisualRepresentationNamespace = $"({fromType.ContainingNamespace.ToDisplayString()})";
        }
    }
}
