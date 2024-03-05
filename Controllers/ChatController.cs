using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenAI.API.Example.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private const string OpenAiApiKey = "{OpenAiApiKey}";

        [HttpGet]
        [Route("UseChatGPT")]
        public async Task<IActionResult> UseChatGPT(string query)
        {
            try
            {
                CompletionRequest completionRequest = new CompletionRequest
                {
                    Model = "gpt-3.5-turbo-instruct",
                    Prompt = query,
                    MaxTokens = 120
                };

                using (HttpClient httpClient = new HttpClient())
                {
                    var requestContent = new StringContent(JsonSerializer.Serialize(completionRequest), Encoding.UTF8, "application/json");
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {OpenAiApiKey}");

                    using (HttpResponseMessage httpResponse = await httpClient.PostAsync("https://api.openai.com/v1/completions", requestContent))
                    {
                        if (httpResponse.IsSuccessStatusCode)
                        {
                            string responseString = await httpResponse.Content.ReadAsStringAsync();

                            if (!string.IsNullOrWhiteSpace(responseString))
                            {
                                var completionResponse = JsonSerializer.Deserialize<CompletionResponse>(responseString);

                                if (completionResponse?.Choices != null && completionResponse.Choices.Count > 0)
                                {
                                    string completionText = completionResponse.Choices[0].Text;
                                    return Ok(completionText);
                                }
                                else
                                {
                                    return BadRequest("API response does not contain valid completion data.");
                                }
                            }
                            else
                            {
                                return BadRequest("API response is empty.");
                            }
                        }
                        else
                        {
                            return BadRequest($"API request failed with status code: {httpResponse.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the request.");
            }
        }
    }
}


public class CompletionRequest
{
    [JsonPropertyName("model")]
    public string? Model
    {
        get;
        set;
    }
    [JsonPropertyName("prompt")]
    public string? Prompt
    {
        get;
        set;
    }
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens
    {
        get;
        set;
    }
}

public class CompletionResponse
{
    [JsonPropertyName("choices")]
    public List<ChatGPTChoice>? Choices
    {
        get;
        set;
    }
    [JsonPropertyName("usage")]
    public ChatGPTUsage? Usage
    {
        get;
        set;
    }
}
public class ChatGPTUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens
    {
        get;
        set;
    }
    [JsonPropertyName("completion_token")]
    public int CompletionTokens
    {
        get;
        set;
    }
    [JsonPropertyName("total_tokens")]
    public int TotalTokens
    {
        get;
        set;
    }
}
[DebuggerDisplay("Text = {Text}")]
public class ChatGPTChoice
{
    [JsonPropertyName("text")]
    public string? Text
    {
        get;
        set;
    }
}


