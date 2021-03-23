using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;
using Task = System.Threading.Tasks.Task;

namespace DpdtInject.Extension
{
    /// <summary>
    /// Interaction logic for AddBindingWindow.xaml
    /// </summary>
    public partial class AddBindingWindow : DialogWindow
    {
        private readonly Func<AddBindingWindow, Task> _factory;

        public AddBindingWindow()
        {
            _factory = null!;

            InitializeComponent();
        }

        /// <inheritdoc />
        public AddBindingWindow(
            Func<AddBindingWindow, Task> factory
            )
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _factory = factory;

            InitializeComponent();

            this.HasMaximizeButton = true;
            this.HasMinimizeButton = false;
        }

        private async void AddBindingWindow_OnLoaded(
            object sender,
            RoutedEventArgs e
            )
        {
            await _factory(this);
        }


        private void AddBindingWindow_OnKeyUp(
            object sender,
            KeyEventArgs e
            )
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

    }
}
