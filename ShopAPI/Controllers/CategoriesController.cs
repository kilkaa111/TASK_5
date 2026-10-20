namespace ShopApi.Controllers;

// Контроллер — класс, который отвечает на запросы по адресам /categories/...
// db — контекст базы, ASP.NET передаст его сам
[ApiController]
[Route("categories")]
public class CategoriesController(ShopContext db) : ControllerBase
{
    // GET /categories — все категории
    [HttpGet]
    public List<Category> GetAll()
    {
        return db.Categories.OrderBy(c => c.Id).ToList();
    }

    // GET /categories/{id}/products — товары одной категории, по возрастанию Id. Нет категории — 404
    [HttpGet("{id}/products")]
    public ActionResult<List<Product>> GetProducts(int id)
    {
        if (!db.Categories.Any(c => c.Id == id))
            return NotFound();

        return db.Products
            .Where(p => p.CategoryId == id)
            .OrderBy(p => p.Id)
            .ToList();
    }
}
