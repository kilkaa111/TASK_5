namespace ShopApi.Models;

// Товар — строка таблицы products
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int CategoryId { get; set; }
    public int Price { get; set; }
    public int Stock { get; set; }
}
