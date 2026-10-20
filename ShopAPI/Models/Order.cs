using System.Text.Json.Serialization;

namespace ShopApi.Models;

// Заказ — строка таблицы orders
public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public DateOnly OrderDate { get; set; }
    public string Status { get; set; } = "";

    // Строки этого заказа. Нужны, чтобы оформить заказ вместе со строками за один раз (задание 12).
    // [JsonIgnore] — в ответах API этого списка нет. Строки заказа отдаёт GET /orders/{id}/items
    [JsonIgnore]
    public List<OrderItem> Items { get; set; } = new();
}
