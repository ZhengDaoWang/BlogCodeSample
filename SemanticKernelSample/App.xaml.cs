using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernelSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Kernel SkKernel { get; set; } = null!;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = Kernel.CreateBuilder();
            builder.Services.AddKeyedSingleton<IChatCompletionService, Llama3OllamaChatCompletionService>("llama3");
            builder.Services.AddKeyedSingleton<IChatCompletionService, Phi3OllamaChatCompletionService>("phi3");
            builder.Services.AddKeyedSingleton<IChatCompletionService, LlavaOllamaChatCompletionService>("llava");
            SkKernel = builder.Build();
        }
    }

}
