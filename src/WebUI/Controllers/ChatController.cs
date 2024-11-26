using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.Controllers;
public class ChatController : ApiControllerBase
{
    public ChatController()
    {
       
    }

    [HttpPost("query")]
    public async Task<string> CallGenerateApi([FromQuery] string promptValue)
    {
        var url = "http://localhost:11434/api/generate";

        var payload = new
        {
            model = "llama3.2:1b",
            prompt = promptValue
        };

        // Serialize the payload to JSON
        var jsonPayload = JsonSerializer.Serialize(payload);

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        HttpClient httpClient = new();

        
        var response = await httpClient.PostAsync(url, content);

        
        response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync();

        using var streamReader = new StreamReader(responseStream);

        var fullResponse = new StringBuilder();

        string line;
        while ((line = await streamReader.ReadLineAsync()) != null)
        {
            try
            {
                // Deserialize the JSON line
                var json = JsonSerializer.Deserialize<ResponseChunk>
                    (line,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Allows case-insensitive property matching

                    });

                // Append the response value if it exists
                if (!string.IsNullOrEmpty(json?.Response))
                {
                    fullResponse.Append(json.Response);
                }

                // Break the loop if `done` is true
                if (json?.Done == true)
                {
                    break;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to parse JSON: {ex.Message}");
            }
        }

        return fullResponse.ToString();
    }
}


public class ResponseChunk
{
    public string Model { get; set; }
    public string CreatedAt { get; set; }
    public string Response { get; set; }
    public bool Done { get; set; }
}