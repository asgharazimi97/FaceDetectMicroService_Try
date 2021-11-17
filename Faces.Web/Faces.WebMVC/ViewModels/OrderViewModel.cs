using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMVC.ViewModels
{
    public class OrderViewModel
    {
        public Guid OrderId { get; set; }
        public string UserEmail { get; set; }
        public IFormFile File { get; set; }
        public string ImageUrl { get; set; }
        public string StatusString { get; set; }
        public byte[] ImageData { get; set; }
    }
}
