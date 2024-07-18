using System.Diagnostics;
using System.Text;
using System.Windows;
using Microsoft.SemanticKernel.ChatCompletion;

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

            SemanticModeComBox.ItemsSource = Enum.GetValues<SemanticMode>();
            SemanticModeComBox.SelectedIndex = 0;
        }


        private async void ExecuteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var promptText = PromptTextBox.Text;
            if (string.IsNullOrEmpty(promptText))
            {
                MessageBox.Show("提示词为空！！");
                return;
            }

            try
            {
                var semanticMode = GetSemanticMode();
                var chatCompletionService = App.SkKernel.GetRequiredService<IChatCompletionService>(semanticMode);
                var chatHistory = new ChatHistory(promptText);
                var stopwatch = Stopwatch.StartNew();
                ResponseTextBox.Text += "\n 执行中...........";
                var responseResult = await chatCompletionService.GetChatMessageContentsAsync(chatHistory);
                stopwatch.Stop();
                ResponseTextBox.Text += $"\n 执行耗时：{stopwatch.Elapsed.TotalSeconds} s \n -------------------  \n";
                ResponseTextBox.ScrollToEnd();

                var stringBuilder = new StringBuilder();
                foreach (var chatMessageContent in responseResult)
                {
                    stringBuilder.Append(chatMessageContent);
                }

                var result = $"{stringBuilder} \n";

                ResponseTextBox.Text += result;

                ResponseTextBox.ScrollToEnd();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error：{exception.Message}");
            }

            
        }

        private async void ExecuteStreamButton_OnClick(object sender, RoutedEventArgs e)
        {
            var promptText = PromptTextBox.Text;
            if (string.IsNullOrEmpty(promptText))
            {
                MessageBox.Show("提示词为空！！");
                return;
            }

            try
            {
                var isFirst = true;
                var executeStopwatch = Stopwatch.StartNew();
                ResponseTextBox.Text += "\n 执行中...........";
                var semanticMode = GetSemanticMode();
                var chatCompletionService = App.SkKernel.GetRequiredService<IChatCompletionService>(semanticMode);
                var chatHistory = new ChatHistory(promptText);
                await foreach (var streamingChatMessageContent in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory))
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        executeStopwatch.Stop();
                        ResponseTextBox.Text += $"\n 执行耗时：{executeStopwatch.Elapsed.TotalSeconds} s \n -------------------  \n";
                        ResponseTextBox.ScrollToEnd();

                    }

                    var result = $" {streamingChatMessageContent}";
                    ResponseTextBox.Text += result;
                    ResponseTextBox.ScrollToEnd();

                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error：{exception.Message}");
            }

        }


        public string GetSemanticMode()
        {
            if (SemanticModeComBox.SelectionBoxItem is not SemanticMode semanticMode) return "llama3";
            return semanticMode switch
            {
                SemanticMode.Llama3 => "llama3",
                SemanticMode.Phi3 => "phi3",
                SemanticMode.Llava => "llava",
                _ => "llama3"
            };
        }

    }


    enum SemanticMode
    {
        Llama3,

        Phi3,

        Llava
    }
}