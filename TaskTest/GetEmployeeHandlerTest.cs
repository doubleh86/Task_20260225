using System.Text.Json;
using Task_20260225.Application.Queries;
using Task_20260225.Common.Services;
using Task_20260225.Common.Utils;
using Task_20260225.Handlers.Queries;
using Task_20260225.Models;

namespace TaskTest;

[TestFixture]
public class GetEmployeeHandlerTest
{
    [Test]
    public async Task HandleAsync_ReturnsPagedResponseWithTotalCountAndTotalPages()
    {
        var cache = new ContactCacheService();
        cache.AddContactList([
            new ContactModel { Name = "A", Email = "a@test.com", Phone = "010-0000-0001", Date = "2026.01.01" },
            new ContactModel { Name = "B", Email = "b@test.com", Phone = "010-0000-0002", Date = "2026.01.02" },
            new ContactModel { Name = "C", Email = "c@test.com", Phone = "010-0000-0003", Date = "2026.01.03" }
        ], out var _);

        var query = new GetEmployeeQuery(2, 2);
        using var sut = new GetEmployeeHandler(query, cache, null);

        var json = await sut.HandleAsync();
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.That(root.GetProperty("totalCount").GetInt32(), Is.EqualTo(3));
        Assert.That(root.GetProperty("totalPages").GetInt32(), Is.EqualTo(2));
        Assert.That(root.GetProperty("items").GetArrayLength(), Is.EqualTo(1));
    }

    [Test]
    public void HandleAsync_WithWrongPage_ThrowsServerException()
    {
        var cache = new ContactCacheService();
        var query = new GetEmployeeQuery(0, 10);
        using var sut = new GetEmployeeHandler(query, cache, null);

        var ex = Assert.ThrowsAsync<ServerException>(async () => await sut.HandleAsync());
        Assert.That(ex?.ResultCode, Is.EqualTo(ErrorCode.GetEmployeeWrongPageOrPageSize));
    }

    [Test]
    public async Task HandleAsync_ByName_ReturnsMatchedContactsCaseInsensitive()
    {
        var cache = new ContactCacheService();
        cache.AddContactList([
            new ContactModel { Name = "Alice", Email = "a1@test.com", Phone = "010-0000-0001", Date = "2026.01.01" },
            new ContactModel { Name = "alice", Email = "a2@test.com", Phone = "010-0000-0002", Date = "2026.01.02" },
            new ContactModel { Name = "Bob", Email = "b@test.com", Phone = "010-0000-0003", Date = "2026.01.03" }
        ], out var _);

        var query = new GetEmployeeByNameQuery("ALICE");
        using var sut = new GetEmployeeByNameHandler(query, cache, null);

        var json = await sut.HandleAsync();
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        Assert.That(root.ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(root.GetArrayLength(), Is.EqualTo(2));
    }

    [Test]
    public void HandleAsync_ByName_WithEmptyName_ThrowsServerException()
    {
        var cache = new ContactCacheService();
        var query = new GetEmployeeByNameQuery(" ");
        using var sut = new GetEmployeeByNameHandler(query, cache, null);

        var ex = Assert.ThrowsAsync<ServerException>(async () => await sut.HandleAsync());
        Assert.That(ex?.ResultCode, Is.EqualTo(ErrorCode.GetEmployeeByNameEmptyName));
    }
}
