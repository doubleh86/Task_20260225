using Task_20260225.Command;
using Task_20260225.Common.Services;
using Task_20260225.Common.Utils;
using Task_20260225.Handlers.Commands;

namespace TaskTest;

[TestFixture]
public class UploadEmployeeHandlerTest
{
    [Test]
    public async Task HandleAsync_WithJsonText_AddsContacts()
    {
        var cache = new ContactCacheService();
        var json = await File.ReadAllTextAsync(GetTestDataPath("contact.json"));
        var command = new UploadEmployeeInfoCommand(json);
        using var sut = new UploadEmployeeInfoHandler(command, cache);

        var result = await sut.HandleAsync();

        Assert.That(result, Is.EqualTo(50));
        var contacts = cache.GetContactList(1, 10);
        Assert.That(contacts.Count, Is.EqualTo(10));
        Assert.That(contacts[0].Name, Is.EqualTo("김철수"));
    }

    [Test]
    public async Task HandleAsync_WithCsvText_AddsContacts()
    {
        var cache = new ContactCacheService();
        var csv = await File.ReadAllTextAsync(GetTestDataPath("contact.csv"));
        var command = new UploadEmployeeInfoCommand(csv);
        using var sut = new UploadEmployeeInfoHandler(command, cache);

        var result = await sut.HandleAsync();

        Assert.That(result, Is.EqualTo(50));
        var contacts = cache.GetContactList(1, 10);
        Assert.That(contacts.Count, Is.EqualTo(10));
        Assert.That(contacts[0].Date, Is.EqualTo("2025.09.12"));
    }

    [Test]
    public void HandleAsync_WithInvalidCsvText_ThrowsServerException()
    {
        var cache = new ContactCacheService();
        var invalidCsv = "Kim, kim@test.com";
        var command = new UploadEmployeeInfoCommand(invalidCsv);
        using var sut = new UploadEmployeeInfoHandler(command, cache);

        Assert.ThrowsAsync<ServerException>(async () => await sut.HandleAsync());
    }

    private static string GetTestDataPath(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, "TestData", fileName);
    }
}
