namespace ShopApi.Controllers;

[ApiController]
[Route("products")]
public class ProductsController(ShopContext db) : ControllerBase
{
    // GET /products — все товары, по возрастанию Id
    [HttpGet]
    public List<Product> GetAll()
    {
        return db.Products.OrderBy(p => p.Id).ToList();
    }

    // GET /products/{id} — один товар. Нет товара — 404
    [HttpGet("{id}")]
    public ActionResult<Product> Get(int id)
    {
        var product = db.Products.Find(id);
        if (product == null)
            return NotFound();
        return product;
    }

    // GET /products/cheap?max=500 — товары, которые есть на складе и стоят не дороже max, от дешёвых к дорогим
    [HttpGet("cheap")]
    public List<Product> GetCheap(int max)
    {
        return db.Products
            .Where(p => p.Stock > 0 && p.Price <= max)
            .OrderBy(p => p.Price)
            .ToList();
    }

    // Проверка товара перед сохранением. Возвращает текст ошибки или null, если всё хорошо.
    private string? Check(Product p)
    {
        if (string.IsNullOrWhiteSpace(p.Name)) return "Название не может быть пустым";
        if (p.Price <= 0) return "Цена должна быть больше нуля";
        if (p.Stock < 0) return "Остаток не может быть меньше нуля";
        if (!db.Categories.Any(c => c.Id == p.CategoryId)) return "Нет такой категории";

        return null;
    }

    // POST /products — добавить товар. Ответ — 201 и созданный товар с его id. Ошибка в данных — 400
    [HttpPost]
    public ActionResult<Product> Create(Product p)
    {
        var error = Check(p);
        if (error != null)
            return BadRequest(error);

        p.Id = 0; // id выдаёт база
        db.Products.Add(p);
        db.SaveChanges();
        return Created($"/products/{p.Id}", p);
    }

    // PUT /products/{id} — изменить товар. Нет товара — 404, ошибка в данных — 400
    [HttpPut("{id}")]
    public ActionResult<Product> Update(int id, Product input)
    {
        var product = db.Products.Find(id);
        if (product == null)
            return NotFound();

        var error = Check(input);
        if (error != null)
            return BadRequest(error);

        product.Name = input.Name;
        product.CategoryId = input.CategoryId;
        product.Price = input.Price;
        product.Stock = input.Stock;

        db.SaveChanges();
        return product;
    }

    // DELETE /products/{id} — удалить товар. Ответ — 204, нет товара — 404.
    // Товар, который есть в заказах, удалять нельзя — 409
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var product = db.Products.Find(id);
        if (product == null)
            return NotFound();
        if (db.OrderItems.Any(i => i.ProductId == id))
            return Conflict("Товар есть в заказах, удалить нельзя");

        db.Products.Remove(product);
        db.SaveChanges();
        return NoContent();
    }
}
