using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils
{
    public class ImageServices
    {
        public static string Base64StringToImage(string subdir, string imgvalue, string id = "")
        {
            try
            {
                string[] results = imgvalue.Split(new string[] { ";base64," }, StringSplitOptions.RemoveEmptyEntries);
                var fileType = results[0].Replace("data:image/", "");
                var strbase64 = results[1];
                byte[] arr = Convert.FromBase64String(strbase64);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                string fileDir = string.Concat(ConfigurationManager.AppSettings.Get("netfilerootdir"), subdir, @"\");
                if (!string.IsNullOrEmpty(id))
                {

                    fileDir = string.Concat(fileDir, id, @"\");
                }
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                string fileName = string.Concat(Guid.NewGuid().ToString("N"), ".", fileType);
                string filepath = string.Concat(fileDir, fileName);
                switch (fileType.ToLower())
                {
                    case "png":
                        bmp.Save(filepath, ImageFormat.Png);
                        break;
                    case "jpg":
                        bmp.Save(filepath, ImageFormat.Jpeg);
                        break;
                    case "jpeg":
                        bmp.Save(filepath, ImageFormat.Jpeg);
                        break;
                    case "gif":
                        bmp.Save(filepath, ImageFormat.Gif);
                        break;
                    case "bmp":
                        bmp.Save(filepath, ImageFormat.Bmp);
                        break;
                    default:
                        break;
                }



                ms.Close();
                string url = string.Concat(ConfigurationManager.AppSettings.Get("netfilebaseurl"), "/", subdir, "/");
                if (!string.IsNullOrEmpty(id))
                {

                    url = string.Concat(url, id, "/");
                }

                return string.Concat(url, fileName);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
