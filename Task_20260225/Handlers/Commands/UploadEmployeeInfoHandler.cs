using System.Text.Json;
using Task_20260225.Application.Command;
using Task_20260225.Application.Handlers.Commands;
using Task_20260225.Common.Services;
using Task_20260225.Common.Utils;
using Task_20260225.Models;

namespace Task_20260225.Handlers.Commands;

public class UploadEmployeeInfoHandler : CommandHandler<(int successCount, int failedCount)>
{
    public UploadEmployeeInfoHandler(UploadEmployeeInfoCommand command, ContactCacheService cacheService, LoggerService loggerService)
        : base(command, cacheService, loggerService)
    {
    }

    public override async Task<(int successCount, int failedCount)> HandleAsync()
    {
        if (_command is not UploadEmployeeInfoCommand command)
            throw new ServerException(ErrorCode.InvalidCommand, "Invalid Command [UploadEmployeeInfoCommand]");

        if (command.IsFileType() == false)
        {
            return await _HandleTextType(command.Text);
        }
        
        if(command.File == null)
            throw new ServerException(ErrorCode.UploadEmployeeInfoFileIsNull, "File is required");

        var file = command.File;
        var extension = Path.GetExtension(file.FileName);
        if (string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase))
            return await _HandleJsonType(command.File);

        if (string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
            return await _HandleCsvType(command.File);

        throw new ServerException(ErrorCode.UploadEmployeeInfoWrongFileType, "Only .json or .csv files are supported");
    }

    private async Task<(int successCount, int failedCount)> _HandleTextType(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ServerException(ErrorCode.UploadEmployeeInfoTextEmpty, "Text is required");

        var trimmed = text.TrimStart();
        if (trimmed.StartsWith("[") || trimmed.StartsWith("{"))
        {
            return _HandleJsonTextType(text);
        }

        return await _HandleCsvTextType(text);
    }

    private async Task<(int successCount, int failedCount)> _HandleJsonType(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<ContactModel> contacts;

        try
        {
            contacts = await JsonSerializer.DeserializeAsync<List<ContactModel>>(stream, options);
        }
        catch (JsonException)
        {
            throw new ServerException(ErrorCode.UploadEmployeeInfoInvalidJson, "Invalid JSON format");
        }

        if (contacts is null || contacts.Count == 0)
            return (0, 0);

        var successCount = _cacheService.AddContactList(contacts, out var failedCount);
        return (successCount, failedCount);
    }

    private async Task<(int successCount, int failedCount)> _HandleCsvType(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        var contacts = new List<ContactModel>();

        while (await reader.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = line.Split(',', StringSplitOptions.TrimEntries);
            if (columns.Length < 4)
                throw new ServerException(ErrorCode.UploadEmployeeInfoInvalidCsv, "Invalid CSV format");

            contacts.Add(new ContactModel
            {
                Name = columns[0],
                Email = columns[1],
                Phone = columns[2],
                Date = columns[3]
            });
        }

        if (contacts.Count == 0)
            return (0, 0);

        var successCount = _cacheService.AddContactList(contacts, out var failedCount);
        return (successCount, failedCount);
    }

    private (int successCount, int failedCount) _HandleJsonTextType(string text)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        try
        {
            var list = JsonSerializer.Deserialize<List<ContactModel>>(text, options);
            if (list is not null)
            {
                var successCount = _cacheService.AddContactList(list, out var failedCount);
                return (successCount, failedCount);
            }

            var single = JsonSerializer.Deserialize<ContactModel>(text, options);
            if (single is not null)
            {
                var successCount = _cacheService.AddContactList([single], out var failedCount);
                return (successCount, failedCount);
            }
        }
        catch (JsonException)
        {
            throw new ServerException(ErrorCode.UploadEmployeeInfoInvalidJsonWithText, "Invalid JSON format");
        }

        return (0, 0);
    }

    private async Task<(int successCount, int failedCount)> _HandleCsvTextType(string text)
    {
        using var reader = new StringReader(text);
        var contacts = new List<ContactModel>();

        while (await reader.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = line.Split(',', StringSplitOptions.TrimEntries);
            if (columns.Length < 4)
                throw new ServerException(ErrorCode.UploadEmployeeInfoInvalidCsvWithText, "Invalid CSV format");

            contacts.Add(new ContactModel
            {
                Name = columns[0],
                Email = columns[1],
                Phone = columns[2],
                Date = columns[3]
            });
        }

        if (contacts.Count == 0)
            return (0, 0);

        var successCount = _cacheService.AddContactList(contacts, out var failedCount);
        return (successCount, failedCount);
    }

    protected override void _Dispose()
    {
    }

}
