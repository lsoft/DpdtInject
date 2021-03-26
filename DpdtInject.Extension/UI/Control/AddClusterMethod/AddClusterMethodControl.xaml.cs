using Microsoft.VisualStudio.PlatformUI;
using System.Windows;

namespace DpdtInject.Extension.UI.Control.AddClusterMethod
{
    /// <summary>
    /// Interaction logic for AddClusterMethodControl.xaml
    /// </summary>
    public partial class AddClusterMethodControl : DialogWindow
    {
        public AddClusterMethodControl()
        {
            InitializeComponent();

            this.ShouldBeThemed();

            //var comboTheme = System.Windows.Application.LoadComponent(new System.Uri("DpdtInject.Extension;component/UI/Control/VsThemedComboBox.xaml", System.UriKind.Relative));
            //ResourceDictionary allResources = new ResourceDictionary();
            //allResources.MergedDictionaries.Add(comboTheme);

        }
    }
}
