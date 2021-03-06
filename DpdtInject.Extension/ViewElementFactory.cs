using System;
using System.ComponentModel.Composition;
using System.Windows;
using DpdtInject.Extension.Container;
using DpdtInject.Extension.Shared.Dto;
using DpdtInject.Extension.ViewModel;
using DpdtInject.Generator.Binding;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace DpdtInject.Extension
{
    [Export(typeof(IViewElementFactory))]
    [Name("Dpdt bind details UI factory")]
    [TypeConversion(@from: typeof(DpdtBindingReferenceSet), to: typeof(FrameworkElement))]
    [Order]
    internal class ViewElementFactory : IViewElementFactory
    {
        public TView CreateViewElement<TView>(ITextView textView, object model) where TView : class
        {
            // Should never happen if the service's code is correct, but it's good to be paranoid.
            if (typeof(FrameworkElement) != typeof(TView))
            {
                throw new ArgumentException($"Invalid type conversion. Unsupported {nameof(model)} or {nameof(TView)} type");
            }

            if (model is DpdtBindingReferenceSet set)
            {
                var detailsUI = new BindDetailsWindow();

                var viewModel = new DpdtBindingReferenceSetViewModel(
                    detailsUI.Dispatcher,
                    set
                    );

                detailsUI.DataContext = viewModel;
                return (detailsUI as TView)!;
            }


            //if (model is Type t)
            //{
            //    var detailsUI = new AddBindingWindow();

            //    var viewModel = new AddBindingViewModel(
            //        detailsUI.Dispatcher,
            //        t
            //        );

            //    detailsUI.DataContext = viewModel;
            //    return (detailsUI as TView)!;
            //}

            return null!;
        }
    }
}
