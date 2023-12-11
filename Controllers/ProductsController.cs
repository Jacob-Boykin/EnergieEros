using EnergieEros.Models;
using EnergieEros.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/products")]
[ApiController]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsApiController(IProductService productService)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();

        if (products == null || !products.Any() || products.Count() == 0)
        {
            return NotFound("No products found");
        }

        return products.ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        return product;
    }

    [Route("add")]
    [HttpPost]
    public async Task<ActionResult<Product>> AddProduct([FromBody] Product product)
    {
        if (product == null)
        {
            return BadRequest("Product is null");
        }

        await _productService.AddProductAsync(product);

        return CreatedAtAction(nameof(GetProductById), product);
    }

    [Route("update")]
    [HttpPut]
    public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product product)
    {
        if (product == null)
        {
            return BadRequest("Product is null");
        }

        await _productService.UpdateProductAsync(product);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);

        return NoContent();
    }
}