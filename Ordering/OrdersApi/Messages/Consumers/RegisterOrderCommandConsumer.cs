using MassTransit;
using Messaging.InterfacesConstants.Commands;
using OrdersApi.Models;
using OrdersApi.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            var result = context.Message;
            if(result.UserEmail!=null && result.PicUrl!=null && result.ImageData!=null)
            {
                SaveOrder(result);
            }
            return Task.FromResult(true);
        }

        private void SaveOrder(IRegisterOrderCommand result)
        {
            Order order = new Order
            {
                OrderId=result.OrderId,
                UserEmail=result.UserEmail,
                Status=Status.Registered,
                PictureUrl=result.PicUrl,
                ImageData=result.ImageData
            };
            _orderRepository.RegisterOrder(order);
        }
    }
}
