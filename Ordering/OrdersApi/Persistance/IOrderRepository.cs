using OrdersApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Persistance
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderAsync(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task RegisterOrder(Order order);

        Order GetOrder(Guid id);
        void UpdateOrder(Order order);
    }
}
