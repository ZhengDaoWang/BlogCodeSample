using Microsoft.SemanticKernel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SemanticKernelSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void ExecuteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var promptText = PromptTextBox.Text;
            if (string.IsNullOrEmpty(promptText))
            {
                MessageBox.Show("提示词为空！！");
                return;
            }

            var responseResult = await App.SkKernel.InvokePromptAsync(promptText);

            var result = $" -------------------  /n {responseResult}";

            ResponseTextBox.Text += result;

            ResponseTextBox.ScrollToEnd();
        }
    }
}