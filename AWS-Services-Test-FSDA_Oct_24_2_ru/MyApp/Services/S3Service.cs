
using Amazon.S3;
using Amazon.S3.Model;

namespace MyApp.Services;

public class S3Service : IStorage
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;

    public S3Service(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _configuration = configuration;
    }
    public async Task<string> UploadFileAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0) return null!;

        var bucketName = _configuration["AWS:BucketName"];
        var region = _configuration["AWS:Region"];
        if (string.IsNullOrWhiteSpace(bucketName))
            throw new InvalidOperationException("AWS S3 Bucket name is not configured");

        var key = $"products/{Guid.NewGuid()}_{file.FileName}";

        await using var stream = file.OpenReadStream();
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3Client.PutObjectAsync(request);

        return $"https://{bucketName}.s3.{region}.amazonaws.com/{key}";
    }

    public async Task DeleteFileByUrl(string? fileUrl)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return;

        var bucketName = _configuration["AWS:BucketName"];

        if (string.IsNullOrWhiteSpace(bucketName)) return;

        var region = _configuration["AWS:Region"];

        var marker = $"https://{bucketName}.s3.{region}.amazonaws.com/";
        var index = fileUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        if (index < 0) return;
        var key = fileUrl[(index + marker.Length)..];

        if (string.IsNullOrWhiteSpace(key)) return;

        var request = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        await _s3Client.DeleteObjectAsync(request);
    }
}