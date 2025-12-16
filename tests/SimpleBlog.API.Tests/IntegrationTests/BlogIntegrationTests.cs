using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleBlog.API.Data;
using System.Net.Http.Json;
using Xunit;

namespace SimpleBlog.API.Tests.IntegrationTests;

public class BlogIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BlogIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FullFlow_CreatePostAndAddComment_WorksCorrectly()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act 1: Создаем пост
        var newPost = new { Title = "Интеграционный тест", Content = "Тестируем полный поток", Author = "Тестер" };
        var postResponse = await client.PostAsJsonAsync("/api/posts", newPost);

        postResponse.EnsureSuccessStatusCode();
        var createdPost = await postResponse.Content.ReadFromJsonAsync<Post>();
        
        // Act 2: Добавляем комментарий
        var newComment = new { Author = "Читатель", Content = "Отличный пост!" };
        var commentResponse = await client.PostAsJsonAsync($"/api/posts/{createdPost.Id}/comments", newComment);

        // Act 3: Получаем пост с комментариями
        var getResponse = await client.GetAsync($"/api/posts/{createdPost.Id}");
        var postWithComments = await getResponse.Content.ReadFromJsonAsync<Post>();

        // Assert
        Assert.NotNull(postWithComments);
        Assert.Single(postWithComments.Comments);
        Assert.Equal("Отличный пост!", postWithComments.Comments[0].Content);
    }
}
