using System.Data;
using Npgsql;

namespace Infrastructure.Data;

public class DataContext
{
    private const string ConnectionString = "Host=localhost; Username=postgres; Password=Fa1konm1; Database=App";
    public async Task<IDbConnection> GetConnectionAsync()
    {
        return new NpgsqlConnection(ConnectionString);
    }
}