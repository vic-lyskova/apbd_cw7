using System.ComponentModel.DataAnnotations;

namespace Warehouse_API.Models.DTOs;

public class WarehouseDTO
{
    [Required]
    public int IdWarehouse { get; set; }
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string Adress { get; set; }
}