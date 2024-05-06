using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Warehouse_API.Repositories;

namespace Warehouse_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehousesRepository _warehousesRepository;
    private readonly IConfiguration _configuration;

    public WarehouseController(IWarehousesRepository warehousesRepository, IConfiguration configuration)
    {
        _warehousesRepository = warehousesRepository;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse(int IdProduct, int IdWarehouse, 
        int Amount, DateTime CreatedAt)
    {
        double price = await _warehousesRepository.DoesProductExist(IdProduct);
        if (price < 0)
        {
            return NotFound("Product doesn't exist");
        }

        if (!await _warehousesRepository.DoesWarehouseExist(IdWarehouse))
        {
            return NotFound("Warehouse doesn't exist");
        }

        if (Amount <= 0)
        {
            return BadRequest("Amount can't be below zero");
        }

        int idOrder = await _warehousesRepository.DoesOrderExist(IdProduct, Amount, CreatedAt);
        if (idOrder == -1)
        {
            return NotFound("Order doesn't exist");
        }

        if (!await _warehousesRepository.IsFulfilled(idOrder))
        {
            return BadRequest("Order is fulfilled");
        }

        await _warehousesRepository.UpdateFulfillment(idOrder);
        
        var query = "INSERT INTO Product_Warehouse VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdWarehouse", IdWarehouse);
        command.Parameters.AddWithValue("@IdProduct", IdProduct);
        command.Parameters.AddWithValue("@IdOrder", idOrder);
        command.Parameters.AddWithValue("@Amount", Amount);
        command.Parameters.AddWithValue("@Price", price*Amount);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Today.ToString());
        
        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();

        return NoContent();
    }
}