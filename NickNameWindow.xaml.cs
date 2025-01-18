using System.Windows;

namespace EternalReturnOverlay
{
    public partial class NickNameWindow : Window
    {
        public string NickName { get; private set; } = string.Empty;

        public NickNameWindow()
        {
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            NickName = NickNameInput.Text;
            this.DialogResult = true;
            this.Close();
        }
    }
}