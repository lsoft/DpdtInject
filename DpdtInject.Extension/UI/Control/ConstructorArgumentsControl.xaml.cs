using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DpdtInject.Extension.UI.Control
{
    /// <summary>
    /// Interaction logic for ConstructorArgumentsControl.xaml
    /// </summary>
    public partial class ConstructorArgumentsControl : UserControl
    {
        public ConstructorArgumentsControl()
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
