using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using DpdtInject.Extension.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;

namespace DpdtInject.Extension.ViewModel.Add
{
    public class ConstructorViewModel : BaseViewModel
    {
        private bool _isChecked;

        public INamedTypeSymbol TargetType
        {
            get;
        }

        public IMethodSymbol Constructor
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

        public string VisualRepresentation
        {
            get;
        }

        public Brush ForegroundColor
        {
            get;
        }



        /// <inheritdoc />
        public ConstructorViewModel(
            Dispatcher dispatcher,
            INamedTypeSymbol targetType,
            IMethodSymbol constructor
            )
            : base(dispatcher)
        {
            if (constructor is null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            TargetType = targetType;
            Constructor = constructor;

            var arguments = string.Join(
                ",",
                constructor.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}")
                );

            if (constructor.DeclaredAccessibility == Accessibility.Public)
            {
                VisualRepresentation = $"{targetType.Name}({arguments})";
                ForegroundColor = (Brush)Application.Current.FindResource(VsBrushes.WindowTextKey);
            }
            else
            {
                VisualRepresentation = $"{targetType.Name}({arguments})  ({constructor.DeclaredAccessibility})";
                ForegroundColor = Brushes.Gray;
            }
        }
    }
}
