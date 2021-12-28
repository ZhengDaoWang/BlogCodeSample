// See https://aka.ms/new-console-template for more information


using System.Net;

//WebClient
using var webClient = new WebClient();
webClient.DownloadProgressChanged += (sender, args) => Console.WriteLine($"进度是：{args.ProgressPercentage}%");
var webClientResult = await webClient.DownloadStringTaskAsync(new Uri("https://www.baidu.com/"));
Console.WriteLine(webClientResult);

//HttpWebRequest
var webRequest = (HttpWebRequest)WebRequest.Create("https://www.baidu.com/");
webRequest.Credentials = CredentialCache.DefaultCredentials;
var webResponse = (HttpWebResponse)webRequest.GetResponse();
var reader = new StreamReader(webResponse.GetResponseStream());
var webRequestResult = reader.ReadToEnd();
Console.WriteLine(webRequestResult);

//HttpClient
using var httpClient = new HttpClient();
var httpClientResult =await httpClient.GetStringAsync("https://www.baidu.com/");
Console.WriteLine(httpClientResult);


