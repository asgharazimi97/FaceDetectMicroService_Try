using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FacesApiTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var imagePath = @"oscars-2017.jpg";
            var url = "https://localhost:44337/api/faces/NoInput";
            ImageUtility imgUtil = new ImageUtility();
            var bytes = imgUtil.ConverToBytes(imagePath);
            List<byte[]> faceList = null;
            var byteContent = new ByteArrayContent(bytes);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            using (var httpclient=new HttpClient())
            {
                using (var response = await httpclient.PostAsync(url, byteContent)) 
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    faceList = JsonConvert.DeserializeObject<List<byte[]>>(apiResponse);
                }
            }
            if(faceList.Count>0)
            {
                for (int i = 0; i < faceList.Count; i++)
                {
                    imgUtil.BytesToImage(faceList[i], "face" + i);
                }
                
            }
            Console.WriteLine("Hello World!");
        }
    }
}
