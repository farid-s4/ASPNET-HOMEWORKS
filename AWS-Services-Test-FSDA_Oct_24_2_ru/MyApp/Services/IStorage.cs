namespace MyApp.Services;

public interface IStorage
{
    Task<string> UploadFileAsync(IFormFile? file);
    Task DeleteFileByUrl(string? fileUrl);
}
