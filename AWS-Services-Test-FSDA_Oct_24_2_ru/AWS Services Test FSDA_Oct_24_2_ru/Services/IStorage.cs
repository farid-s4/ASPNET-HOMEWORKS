namespace AWS_Services_Test_FSDA_Oct_24_2_ru.Services;

public interface IStorage
{
    Task<string> UploadFileAsync(IFormFile? file);
    Task DeleteFileByUrl(string? fileUrl);
}
