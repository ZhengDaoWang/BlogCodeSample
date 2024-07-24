using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using OllamaSharp.Models.Chat;
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
            var semanticMode = GetSemanticMode();
            var client = LoadCustomMode(semanticMode);

            var chat = new Chat(client, _ => { });

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


            byte[] imageBytes = File.ReadAllBytes(imagePath);
            // Convert the bytes to a Base64 string
            string base64String = Convert.ToBase64String(imageBytes);
            var images = new List<string>() { base64String };
            ResponseTextBox.Text += $"\n 添加图片成功：{imagePath} \n ";
            ResponseTextBox.Text += " 执行中........... \n \n";

            var chatMessageContents = await Task.Run(async () =>
            {
                var messages = await chat.Send("请识别这张图片的内容有什么？请用中文回答", images, CancellationToken.None).ConfigureAwait(false);
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

        //private void ExecuteLLaMA()
        //{
        //    string modelPath = @"<Your Model Path>"; // change it to your own model path.

        //    var parameters = new ModelParams(modelPath)
        //    {
        //        ContextSize = 1024, // The longest length of chat as memory.
        //        GpuLayerCount = 5 // How many layers to offload to GPU. Please adjust it according to your GPU memory.
        //    };
        //    using var model = LLamaWeights.LoadFromFile(parameters);
        //    using var context = model.CreateContext(parameters);
        //    var executor = new InteractiveExecutor(context);

        //    // Add chat histories as prompt to tell AI how to act.
        //    var chatHistory = new ChatHistory();
        //    chatHistory.AddMessage(AuthorRole.System, "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.");
        //    chatHistory.AddMessage(AuthorRole.User, "Hello, Bob.");
        //    chatHistory.AddMessage(AuthorRole.Assistant, "Hello. How may I help you today?");

        //    ChatSession session = new(executor, chatHistory);

        //    InferenceParams inferenceParams = new InferenceParams()
        //    {
        //        MaxTokens = 256, // No more than 256 tokens should appear in answer. Remove it if antiprompt is enough for control.
        //        AntiPrompts = new List<string> { "User:" } // Stop generation once antiprompts appear.
        //    };
        //}

    }


    enum SemanticMode
    {
        Llama3,

        Phi3,

        Llava8B,

        Llava34B
    }
}