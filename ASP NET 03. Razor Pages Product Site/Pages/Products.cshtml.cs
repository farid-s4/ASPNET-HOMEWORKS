using ASP_NET_03._Razor_Pages_Product_Site.Models;
using ASP_NET_03._Razor_Pages_Product_Site.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_03._Razor_Pages_Product_Site.Pages;

public class ProductsModel : PageModel
{
    private readonly ProductService _productService;
    public IEnumerable<Product> Products { get; set; } =
        Enumerable.Empty<Product>();

    public ProductsModel(ProductService productService)
    {
        _productService = productService;
    }

    public async Task OnGet()
    {
        Products = await _productService.GetProductsAsync();
    }
}
