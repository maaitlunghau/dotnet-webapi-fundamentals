namespace _06_upload_many_file.Helpers;

public interface IFileStorage
{
    Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken token);
    Task DeleteFileAsync(string filePath, CancellationToken token);
}

public class LocalFileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private string baseFolder = "Storages";

    public LocalFileStorage(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }


    public async Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken token)
    {
        var uploads = Path.Combine(_environment.ContentRootPath, baseFolder, subFolder);
        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

        var fileName = $"{Guid.NewGuid().ToString()}-{file.FileName}";
        var fullPath = Path.Combine(uploads, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, token);

        var baseUrl = _configuration["App:BaseUrl"]
            ?? throw new ArgumentNullException(nameof(_configuration));

        var url = $"{baseUrl}/{baseFolder}/{subFolder}/{fileName}";
        return url;
    }

    public Task DeleteFileAsync(string filePath, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}