using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hant
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Grant grant = new Grant();
            using (FileStream fs = new FileStream("C:\\Users\\Andrey\\Desktop\\Hant\\info.txt", FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    grant.Description = sr.ReadToEnd();
                }
            }

            string apiUrl = "https://gptunnel.ru/v1/chat/completions"; // Укажите точный URL API GPTTunnel
            string apiKey = "shds-j16ljPGPoOEXROL0OpaPaJhytWk"; // Замените на ваш API-ключ GPTTunnel

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestPayload = new
            {
                model = "gpt-4-turbo",
                messages = new[]
                {
                new { role = "system", content = "Вы ассистент, помогаете анализировать заявки на гранты губернатора ХМАО." },
                new { role = "user", content = $"Этот грант необходимо проанализировать и дать ответ на русском {grant.Description}" }
            },
                max_tokens = 500,
                temperature = 0.7,
                top_p = 1.0
            };

            string jsonPayload = JsonConvert.SerializeObject(requestPayload);

            try
            {
                var response = await client.PostAsync(apiUrl, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                var responseObj = JsonConvert.DeserializeObject<GPT4TurboResponse>(responseContent);

                // Вывод содержимого
                Console.WriteLine($"Response ID: {responseObj.Id}");
                Console.WriteLine($"Created: {DateTimeOffset.FromUnixTimeSeconds(responseObj.Created)}");
                Console.WriteLine($"Model: {responseObj.Model}");
                Console.WriteLine($"Assistant Message: {responseObj.Choices[0].Message.Content}");
                Console.WriteLine($"Prompt Tokens: {responseObj.Usage.PromptTokens}");
                Console.WriteLine($"Total Cost: {responseObj.Usage.TotalCost:C2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(ex.Message);
            }
        }
    }
    public class GPT4TurboResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; }

        [JsonProperty("usage")]
        public Usage Usage { get; set; }

        [JsonProperty("system_fingerprint")]
        public string SystemFingerprint { get; set; }
    }

    public class Choice
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("logprobs")]
        public object Logprobs { get; set; }

        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; }
    }

    public class Message
    {
        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("refusal")]
        public object Refusal { get; set; }
    }

    public class Usage
    {
        [JsonProperty("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonProperty("total_tokens")]
        public int TotalTokens { get; set; }

        [JsonProperty("prompt_tokens_details")]
        public TokenDetails PromptTokensDetails { get; set; }

        [JsonProperty("completion_tokens_details")]
        public TokenDetails CompletionTokensDetails { get; set; }

        [JsonProperty("prompt_cost")]
        public double PromptCost { get; set; }

        [JsonProperty("completion_cost")]
        public double CompletionCost { get; set; }

        [JsonProperty("total_cost")]
        public double TotalCost { get; set; }
    }

    public class TokenDetails
    {
        [JsonProperty("cached_tokens")]
        public int CachedTokens { get; set; }

        [JsonProperty("audio_tokens")]
        public int AudioTokens { get; set; }

        [JsonProperty("reasoning_tokens")]
        public int ReasoningTokens { get; set; }

        [JsonProperty("accepted_prediction_tokens")]
        public int AcceptedPredictionTokens { get; set; }

        [JsonProperty("rejected_prediction_tokens")]
        public int RejectedPredictionTokens { get; set; }
    }
}
