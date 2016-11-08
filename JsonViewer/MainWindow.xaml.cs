using MahApps.Metro.Controls;

namespace JsonViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            TextEditor.Options.HighlightCurrentLine = true;
        }

    }
}
