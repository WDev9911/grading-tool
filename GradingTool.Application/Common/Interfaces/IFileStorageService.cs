namespace GradingTool.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveSubmissionFileAsync(int submissionId, Stream fileStream, string originalFileName);
    Task<List<string>> ExtractAndSaveImagesAsync(int submissionId, string docxFilePath);
    Task DeleteSubmissionFolderAsync(int submissionId);
    string GetAbsolutePath(string relativePath);
    bool FileExists(string relativePath);
    int GetImageCount(int submissionId);
    List<(string FileName, long SizeBytes)> GetImageFiles(int submissionId);
    Stream OpenImageFile(int submissionId, string fileName);
    Stream OpenDocxFile(int submissionId);
}
