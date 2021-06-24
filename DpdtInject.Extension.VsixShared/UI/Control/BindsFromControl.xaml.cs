using System.Windows.Controls;
using System.Windows.Input;

namespace DpdtInject.Extension.UI.Control
{
    /// <summary>
    /// Interaction logic for BindsFromControl.xaml
    /// </summary>
    public partial class BindsFromControl : UserControl
    {
        public BindsFromControl()
        {
            InitializeComponent();
        }

        private void ListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Space)
            {
                e.Handled = true;
            }
        }

    }
}
