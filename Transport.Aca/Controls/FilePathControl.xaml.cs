using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Transport.Aca.Controls
{
    /// <summary>
    /// Interaction logic for FilePathControl.xaml
    /// </summary>
    public partial class FilePathControl : UserControl
    {
        public FilePathControl()
        {
            InitializeComponent();
        }

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(FilePathControl));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(FilePathControl));

        public object CommandParameter
        {
            get { return (ICommand)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(FilePathControl));
    }
}
