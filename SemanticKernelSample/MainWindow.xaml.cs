using System.Diagnostics;
using Microsoft.SemanticKernel;
using System.Windows;

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

            var stopwatch = Stopwatch.StartNew();
            ResponseTextBox.Text += "\n 执行中...........";
            var responseResult = await App.SkKernel.InvokePromptAsync(promptText);
            stopwatch.Stop();
            ResponseTextBox.Text += $"\n 执行耗时：{stopwatch.Elapsed.TotalSeconds} s \n -------------------  \n";
            ResponseTextBox.ScrollToEnd();


            var result = $"{responseResult} \n";

            ResponseTextBox.Text += result;

            ResponseTextBox.ScrollToEnd();
        }

        private async void ExecuteStreamButton_OnClick(object sender, RoutedEventArgs e)
        {
            var promptText = PromptTextBox.Text;
            if (string.IsNullOrEmpty(promptText))
            {
                MessageBox.Show("提示词为空！！");
                return;
            }

            var isFirst = true;
            var executeStopwatch = Stopwatch.StartNew();
            var executeTotalStopwatch = Stopwatch.StartNew();
            ResponseTextBox.Text += "\n 执行中...........";
            await foreach (var streamingKernelContent in App.SkKernel.InvokePromptStreamingAsync(promptText))
            {
                if (isFirst)
                {
                    isFirst = false;
                    executeStopwatch.Stop();
                    ResponseTextBox.Text += $"\n 执行耗时：{executeStopwatch.Elapsed.TotalSeconds} s \n -------------------  \n";
                    ResponseTextBox.ScrollToEnd();

                }

                var result = $" {streamingKernelContent}";
                ResponseTextBox.Text += result;
                ResponseTextBox.ScrollToEnd();

            }
            executeTotalStopwatch.Stop();
            ResponseTextBox.Text += $"\n 流式总执行耗时：{executeTotalStopwatch.Elapsed.TotalSeconds} s \n -------------------  \n";
            ResponseTextBox.ScrollToEnd();
        }
    }
}