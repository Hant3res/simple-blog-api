using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleBlog.API.Data;
using SimpleBlog.API.Models;
using System.Net.Http.Json;
using Xunit;

namespace SimpleBlog.API.Tests.Controllers;

public class PostsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PostsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Заменяем базу данных на InMemory для тестов
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<BlogContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                    
                services.AddDbContext<BlogContext>(options =>
                {
                    options.UseInMemoryDatabase("TestBlog");
                });
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetPosts_ReturnsOk()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/posts");
        
        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetPost_ExistingId_ReturnsOk()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/posts/1");
        
        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreatePost_ReturnsCreated()
    {
        // Arrange
        var newPost = new { 
            Title = "Новый пост", 
            Content = "Содержание нового поста", 
            Author = "Тест" 
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/posts", newPost);
        
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    }
}
