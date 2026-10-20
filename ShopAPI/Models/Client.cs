namespace ShopApi.Models;

// Клиент — строка таблицы clients
public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string City { get; set; } = "";
}
