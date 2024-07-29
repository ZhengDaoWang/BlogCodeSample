using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
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
                ResponseTextBox.Text += $"\n当前模型：{semanticMode} 执行中...........";
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
                var semanticMode = GetSemanticMode();
                ResponseTextBox.Text += $"\n当前模型：{semanticMode} 执行中...........";
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

        private OllamaApiClient LoadCustomMode(string modelName)
        {
            var client = new OllamaApiClient("http://localhost:11434", modelName);

            return client;
        }


        public string GetSemanticMode()
        {
            if (SemanticModeComBox.SelectionBoxItem is not SemanticMode semanticMode) return "llama3";
            return semanticMode switch
            {
                SemanticMode.Llama3 => "llama3",
                SemanticMode.Phi3 => "phi3",
                SemanticMode.Llava8B => "llava",
                SemanticMode.Llava34B => "llava:34b",
                _ => "llama3"
            };
        }

        private async void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Load the image file
            var imagePath = ImageTextBox.Text;

            imagePath = imagePath.Replace("\"", "");
            if (string.IsNullOrEmpty(imagePath))
            {
                MessageBox.Show("image path is empty");
                return;
            }

            if (!File.Exists(imagePath))
            {
                MessageBox.Show("image path is not exist");
                return;
            }

            var semanticMode = GetSemanticMode();
            var client = LoadCustomMode(semanticMode);
            var chat = new Chat(client, _ => { });

            var imageBytes =await File.ReadAllBytesAsync(imagePath);
            var base64String = Convert.ToBase64String(imageBytes);
            var images = new List<string>() { base64String };
            ResponseTextBox.Text += $"\n 添加图片成功：{imagePath} \n ";
            ResponseTextBox.Text += $"当前模型：{semanticMode} 执行中........... \n \n";

            var chatMessageContents = await Task.Run(async () =>
            {
                var messages = await chat.Send("请识别这张图片的内容有什么？请用中文回答", images, CancellationToken.None);
                var last = messages.Last();
                var chatResponse = last.Content;

                return new List<ChatMessageContent> { new(AuthorRole.Assistant, chatResponse) };
            });

            var stringBuilder = new StringBuilder();
            foreach (var chatMessageContent in chatMessageContents)
            {
                stringBuilder.Append(chatMessageContent);
            }

            var result = $"{stringBuilder} \n";
            ResponseTextBox.Text += result;
            ResponseTextBox.ScrollToEnd();

        }

    }


    enum SemanticMode
    {
        Llama3,

        Phi3,

        Llava8B,

        Llava34B
    }
}