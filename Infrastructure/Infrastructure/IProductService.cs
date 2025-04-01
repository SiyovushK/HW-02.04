using Domain.Entities;
using Domain.Responses;

namespace Infrastructure.Infrastructure;

public interface IProductService
{
    Task<Response<List<Product>>> GetAllAsync();
    Task<Response<Product>> GetByIdAsync(int ID);
    Task<Response<string>> CreateAsync(Product product);
    Task<Response<string>> UpdateAsync(Product product);
    Task<Response<string>> DeleteAsync(int ID);
    Task<Response<string>> Export();
    Task<Response<string>> Import();
    Task<Response<string>> Edit();
    Task<Response<string>> Statistics();
}