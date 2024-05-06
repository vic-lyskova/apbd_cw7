using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Warehouse_API.Models.DTOs;

namespace Warehouse_API.Repositories;

public class WarehousesRepository : IWarehousesRepository
{

    private readonly IConfiguration _configuration;

    public WarehousesRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesProductExist(int idProduct)
    {
        var query = "SELECT 1 FROM PRODUCT WHERE IdProduct = @IdProduct";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", idProduct);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<bool> DoesWarehouseExist(int idWarehouse)
    {
        var query = "SELECT 1 FROM WAREHOUSE WHERE IdWarehouse = @IdWarehouse";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<int> DoesOrderExist(int idProduct, int amount, DateTime toBeCreatedAt)
    {
        var query = "SELECT IdOrder, CreatedAt FROM [ORDER] WHERE IdProduct = @IdProduct AND Amount >= @Amount";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        command.Parameters.AddWithValue("@Amount", amount);

        await connection.OpenAsync();

        // var res = await command.ExecuteScalarAsync();
        var reader = await command.ExecuteReaderAsync();

        if (!reader.Read())
        {
            return -1;
        }

        var idOrderOrdinal = reader.GetOrdinal("IdOrder");
        var createdAtOrdinal = reader.GetOrdinal("CreatedAt");

        var idOrder = reader.GetInt32(idOrderOrdinal);
        var createdAtString = reader.GetString(createdAtOrdinal);
        
        reader.Close();

        DateTime createdAt = DateTime.Parse(createdAtString);
        if (createdAt.CompareTo(toBeCreatedAt) == -1)
        {
            return idOrder;
        }

        return -1;
    }

    public async Task<bool> IsFulfilled(int idOrder)
    {
        var query = "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task UpdateFulfillment(int idOrder)
    {
        var query = "UPDATE [Order] SET FulfilledAt = @CurrDate WHERE IdOrder = @IdOrder";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@CurrDate", DateTime.Today.ToString());
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }

    public async Task<IActionResult> AddProductToWarehouse(int idProduct, int idWarehouse, int amount, DateTime createdAt)
    {
        if (!await DoesProductExist(idProduct))
        {
            return NotFound("Product doesn't exist");
        }

        if (!await DoesWarehouseExist(idWarehouse))
        {
            return NotFound("Warehouse doesn't exist");
        }

        if (amount <= 0)
        {
            return BadRequest("Amount can't be below zero");
        }

        int idOrder = await DoesOrderExist(idProduct, amount, createdAt);
        if (idOrder == -1)
        {
            return NotFound("Order doesn't exist");
        }

        if (!await IsFulfilled(idOrder))
        {
            return BadRequest("Order is fulfilled");
        }
    }
}