using System.Windows;

namespace MTChat.Client.Views
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();

            ChatTextBox.TextChanged += (sender, args) => { ChatTextBox.ScrollToEnd(); };
        }
    }
}
