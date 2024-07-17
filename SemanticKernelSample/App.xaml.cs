using System.Configuration;
using System.Data;
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
            builder.Services.AddSingleton<IChatCompletionService, OllamaChatCompletionService>();
            SkKernel = builder.Build();
        }
    }

}
