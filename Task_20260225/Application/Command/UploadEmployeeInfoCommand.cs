namespace Task_20260225.Application.Command;

public class UploadEmployeeInfoCommand : ICommandBase
{
    public readonly IFormFile? File;
    public readonly string Text;

    public UploadEmployeeInfoCommand(IFormFile file)
    {
        File = file;
        Text = string.Empty;
    }

    public UploadEmployeeInfoCommand(string text)
    {
        Text = text;
        File = null;
    }

    public bool IsFileType()
    {
        return File != null;
    }
}
