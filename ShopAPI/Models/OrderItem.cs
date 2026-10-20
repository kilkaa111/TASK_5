namespace ShopApi.Models;

// Строка заказа — строка таблицы order_items
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
}
