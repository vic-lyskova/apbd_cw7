using System.ComponentModel.DataAnnotations;

namespace Warehouse_API.Models.DTOs;

public class ProductDTO
{
    [Required]
    public int IdProduct { get; set; }
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string Description { get; set; }
    public double Price { get; set; }
}