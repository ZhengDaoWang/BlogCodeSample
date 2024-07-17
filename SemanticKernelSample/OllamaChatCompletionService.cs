using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OllamaSharp;

namespace SemanticKernelSample
{
    public class OllamaChatCompletionService : IChatCompletionService
    {
        public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

        public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            var ollama = new OllamaApiClient("http://localhost:11434", "llama3");

            var chat = new Chat(ollama, _ => { });

            var lastMessage = chatHistory.LastOrDefault();

            string question = lastMessage.Content;
            var chatResponse = "";
            var history = (await chat.Send(question, CancellationToken.None)).ToArray();

            var last = history.Last();
            chatResponse = last.Content;
        
            return new List<ChatMessageContent> { new ChatMessageContent(AuthorRole.Assistant, chatResponse) };
        }


        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
