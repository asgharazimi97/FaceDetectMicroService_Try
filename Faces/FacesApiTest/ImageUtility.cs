using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacesApiTest
{
    public class ImageUtility
    {
        public byte[] ConverToBytes(string imagePath)
        {
            MemoryStream ms = new MemoryStream();
            using(FileStream fs=new FileStream(imagePath, FileMode.Open))
            {
                fs.CopyTo(ms);
            }
            var bytes = ms.ToArray();
            return bytes;
        }

        public void BytesToImage(byte []imageByte, string fileName)
        {
            using(var ms=new MemoryStream(imageByte))
            {
                Image img = Image.FromStream(ms);
                img.Save(fileName + ".jpg", ImageFormat.Jpeg);
            }
        }
    }
}
