using System.ComponentModel.DataAnnotations;

namespace Warehouse_API.Models.DTOs;

public class OrderDTO
{
    public int IdOrder { get; set; }
    public ProductDTO Product { get; set; }
    [Required]
    public int Amount { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
    public DateTime FulfilledAt { get; set; }
}