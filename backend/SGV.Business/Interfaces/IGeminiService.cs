using SGV.DTOs.Ocr;

namespace SGV.Business.Interfaces
{
    public interface IGeminiService
    {
        Task<OcrAttendanceResultDto> ProcessAttendanceSheetAsync(Stream imageStream, string contentType);
    }
}
