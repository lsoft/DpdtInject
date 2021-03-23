using System;
using DpdtInject.Extension.Helper;
using Microsoft.CodeAnalysis;

namespace DpdtInject.Extension.UI.ViewModel.Add.Inner
{
    public class ConstructorArgumentViewModel : BaseViewModel
    {
        private bool _isChecked;

        public IParameterSymbol Parameter
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

        public string VisualRepresentationTypeAndName
        {
            get;
        }

        public string VisualRepresentationNamespace
        {
            get;
        }


        /// <inheritdoc />
        public ConstructorArgumentViewModel(
            IParameterSymbol parameter
            )
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            Parameter = parameter;

            VisualRepresentationTypeAndName = $"{parameter.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} {parameter.Name}";
            VisualRepresentationNamespace = $"{parameter.Type.ContainingNamespace.ToDisplayString()}";
        }
    }
}
