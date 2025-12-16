using Microsoft.EntityFrameworkCore;
using SimpleBlog.API.Models;

namespace SimpleBlog.API.Data;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options)
    {
    }
    
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настраиваем отношения без циклических зависимостей
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne()
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Наполняем начальными данными
        modelBuilder.Entity<Post>().HasData(
            new Post { Id = 1, Title = "Добро пожаловать в блог!", Content = "Это первый пост в нашем блоге.", Author = "Админ", CreatedAt = DateTime.Now },
            new Post { Id = 2, Title = "О CI/CD пайпланах", Content = "Сегодня мы изучаем GitHub Actions.", Author = "Студент", CreatedAt = DateTime.Now }
        );
        
        modelBuilder.Entity<Comment>().HasData(
            new Comment { Id = 1, Author = "Читатель", Content = "Отличный пост!", PostId = 1, CreatedAt = DateTime.Now },
            new Comment { Id = 2, Author = "Разработчик", Content = "Полезная информация!", PostId = 1, CreatedAt = DateTime.Now }
        );
    }
}
