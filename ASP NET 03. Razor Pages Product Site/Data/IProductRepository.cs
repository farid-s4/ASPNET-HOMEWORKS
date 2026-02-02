using ASP_NET_03._Razor_Pages_Product_Site.Models;
namespace ASP_NET_03._Razor_Pages_Product_Site.Data;

public interface IProductRepository
{
    public Task<IEnumerable<Product>> GetProductsAsync();
    public Task<Product> GetProductByIdAsync(int id);
    public Product AddProduct(Product product);
}
