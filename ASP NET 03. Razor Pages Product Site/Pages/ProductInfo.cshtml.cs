using ASP_NET_03._Razor_Pages_Product_Site.Models;
using ASP_NET_03._Razor_Pages_Product_Site.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP_NET_03._Razor_Pages_Product_Site.Pages;

public class ProductInfo : PageModel
{
    private readonly ProductService _productService;
    
    public Product? Product { get; set; }

    public ProductInfo(ProductService productService)
    {
        _productService = productService;
    }
    
    public async Task OnGet(int id)
    {
        var product = await _productService.GetProductById(id);
        Product = product;
    }
}