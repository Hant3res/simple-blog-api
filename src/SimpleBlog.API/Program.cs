using Microsoft.EntityFrameworkCore;
using SimpleBlog.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настраиваем базу данных
// По умолчанию используем InMemory для тестов
if (builder.Environment.EnvironmentName == "Test" || 
    builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<BlogContext>(options =>
        options.UseInMemoryDatabase("TestBlog"));
}
else
{
    builder.Services.AddDbContext<BlogContext>(options =>
        options.UseSqlite("Data Source=blog.db"));
}

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

// Инициализируем базу данных (только если не тестовая среда)
if (!app.Environment.IsEnvironment("Test") && 
    !app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<BlogContext>();
        context.Database.EnsureCreated();
    }
}

app.Run();

// Для тестирования
public partial class Program { }
