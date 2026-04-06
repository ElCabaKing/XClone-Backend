using CloudinaryDotNet;

using CloudinaryDotNet.Actions;
using X.Application.Interfaces;

namespace X.Infrastructure.Services;

public class ImageStorage : IStorage
{
    private Cloudinary _cloudinary;
    public ImageStorage(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        throw new NotImplementedException();
    }
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream)
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        {
            return uploadResult.SecureUrl.ToString();
        }
        else
        {
            throw new Exception($"Cloudinary image upload failed: {uploadResult.Error?.Message}");
        }
    }
}