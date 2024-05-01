using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MinimalAPIsMovies.Services;

public class AzureFileStorage(IConfiguration configuration) : IFileStorage
{
    private readonly string? _connectionString = configuration.GetConnectionString("AzureStorage");

    /// <inheritdoc />
    public async Task<string> Store(string container, IFormFile file)
    {
        var client = new BlobContainerClient(_connectionString, container);
        await client.CreateIfNotExistsAsync();
        await client.SetAccessPolicyAsync(PublicAccessType.Blob);
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var blob = client.GetBlobClient(fileName);
        BlobHttpHeaders blobHttpHeaders = new() { ContentType = file.ContentType };
        await blob.UploadAsync(file.OpenReadStream(), blobHttpHeaders);

        return blob.Uri.ToString();
    }

    /// <inheritdoc />
    public async Task Delete(string? route, string container)
    {
        if (string.IsNullOrEmpty(route)) { return; }

        var client = new BlobContainerClient(_connectionString, container);
        await client.CreateIfNotExistsAsync();
        var fileName = Path.GetFileName(route);
        var blob = client.GetBlobClient(fileName);
        await blob.DeleteIfExistsAsync();
    }
}