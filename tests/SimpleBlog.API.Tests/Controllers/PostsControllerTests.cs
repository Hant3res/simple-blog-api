using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleBlog.API.Data;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace SimpleBlog.API.Tests.Controllers;

public class PostsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly BlogContext _context;

    public PostsControllerTests(WebApplicationFactory<Program> factory)
    {
        // Создаем тестовую базу данных
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        _context.Database.EnsureCreated();
        
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPosts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/posts");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPost_ExistingId_ReturnsSuccess()
    {
        // Act
        var response = await _client.GetAsync("/api/posts/1");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_ReturnsCreated()
    {
        // Arrange
        var newPost = new 
        { 
            Title = "Тестовый пост", 
            Content = "Содержание тестового поста", 
            Author = "Тестер" 
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/posts", newPost);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
