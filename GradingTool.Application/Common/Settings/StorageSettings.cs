namespace GradingTool.Application.Common.Settings;

public class StorageSettings
{
    public string BasePath { get; set; } = "Storage";
    public int MaxFileSizeMB { get; set; } = 50;
}
