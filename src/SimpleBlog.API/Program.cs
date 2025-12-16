using Microsoft.EntityFrameworkCore;
using SimpleBlog.API.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы в контейнер
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настраиваем базу данных
builder.Services.AddDbContext<BlogContext>(options =>
{
    // Всегда используем InMemory для простоты тестирования
    options.UseInMemoryDatabase("BlogDb");
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
public partial class Program { }
