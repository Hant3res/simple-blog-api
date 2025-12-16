using Microsoft.EntityFrameworkCore;
using SimpleBlog.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настраиваем базу данных
builder.Services.AddDbContext<BlogContext>(options =>
{
    // Для разработки используем SQLite, для тестов - InMemory
    if (builder.Environment.IsEnvironment("Testing"))
    {
        options.UseInMemoryDatabase("TestBlog");
    }
    else
    {
        options.UseSqlite("Data Source=blog.db");
    }
});

var app = builder.Build();

// Настраиваем конвейер HTTP запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Инициализируем базу данных
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
    context.Database.EnsureCreated();
}

app.Run();

// Для тестирования
public partial class Program {}
