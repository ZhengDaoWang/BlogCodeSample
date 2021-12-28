using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace HttpClientFactorySample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IHttpClientFactory _httpClientFactory;
        private Http3Client _http3Client;
        public MainWindow()
        {
            InitializeComponent();
            AppContext.SetSwitch("System.Net.SocketsHttpHandler.Http3Support", true);
            //FrameWork下支持配置无需返回证书验证信息
            //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            serviceCollection.AddHttpClient<Http3Client>();
            //serviceCollection.AddHttpClient<Http3Client>().ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler()
            //{
            //    SslOptions = new SslClientAuthenticationOptions()
            //    { RemoteCertificateValidationCallback = delegate { return true; } }
            //});

            var buildServiceProvider = serviceCollection.BuildServiceProvider();
            _httpClientFactory = buildServiceProvider.GetService<IHttpClientFactory>();
            _http3Client = buildServiceProvider.GetService<Http3Client>();
        }

        private async void HttpClient_OnClick(object sender, RoutedEventArgs e)
        {
            TextBox.Text = string.Empty;
            var stringBuilder = new StringBuilder(128);
            for (int i = 0; i < 10; i++)
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestVersion = HttpVersion.Version30;
                httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
                var result = await httpClient.GetAsync("https://localhost:5001/WeatherForecast");
                stringBuilder.AppendLine($"状态码：{result.StatusCode}, HTTP版本：{result.Version}");
            }
            TextBox.Text = stringBuilder.ToString();
        }

        private async void HttpClientFactory_OnClick(object sender, RoutedEventArgs e)
        {
            TextBox.Text = string.Empty;
            var stringBuilder = new StringBuilder(128);
            for (int i = 0; i < 10; i++)
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.GetAsync("http://172.20.114.55:49162/WeatherForecast");
                stringBuilder.AppendLine($"状态码：{result.StatusCode}");
            }
            TextBox.Text = stringBuilder.ToString();
        }

        private async void HttpClient3_OnClick(object sender, RoutedEventArgs e)
        {
            TextBox.Text = string.Empty;
            var stringBuilder = new StringBuilder(128);
            for (int i = 0; i < 10; i++)
            {
                var result = await _http3Client.GetData();
                stringBuilder.AppendLine($"状态码：{result.StatusCode}, HTTP版本：{result.Version}");
            }
            TextBox.Text = stringBuilder.ToString();
        }
    }
}
