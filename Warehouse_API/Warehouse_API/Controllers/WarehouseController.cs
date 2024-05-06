using Microsoft.AspNetCore.Mvc;
using Warehouse_API.Repositories;

namespace Warehouse_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehousesRepository _warehousesRepository;

    public WarehouseController(IWarehousesRepository warehousesRepository)
    {
        _warehousesRepository = warehousesRepository;
    }

    [HttpPost]
    public async Task<IActionResult> AddProductToWarehouse(int IdProduct, int IdWarehouse, 
        int Amount, DateTime CreatedAt)
    {
        _warehousesRepository.AddProductToWarehouse()
    }
}