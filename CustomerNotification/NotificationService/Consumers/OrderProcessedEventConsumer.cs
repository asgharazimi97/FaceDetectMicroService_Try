using MassTransit;
using Messaging.InterfacesConstants.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using EmailService;

namespace NotificationService.Consumers
{
    public class OrderProcessedEventConsumer : IConsumer<IOrderProcessedEvent>
    {
        private readonly IEmailSender _emailsender;

        public OrderProcessedEventConsumer(IEmailSender emailsender)
        {
            _emailsender = emailsender;
        }
        public async Task Consume(ConsumeContext<IOrderProcessedEvent> context)
        {
            var rootFolder = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
            var result = context.Message;
            var facesData = result.Faces;
            if (!result.Faces.Any())
                Console.Out.WriteLine("No face detected!");
            else
            {
                int j = 0;
                foreach (var face in result.Faces)
                {
                    MemoryStream ms = new MemoryStream(face);
                    var image = Image.FromStream(ms);
                    image.Save(rootFolder + "Image/face/" + j + ".jpg", ImageFormat.Jpeg);
                    j++;
                }
            }
            string[] mailAdress = { result.UserEmail };
            await _emailsender.SendEmailAsync(new Message (
                mailAdress,
                "your order: "+ result.OrderId,
                " From FaceAndFaces",
                facesData
            ));
            await context.Publish<IOrderDispatchedEvent>(new
            {
                OrderId = context.Message.OrderId,
                DispatchDateTime = DateTime.Now
            });
        }
    }
}
