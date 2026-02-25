using System.Text.Json;
using Task_20260225.Command;
using Task_20260225.Models;
using Task_20260225.Services;
using Task_20260225.Utils;

namespace Task_20260225.Handlers.Commands;

public class UploadEmployeeInfoHandler : CommandHandler<int>
{
    public UploadEmployeeInfoHandler(UploadEmployeeInfoCommand command, ContactCacheService cacheService)
        : base(command, cacheService)
    {
    }

    public override async Task<int> HandleAsync()
    {
        if (_command is not UploadEmployeeInfoCommand command)
            throw new ServerException(-1, "Invalid Command Data");

        if (command.IsFileType() == false)
        {
            return await _HandleTextType(command.Text);
        }
        
        if(command.File == null)
            throw new ServerException(-1, "File is required");

        var file = command.File;
        var extension = Path.GetExtension(file.FileName);
        if (string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase))
            return await _HandleJsonType(command.File);

        if (string.Equals(extension, ".csv", StringComparison.OrdinalIgnoreCase))
            return await _HandleCsvType(command.File);

        throw new ServerException(-1, "Only .json or .csv files are supported");
    }

    private async Task<int> _HandleTextType(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ServerException(-1, "Text is required");

        var trimmed = text.TrimStart();
        if (trimmed.StartsWith("[") || trimmed.StartsWith("{"))
        {
            return _HandleJsonTextType(text);
        }

        return await _HandleCsvTextType(text);
    }

    private async Task<int> _HandleJsonType(IFormFile file)
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
            throw new ServerException(-1, "Invalid JSON format");
        }

        if (contacts is null || contacts.Count == 0)
            return 0;

        return _cacheService.AddContactList(contacts);
    }

    private async Task<int> _HandleCsvType(IFormFile file)
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
                throw new ServerException(-1, "Invalid CSV format");

            contacts.Add(new ContactModel
            {
                Name = columns[0],
                Email = columns[1],
                Phone = columns[2],
                Date = columns[3]
            });
        }

        if (contacts.Count == 0)
            return 0;

        return _cacheService.AddContactList(contacts);
    }

    private int _HandleJsonTextType(string text)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        try
        {
            var list = JsonSerializer.Deserialize<List<ContactModel>>(text, options);
            if (list is not null)
                return _cacheService.AddContactList(list);

            var single = JsonSerializer.Deserialize<ContactModel>(text, options);
            if (single is not null)
                return _cacheService.AddContactList([single]);
        }
        catch (JsonException)
        {
            throw new ServerException(-1, "Invalid JSON format");
        }

        return 0;
    }

    private async Task<int> _HandleCsvTextType(string text)
    {
        using var reader = new StringReader(text);
        var contacts = new List<ContactModel>();

        while (await reader.ReadLineAsync() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = line.Split(',', StringSplitOptions.TrimEntries);
            if (columns.Length < 4)
                throw new ServerException(-1, "Invalid CSV format");

            contacts.Add(new ContactModel
            {
                Name = columns[0],
                Email = columns[1],
                Phone = columns[2],
                Date = columns[3]
            });
        }

        if (contacts.Count == 0)
            return 0;

        return _cacheService.AddContactList(contacts);
    }

    protected override void _Dispose()
    {
    }

}
