using System;

namespace X.Application.Interfaces;

public interface IStorage
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteFileAsync(string fileUrl);
}