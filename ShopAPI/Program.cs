// День 5. API для ООО «Рога и Копыта» — уже готово и исправлено, менять ничего не нужно.
// Его нужно просто запустить (README, шаг 2), а сайт ShopSite будет к нему обращаться.
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    // Русские буквы в ответах — как есть, а не непонятными кодами
    .AddJsonOptions(o => o.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All));
builder.Services.AddSwaggerGen();

// CORS. Браузер по умолчанию не даёт странице с одного адреса читать ответы API с другого адреса.
// Здесь мы разрешаем это любой странице — иначе сайт из папки ShopSite не получит данные
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()));

// Подключение к базе shop. Строка подключения с паролем — в секретах пользователя (README, шаг 2).
// UseSnakeCaseNamingConvention: свойство CategoryId в C# — это столбец category_id в базе
builder.Services.AddDbContext<ShopContext>(o => o
    .UseNpgsql(builder.Configuration.GetConnectionString("Shop"))
    .UseSnakeCaseNamingConvention());

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();
app.Run();
