namespace frontend.Helpers;

public class FileUpload
{
    private static readonly string[] AllowedExtensions = {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".webp"
    };
    private static readonly string[] AllowedMimeTypes = {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    };
    private const long MaxFileSize = 5 * 1024 * 1024;

    public static async Task<string> SaveImage(
        IWebHostEnvironment env,
        string subFolder,
        IFormFile formFile
    )
    {
        if (formFile.Length > MaxFileSize)
            throw new InvalidOperationException($"File không được vượt quá {MaxFileSize / 1024 / 1024}MB");

        var extension = Path.GetExtension(formFile.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Chỉ chấp nhận file ảnh (.jpg, .png, .gif, .webp)");

        if (!AllowedMimeTypes.Contains(formFile.ContentType.ToLowerInvariant()))
            throw new InvalidOperationException("MIME type không hợp lệ");

        var originalFileName = Path.GetFileName(formFile.FileName);
        var safeFileName = string.Concat(originalFileName.Split(Path.GetInvalidFileNameChars()));
        var fileName = $"{Guid.NewGuid()}_{safeFileName}";

        var imagePath = Path.Combine(env.WebRootPath, subFolder);
        if (!Directory.Exists(imagePath))
        {
            Directory.CreateDirectory(imagePath);
        }

        var exactPath = Path.Combine(imagePath, fileName);
        try
        {
            using (var fileStream = new FileStream(exactPath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"Lỗi khi lưu file: {ex.Message}");
        }

        return Path.Combine(subFolder, fileName).Replace("\\", "/");
    }

    public static void DeleteImage(
        IWebHostEnvironment env,
        string imagePath
    )
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return;

        var exactPath = Path.Combine(env.WebRootPath, imagePath);
        if (File.Exists(exactPath))
        {
            try
            {
                File.Delete(exactPath);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException($"Lỗi khi xóa file: {ex.Message}");
            }
        }
    }
}
