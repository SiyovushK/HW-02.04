using System.Data;
using System.Globalization;
using System.Net;
using Dapper;
using Domain.Entities;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Infrastructure;

namespace Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly DataContext _context;
    public ProductService(DataContext context)
    {
        _context = context;
    }

    public async Task<Response<List<Product>>> GetAllAsync()
    {
        using var connection = await _context.GetConnectionAsync();
        string sql = @"SELECT * FROM products";
        var result = await connection.QueryAsync<Product>(sql);

        return result == null
            ? new Response<List<Product>>(HttpStatusCode.NotFound, "Not found")
            : new Response<List<Product>>(result.ToList());
    }

    public async Task<Response<Product>> GetByIdAsync(int ID)
    {
        using var connection = await _context.GetConnectionAsync();
        string sql = @"SELECT * FROM products WHERE Id = @Id";
        var result = await connection.QueryFirstOrDefaultAsync<Product>(sql, new {Id = ID});

        return result == null
            ? new Response<Product>(HttpStatusCode.NotFound, "Not found")
            : new Response<Product>(result);
    }

    public async Task<Response<string>> CreateAsync(Product product)
    {
        using var connection = await _context.GetConnectionAsync();
        string sql = @"
            INSERT INTO products(Name, Description, Price, StockQuantity, CategoryName, CreatedDate)
            VALUES
            (@Name, @Description, @Price, @StockQuantity, @CategoryName, @CreatedDate)
        ";
        var result = await connection.ExecuteAsync(sql, product);

        return result == 0
            ? new Response<string>(HttpStatusCode.BadRequest, "Something went wrong")
            : new Response<string>("Product created successefully");
    }

    public async Task<Response<string>> UpdateAsync(Product product)
    {
        using var connection = await _context.GetConnectionAsync();
        string sql = @"
            UPDATE products
            SET 
                Name = @Name, 
                Description = @Description, 
                Price = @Price, 
                StockQuantity = @StockQuantity, 
                CategoryName = @CategoryName, 
                CreatedDate = @CreatedDate
            WHERE Id = @Id
        ";
        var result = await connection.ExecuteAsync(sql, product);

        return result == 0
            ? new Response<string>(HttpStatusCode.BadRequest, "Something went wrong")
            : new Response<string>("Product updated successefully");
    }

    public async Task<Response<string>> DeleteAsync(int ID)
    {
        using var connection = await _context.GetConnectionAsync();
        string sql = @"
            DELETE FROM Products
            WHERE Id = @Id
        ";
        var result = await connection.ExecuteAsync(sql, new {Id = ID});

        return result == 0
            ? new Response<string>(HttpStatusCode.BadRequest, "Something went wrong")
            : new Response<string>("Product deleted successefully");
    }

    public async Task<Response<string>> Export()
    {
        string Path = @"C:\Users\-\Desktop\HomeTask for 02.04\Export.txt";
        string sql = @"SELECT * FROM products";
        List<string> ProductList = new();
        
        if (!File.Exists(Path))
        {
            return new Response<string>(HttpStatusCode.NotFound, "File is not fount");
        }

        using var connection = await _context.GetConnectionAsync();
        var result = await connection.QueryAsync<Product>(sql);

        foreach (var item in result)
        {
            var product = @$"{item.Id},{item.Name},{item.Description},{item.Price},{item.StockQuantity},{item.CategoryName},{item.CreatedDate}";
            ProductList.Add(product);
        }

        await File.WriteAllLinesAsync(Path, ProductList);

        return new Response<string>("Export has been done successefully");
    }

    public async Task<Response<string>> Import()
    {
        string Path = @"C:\Users\-\Desktop\HomeTask for 02.04\add.txt";
        string sql = @"
            INSERT INTO products(Name, Description, Price, StockQuantity, CategoryName, CreatedDate)
            VALUES
            (@Name, @Description, @Price, @StockQuantity, @CategoryName, @CreatedDate)
        ";

        if (!File.Exists(Path))
        {
            return new Response<string>(HttpStatusCode.NotFound, "File not found");
        }

        var lines = await File.ReadAllLinesAsync(Path);

        using var connection = await _context.GetConnectionAsync();

        foreach (var line in lines)
        {
            var values = line.Split(",");
            var product = new Product
            {
                Name = values[0],
                Description = values[1],
                Price = decimal.Parse(values[2], CultureInfo.InvariantCulture),
                StockQuantity = int.Parse(values[3]),
                CategoryName = values[4],
                CreatedDate = DateTime.Parse(values[5])
            };

            var result = await connection.ExecuteAsync(sql, product);
        }

        return new Response<string>("Products successefully added");
    }

    public async Task<Response<string>> Edit()
    {
        string Path = @"C:\Users\-\Desktop\HomeTask for 02.04\edit.txt";
        string sql = @"
            UPDATE products
            SET 
                Name = @Name, 
                Description = @Description, 
                Price = @Price, 
                StockQuantity = @StockQuantity, 
                CategoryName = @CategoryName, 
                CreatedDate = @CreatedDate
            WHERE Id = @Id
        ";

        if (!File.Exists(Path))
        {
            return new Response<string>(HttpStatusCode.NotFound, "File not found");
        }

        var lines = await File.ReadAllLinesAsync(Path);

        using var connection = await _context.GetConnectionAsync();

        foreach (var line in lines)
        {
            var values = line.Split(",");
            var product = new Product
            {
                Id = int.Parse(values[0]),
                Name = values[1],
                Description = values[2],
                Price = decimal.Parse(values[3], CultureInfo.InvariantCulture),
                StockQuantity = int.Parse(values[4]),
                CategoryName = values[5],
                CreatedDate = DateTime.Parse(values[6])
            };
            
            var result = await connection.ExecuteAsync(sql, product);
        }

        return new Response<string>("Edited successefully");
    }

    public async Task<Response<string>> Statistics()
    {
        string Path = @"C:\Users\-\Desktop\HomeTask for 02.04\static.txt";
        string SQLTotalProducts = @"SELECT COUNT(*) FROM products";
        string SQLAveragePrice = @"SELECT AVG(Price) FROM products";
        string SQLTotalStock = @"SELECT SUM(StockQuantity) FROM products";

        if (!File.Exists(Path))
        {
            return new Response<string>(HttpStatusCode.NotFound, "NotFound");
        }

        using var connection = await _context.GetConnectionAsync();
        
        var TotalProducts = await connection.ExecuteScalarAsync(SQLTotalProducts);
        var AveragePrice = await connection.ExecuteScalarAsync(SQLAveragePrice);
        var TotalStock = await connection.ExecuteScalarAsync(SQLTotalStock);
        
        await File.WriteAllTextAsync(Path, $"Total products: {TotalProducts}\nAverage Price: {AveragePrice}\nTotal Stock Quantity: {TotalStock}");

        return new Response<string>("Statistics added");
    }

}