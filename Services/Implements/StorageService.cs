using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using FoodioAPI.Services;
namespace FoodioAPI.Services.Implements;

public class StorageService : IStorageService
{
    private readonly StorageClient _client;
    private readonly GoogleCredential _credential;

    private const string BUCKET_NAME = "alphachat-abcb6.appspot.com";
    private const string CREDENTIALS_PATH = "./netflex-storage-key.json";

    public StorageService()
    {
        _credential = GoogleCredential.FromFile(CREDENTIALS_PATH);
        _client = StorageClient.Create(_credential);
    }

    public async Task<Uri> UploadFileAsync(string fileName, IFormFile file)
    {
        var randomGuid = Guid.NewGuid();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var blob = await _client.UploadObjectAsync(BUCKET_NAME,
            $"{fileName}-{randomGuid}", file.ContentType, stream);

        return new Uri(blob.MediaLink);
    }
}