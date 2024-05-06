using System.ComponentModel.DataAnnotations;

namespace Warehouse_API.Models.DTOs;

public class ProductWarehouseDTO
{
    public int IdProductWarehouse { get; set; }
    public WarehouseDTO Warehouse { get; set; }
    public ProductDTO Product { get; set; }
    public OrderDTO Order { get; set; }
    [Required]
    public int Amount { get; set; }
    public double Price { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
}
