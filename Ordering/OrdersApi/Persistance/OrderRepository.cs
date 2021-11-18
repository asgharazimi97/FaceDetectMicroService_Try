using Microsoft.EntityFrameworkCore;
using OrdersApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Persistance
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersContext _context;

        public OrderRepository(OrdersContext ordersContext)
        {
            _context = ordersContext;
        }
        public Order GetOrder(Guid id)
        {
            return _context.Orders.Include(x => x.OrderDetails).FirstOrDefault();
        }

        public async Task<Order> GetOrderAsync(Guid orderId)
        {
            return await _context.Orders.Include(x => x.OrderDetails).FirstOrDefaultAsync(x => x.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public Task RegisterOrder(Order order)
        {
            _context.Add(order);
            _context.SaveChanges();
            return Task.FromResult(true);
        }

        public void UpdateOrder(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
