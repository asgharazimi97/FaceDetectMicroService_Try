using MassTransit;
using Messaging.InterfacesConstants.Commands;
using Newtonsoft.Json;
using OrdersApi.Models;
using OrdersApi.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrdersApi.Messages.Consumers
{
    public class RegisterOrderCommandConsumer : IConsumer<IRegisterOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpClientFactory _httpClient;

        public RegisterOrderCommandConsumer(IOrderRepository orderRepository, IHttpClientFactory httpClient)
        {
            _orderRepository = orderRepository;
            _httpClient = httpClient;
        }
        public async Task Consume(ConsumeContext<IRegisterOrderCommand> context)
        {
            var result = context.Message;
            if (result.UserEmail != null && result.PicUrl != null && result.ImageData != null)
            {
                SaveOrder(result);
                var client = _httpClient.CreateClient();
                var orderDetailData =await GetFacesFromFaceAliAsync(client, result.ImageData, result.OrderId);
                List<byte[]> faces = orderDetailData.Item1;
                Guid orderId = orderDetailData.Item2;

                await SaveOrderDetail(orderId, faces);
            }
            //return Task.FromResult(true);
        }

        private async Task SaveOrderDetail(Guid orderId, List<byte[]> faces)
        {
            var order = await _orderRepository.GetOrderAsync(orderId);
            if (order != null)
            {
                order.Status = Status.Processed;
                foreach (var face in faces)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = orderId,
                        FaceData = face
                    };
                    order.OrderDetails.Add(orderDetail);
                }
                _orderRepository.UpdateOrder(order);
            }
        }

        private async Task<Tuple<List<byte[]>, Guid>> GetFacesFromFaceAliAsync(HttpClient client, byte[] imageData, Guid orderId)
        {
            var byteContent = new ByteArrayContent(imageData);
            Tuple<List<byte[]>, Guid> orderDetailData = null;
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            using (var response = await client.PostAsync("https://localhost:44337/api/faces/WithOrderId/" + orderId, byteContent))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                orderDetailData = JsonConvert.DeserializeObject<Tuple<List<byte[]>, Guid>>(apiResponse);
            }
            return orderDetailData;
        }

        private void SaveOrder(IRegisterOrderCommand result)
        {
            Order order = new Order
            {
                OrderId = result.OrderId,
                UserEmail = result.UserEmail,
                Status = Status.Registered,
                PictureUrl = result.PicUrl,
                ImageData = result.ImageData
            };
            _orderRepository.RegisterOrder(order);
        }
    }
}
