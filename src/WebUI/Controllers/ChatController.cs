using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using CleanArchitecture.Domain.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace CleanArchitecture.WebUI.Controllers;
public class ChatController : ApiControllerBase
{
    public ChatController()
    {

    }

    [HttpPost("query-to-ollama")]
    public async Task<string> GetResponseOllamaGenerateAPI([FromQuery] string promptValue)
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



    [HttpGet("parse-protein-data-by-proteinName")]
    public async Task<ActionResult<List<ProteinByName>>> GetProteinDataByProteinName([FromQuery] string proteinName)
    {
        if (string.IsNullOrWhiteSpace(proteinName))
        {
            return BadRequest("Protein name cannot be null or empty.");
        }

        string apiUrl = $"https://rest.uniprot.org/uniprotkb/search?query={Uri.EscapeDataString(proteinName)}&format=json";


        using HttpClient client = new();
        var response = await client.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, $"Error from external API: {response.ReasonPhrase}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();

       
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var proteinData = JsonSerializer.Deserialize<ProteinResponse>(jsonResponse, options);

        if (proteinData?.Results == null || !proteinData.Results.Any())
        {
            return NotFound("No protein data found for the provided protein name.");
        }

        List<ProteinByName> importantDetails = new();
        // Transform results into the desired structure
        importantDetails = proteinData.Results
           .Select(r => new ProteinByName
           {
               UniProtID = r.PrimaryAccession,
               ProteinName = r.ProteinDescription?.RecommendedName?.FullName?.Value ?? "N/A",
               ScientificName = r.Organism?.ScientificName ?? "N/A",
               TaxonID = r.Organism?.TaxonId ?? 0,
               MolecularWeight = r.Sequence?.MolWeight ?? 0
           })
           .ToList();

        return Ok(importantDetails);

    }



    [HttpGet("extract-protein-data-by-uniProtId")]
    public async Task<ActionResult<EssentialProteinResponseById>> ExtractProteinDataByUniProtId(string uniProtId)
    {
        try
        {
            // Define the UniProt API URL
            string apiUrl = $"https://rest.uniprot.org/uniprotkb/{uniProtId}?format=json";

            // Make an HTTP GET request to the UniProt API
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            // Ensure a successful response
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Failed to retrieve data for UniProt ID {uniProtId}: {response.ReasonPhrase}");
            }

            // Read the response content as a string
            string jsonResponse = await response.Content.ReadAsStringAsync();


            // Deserialize the JSON response into a C# object
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ProteinResponseById proteinData = JsonSerializer.Deserialize<ProteinResponseById>(jsonResponse, options);

            // Extract important information
            var result = new EssentialProteinResponseById
            {
                PrimaryAccession = proteinData?.PrimaryAccession,
                ProteinName = proteinData?.ProteinDescription?.RecommendedName?.FullName?.Value,
                ScientificName = proteinData?.Organism?.ScientificName,
                CommonName = proteinData?.Organism?.CommonName,
                TaxonID = proteinData?.Organism?.TaxonId ?? 0,
                Functions = proteinData?.Comments
        ?.Where(c => c.CommentType == "FUNCTION")
        .SelectMany(c => c.Texts)
        .Select(t => t.Value)
        .ToList() is List<string> functionsList && functionsList.Any()
        ? string.Join("; ", functionsList) // Convert the list to a single string
        : "No functions available"
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


}


public class ResponseChunk
{
    public string Model { get; set; }
    public string CreatedAt { get; set; }
    public string Response { get; set; }
    public bool Done { get; set; }
}

public class ProteinResponseById
{
    public string PrimaryAccession { get; set; }
    public ProteinDescription ProteinDescription { get; set; }
    public Organism Organism { get; set; }
    public List<Comment> Comments { get; set; }
    public List<Keyword> Keywords { get; set; }
}

public class ProteinDescription
{
    public RecommendedName RecommendedName { get; set; }
}

public class RecommendedName
{
    public FullName FullName { get; set; }
}

public class FullName
{
    public string Value { get; set; }
}

public class Organism
{
    public string ScientificName { get; set; }
    public string CommonName { get; set; }
    public int TaxonId { get; set; }
}



public class Comment
{
    public string CommentType { get; set; }
    public List<Text> Texts { get; set; }
}

public class Text
{
    public string Value { get; set; }
}

public class Keyword
{
    public string Name { get; set; }
}



 public class ProteinByName
{
    public string UniProtID { get; set; }
    public string ProteinName { get; set; }
    public string ScientificName { get; set; }
    public int TaxonID { get; set; }
    public double MolecularWeight { get; set; }
}

public class EssentialProteinResponseById
{
    public string PrimaryAccession { get; set; }
    public string Functions { get; set; }
    public string ProteinName { get; set; }
    public string ScientificName { get; set; }
    public string CommonName { get; set; }
    public int TaxonID { get; set; }
}