namespace ShopApi.Data;

// Контекст базы: через него API читает и меняет таблицы.
// Каждое свойство DbSet — одна таблица: db.Products — это таблица products.
public class ShopContext(DbContextOptions<ShopContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
}
