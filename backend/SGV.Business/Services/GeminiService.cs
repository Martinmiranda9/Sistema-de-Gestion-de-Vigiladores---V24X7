using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using SGV.Business.Interfaces;
using SGV.DTOs.Ocr;

namespace SGV.Business.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent";
        private const string SystemInstruction = "Sos un sistema experto en lectura óptica de planillas de control de asistencia y horarios. Tu única tarea es extraer datos y devolver JSON estructurado.";
        private const string OcrPrompt = @"Analizá esta planilla de control de horarios de vigilancia.
Extraé los datos de cabecera y el detalle de cada día.

Devolvé ESTRICTAMENTE solo un objeto JSON válido (sin markdown, sin bloques de código, sin texto adicional) con esta estructura exacta:
{
  ""month"": ""<mes como número 1-12 o nombre en español>"",
  ""year"": <año como número>,
  ""guardName"": ""<apellido y nombre del vigilador si se ve>"",
  ""workplace"": ""<nombre del objetivo o lugar si se ve>"",
  ""rows"": [
    {
      ""day"": <número de día>,
      ""entry"": ""<hora entrada en formato HH:mm, o vacío si no hay>"",
      ""exit"": ""<hora salida en formato HH:mm, o vacío si no hay>"",
      ""isDayOff"": <true si es franco/descanso, false si no>,
      ""notes"": ""<observaciones si las hay, o cadena vacía>""
    }
  ]
}

Si un campo no se puede leer con claridad, dejalo como vacío ("""") o null.
Los horarios deben estar en formato HH:mm de 24 horas.
No incluyas días que estén completamente en blanco si el mes tiene menos de 31 días.";

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiApiKey"];
        }

        public async Task<OcrAttendanceResultDto> ProcessAttendanceSheetAsync(Stream imageStream, string contentType)
        {
            if (string.IsNullOrEmpty(_apiKey))
                throw new Exception("Gemini API Key is not configured in the backend (GeminiApiKey).");

            string base64Image;
            using (var ms = new MemoryStream())
            {
                await imageStream.CopyToAsync(ms);
                base64Image = Convert.ToBase64String(ms.ToArray());
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new object[]
                        {
                            new { text = $"{SystemInstruction}\n\n{OcrPrompt}" },
                            new
                            {
                                inlineData = new
                                {
                                    mimeType = contentType,
                                    data = base64Image
                                }
                            }
                        }
                    }
                }
            };

            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var url = $"{ApiUrl}?key={_apiKey}";
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API error: {response.StatusCode}. Details: {errorBody}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseBody);
            
            var root = jsonDocument.RootElement;
            if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
            {
                var firstCandidate = candidates[0];
                if (firstCandidate.TryGetProperty("content", out var contentElement) && 
                    contentElement.TryGetProperty("parts", out var parts) && 
                    parts.GetArrayLength() > 0)
                {
                    var text = parts[0].GetProperty("text").GetString()?.Trim();
                    
                    if (string.IsNullOrEmpty(text))
                        throw new Exception("Gemini returned empty text.");

                    // Clean markdown blocks if any
                    if (text.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
                        text = text.Substring(7);
                    else if (text.StartsWith("```"))
                        text = text.Substring(3);

                    if (text.EndsWith("```"))
                        text = text.Substring(0, text.Length - 3);

                    text = text.Trim();

                    try
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var result = JsonSerializer.Deserialize<OcrAttendanceResultDto>(text, options);
                        if (result != null)
                            return result;
                    }
                    catch (JsonException ex)
                    {
                        throw new Exception($"Failed to parse JSON from Gemini. Exception: {ex.Message}. Response text: {text.Substring(0, Math.Min(300, text.Length))}");
                    }
                }
            }

            throw new Exception("Could not parse response structure from Gemini API.");
        }
    }
}
