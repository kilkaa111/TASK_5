namespace ShopApi.Controllers;

[ApiController]
[Route("clients")]
public class ClientsController(ShopContext db) : ControllerBase
{
    // GET /clients — все клиенты. GET /clients?city=Казань — только клиенты из этого города
    [HttpGet]
    public List<Client> GetAll(string? city)
    {
        var clients = db.Clients.AsQueryable();

        if (city != null)
            clients = clients.Where(c => c.City == city);

        return clients.OrderBy(c => c.Id).ToList();
    }

    // GET /clients/{id}/orders — заказы одного клиента, по возрастанию Id. Нет клиента — 404
    [HttpGet("{id}/orders")]
    public ActionResult<List<Order>> GetOrders(int id)
    {
        if (!db.Clients.Any(c => c.Id == id))
            return NotFound();

        return db.Orders
            .Where(o => o.ClientId == id)
            .OrderBy(o => o.Id)
            .ToList();
    }
}
