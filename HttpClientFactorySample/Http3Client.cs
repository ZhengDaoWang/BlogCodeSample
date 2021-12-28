using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientFactorySample
{
    public class Http3Client
    {
        public HttpClient Client { get; private set; }

        public Http3Client(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://localhost:5001/WeatherForecast");
            httpClient.DefaultRequestVersion = HttpVersion.Version30;
            httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
            Client = httpClient;
        }

        public async Task<HttpResponseMessage> GetData()
        {
            return await Client.GetAsync("/");
        }
    }
}
