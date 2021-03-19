using System;
using System.Windows;
using System.Windows.Threading;
using DpdtInject.Extension.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Extension.ViewModel.Add
{
    public class BindFromViewModel : BaseViewModel
    {
        private bool _isChecked;

        public Thickness LeftMargin
        {
            get;
        }

        public INamedTypeSymbol FromType
        {
            get;
        }

        public bool IsSelected
        {
            get;
            set;
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
            int level,
            INamedTypeSymbol fromType
            )
        {
            if (fromType is null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }

            LeftMargin = new Thickness(level * 5, 0, 0, 0);
            FromType = fromType;

            VisualRepresentationTypeName = $"{fromType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}";
            VisualRepresentationNamespace = $"({fromType.ContainingNamespace.ToDisplayString()})";
        }
    }
}
