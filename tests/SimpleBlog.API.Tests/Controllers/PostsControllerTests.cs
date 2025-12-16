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
    private readonly WebApplicationFactory<Program> _factory;

    public PostsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
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
        });
    }

    [Fact]
    public async Task GetPosts_ReturnsPosts()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/posts");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
        
        Assert.NotNull(posts);
        Assert.True(posts.Count >= 2); // Должны быть начальные данные
    }

    [Fact]
    public async Task GetPost_ExistingId_ReturnsPost()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/posts/1");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var post = await response.Content.ReadFromJsonAsync<Post>();
        
        Assert.NotNull(post);
        Assert.Equal("Добро пожаловать в блог!", post.Title);
    }

    [Fact]
    public async Task CreatePost_ValidPost_CreatesSuccessfully()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newPost = new { Title = "Новый пост", Content = "Содержание нового поста", Author = "Тест" };
        
        // Act
        var response = await client.PostAsJsonAsync("/api/posts", newPost);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var createdPost = await response.Content.ReadFromJsonAsync<Post>();
        
        Assert.NotNull(createdPost);
        Assert.Equal("Новый пост", createdPost.Title);
    }
}
