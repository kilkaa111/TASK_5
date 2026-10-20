namespace ShopApi.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(ShopContext db) : ControllerBase
{
    // GET /orders — все заказы, по возрастанию Id
    [HttpGet]
    public List<Order> GetAll()
    {
        return db.Orders.OrderBy(o => o.Id).ToList();
    }

    // GET /orders/{id}/items — строки одного заказа, по возрастанию Id строки. Нет заказа — 404
    [HttpGet("{id}/items")]
    public ActionResult<List<OrderItem>> GetItems(int id)
    {
        if (!db.Orders.Any(o => o.Id == id))
            return NotFound();

        return db.OrderItems
            .Where(i => i.OrderId == id)
            .OrderBy(i => i.Id)
            .ToList();
    }

    // GET /orders/{id}/total — сумма заказа: количество × цена по всем строкам. Нет заказа — 404
    [HttpGet("{id}/total")]
    public ActionResult<int> GetTotal(int id)
    {
        if (!db.Orders.Any(o => o.Id == id))
            return NotFound();

        return db.OrderItems
            .Where(i => i.OrderId == id)
            .Sum(i => i.Quantity * i.Price);
    }

    // GET /orders/{id} — один заказ. Нет заказа — 404
    [HttpGet("{id}")]
    public ActionResult<Order> Get(int id)
    {
        var order = db.Orders.Find(id);
        if (order == null)
            return NotFound();
        return order;
    }

    // PUT /orders/{id}/status?status=Оплачен — сменить статус заказа.
    // Допустимы только статусы: Новый, Оплачен, Доставлен, Отменён. Другой статус — 400. Нет заказа — 404
    [HttpPut("{id}/status")]
    public ActionResult<Order> SetStatus(int id, string status)
    {
        var order = db.Orders.Find(id);
        if (order == null)
            return NotFound();

        string[] statuses = ["Новый", "Оплачен", "Доставлен", "Отменён"];
        if (!statuses.Contains(status))
            return BadRequest("Нет такого статуса");

        order.Status = status;
        db.SaveChanges();
        return order;
    }

    // POST /orders?clientId=11 — оформить заказ.
    // Тело — список строк: [{"productId": 2, "quantity": 5}, {"productId": 14, "quantity": 1}]
    // Цена строки — текущая цена товара за штуку. Остаток товара уменьшается на заказанное количество.
    // Если в запросе что-то не так — 400, и база не меняется.
    [HttpPost]
    public ActionResult<Order> Create(int clientId, List<OrderItem> items)
    {
        if (items.Count == 0)
            return BadRequest("В заказе нет товаров");
        if (!db.Clients.Any(c => c.Id == clientId))
            return BadRequest("Нет такого клиента");

        var order = new Order
        {
            ClientId = clientId,
            OrderDate = DateOnly.FromDateTime(DateTime.Today),
            Status = "Новый",
        };
        db.Orders.Add(order);

        foreach (var line in items)
        {
            var product = db.Products.Find(line.ProductId);
            if (product == null)
                return BadRequest($"Нет товара с номером {line.ProductId}");
            if (line.Quantity <= 0)
                return BadRequest($"Количество товара «{product.Name}» должно быть больше нуля");
            if (product.Stock < line.Quantity)
                return BadRequest($"Не хватает товара «{product.Name}»");

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = line.Quantity,
                Price = product.Price,
            });
            product.Stock -= line.Quantity;
        }

        db.SaveChanges();
        return Created($"/orders/{order.Id}", order);
    }
}
