using Microsoft.AspNetCore.Mvc;
using Warehouse_API.Models.DTOs;

namespace Warehouse_API.Repositories;

public interface IWarehousesRepository
{
    Task<double> DoesProductExist(int idProduct);
    Task<bool> DoesWarehouseExist(int idWarehouse);
    Task<int> DoesOrderExist(int idProduct, int amount, DateTime toBeCreatedAt);
    Task<bool> IsFulfilled(int idOrder);
    Task UpdateFulfillment(int idOrder);
}