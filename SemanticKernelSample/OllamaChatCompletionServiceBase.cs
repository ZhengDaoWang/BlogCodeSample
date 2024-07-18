using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;
using OllamaSharp.Models;

namespace SemanticKernelSample
{
    public abstract class OllamaChatCompletionServiceBase : IOllamaChatCompletionService
    {
        public IReadOnlyDictionary<string, object?>? Attributes { get; }

        protected readonly OllamaApiClient OllamaClient;

        protected OllamaChatCompletionServiceBase(string modelName)
        {
            OllamaClient=new("http://localhost:11434", modelName);
        }

        public virtual async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var chat = new Chat(OllamaClient, _ => { });

            var lastMessage = chatHistory.LastOrDefault();
            if (lastMessage is null || string.IsNullOrEmpty(lastMessage.Content))
            {
                return default;
            }

            string question = lastMessage.Content;
            var chatResponse = "";

            var chatMessageContents = await Task.Run(async () =>
            {
                var messages = await chat.Send(question, CancellationToken.None).ConfigureAwait(false);
                var last = messages.Last();
                chatResponse = last.Content;

                return new List<ChatMessageContent> { new(AuthorRole.Assistant, chatResponse) };
            });

            return chatMessageContents;
        }

        public virtual async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null, Kernel? kernel = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = new CancellationToken())
        {
            var channel = Channel.CreateBounded<GenerateCompletionResponseStream>(new BoundedChannelOptions(100)
            {
                // 如果满了就等待
                FullMode = BoundedChannelFullMode.Wait,
                SingleWriter = true,
                SingleReader = true
            });


            var lastMessage = chatHistory.LastOrDefault();
            string question = lastMessage?.Content!;
            ConversationContext? context = null;
            _ = Task.Run(async () =>
            {
                await foreach (var stream in OllamaClient.StreamCompletion(question, context, cancellationToken))
                {
                    await channel.Writer.WriteAsync(stream!, cancellationToken);
                }
            },cancellationToken);

            while (channel.Reader.CanPeek)
            {
                // 从下载队列拉取结果
                var stream = await channel.Reader.ReadAsync(cancellationToken);
                yield return new StreamingChatMessageContent(AuthorRole.Assistant, stream.Response);
            }
        }
    }
}
