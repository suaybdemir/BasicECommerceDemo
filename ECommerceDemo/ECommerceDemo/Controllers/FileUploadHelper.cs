using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

public static class FileUploadHelper
{
    public static async Task<string> UploadFileAsync(IFormFile file, string uploadDirectory)
    {
        if (file != null && file.Length > 0)
        {
            // Dosya adı ve yolu
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadDirectory, fileName);

            // Dosyayı kaydet
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Yüklenen dosyanın URL'ini döndür
            return filePath; // Eğer sadece dosya adı isteniyorsa Path.GetFileName(filePath) kullanılabilir
        }

        return string.Empty;
    }
}
