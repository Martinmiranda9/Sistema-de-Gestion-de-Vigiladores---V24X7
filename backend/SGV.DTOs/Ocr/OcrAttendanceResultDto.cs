using System.Text.Json.Serialization;

namespace SGV.DTOs.Ocr
{
    public class OcrAttendanceResultDto
    {
        [JsonPropertyName("month")]
        public object? Month { get; set; } // Can be string or int according to prompt

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("guardName")]
        public string? GuardName { get; set; }

        [JsonPropertyName("workplace")]
        public string? Workplace { get; set; }

        [JsonPropertyName("rows")]
        public List<OcrRowDto> Rows { get; set; } = new();
    }

    public class OcrRowDto
    {
        [JsonPropertyName("day")]
        public int Day { get; set; }

        [JsonPropertyName("entry")]
        public string? Entry { get; set; }

        [JsonPropertyName("exit")]
        public string? Exit { get; set; }

        [JsonPropertyName("isDayOff")]
        public bool IsDayOff { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }
}
