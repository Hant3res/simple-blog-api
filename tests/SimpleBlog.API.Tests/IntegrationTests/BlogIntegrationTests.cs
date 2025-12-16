using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleBlog.API.Data;
using SimpleBlog.API.Models;
using System.Net.Http.Json;
using Xunit;

namespace SimpleBlog.API.Tests.IntegrationTests;

public class BlogIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly BlogContext _context;

    public BlogIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        _context.Database.EnsureCreated();
        
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatePost_And_GetPost_Works()
    {
        // Arrange
        var newPost = new 
        { 
            Title = "Интеграционный тест", 
            Content = "Тестируем создание поста", 
            Author = "Тестер" 
        };
        
        // Act 1: Создаем пост
        var postResponse = await _client.PostAsJsonAsync("/api/posts", newPost);
        postResponse.EnsureSuccessStatusCode();
        
        // Act 2: Получаем все посты
        var getResponse = await _client.GetAsync("/api/posts");
        getResponse.EnsureSuccessStatusCode();
        
        var posts = await getResponse.Content.ReadFromJsonAsync<List<Post>>();
        
        // Assert
        Assert.NotNull(posts);
        Assert.True(posts.Count > 0);
    }
}
