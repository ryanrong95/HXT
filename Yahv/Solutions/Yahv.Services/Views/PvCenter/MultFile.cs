using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yahv.Utils.Serializers;

namespace Yahv.Services.Views
{
    internal class CreateBytes
    {
        Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 拼接所有的二进制数组为一个数组
        /// </summary>
        /// <param name="byteArrays">数组</param>
        /// <returns></returns>
        /// <remarks>加上结束边界</remarks>
        public byte[] JoinBytes(ArrayList byteArrays)
        {
            int length = 0;
            int readLength = 0;

            // 加上结束边界
            string endBoundary = Boundary + "--\r\n"; //结束边界
            byte[] endBoundaryBytes = encoding.GetBytes(endBoundary);
            byteArrays.Add(endBoundaryBytes);

            foreach (byte[] b in byteArrays)
            {
                length += b.Length;
            }
            byte[] bytes = new byte[length];

            // 遍历复制
            //
            foreach (byte[] b in byteArrays)
            {
                b.CopyTo(bytes, readLength);
                readLength += b.Length;
            }

            return bytes;
        }

        public bool UploadData(string uploadUrl, byte[] bytes, out byte[] responseBytes)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", ContentType);

            try
            {
                responseBytes = webClient.UploadData(uploadUrl, bytes);
                return true;
            }
            catch (WebException ex)
            {
                Stream resp = ex.Response.GetResponseStream();
                responseBytes = new byte[ex.Response.ContentLength];
                resp.Read(responseBytes, 0, responseBytes.Length);
            }
            return false;
        }

        /**/
        /// <summary>
        /// 获取普通表单区域二进制数组
        /// </summary>
        /// <param name="fieldName">表单名</param>
        /// <param name="fieldValue">表单值</param>
        /// <returns></returns>
        /// <remarks>
        /// -----------------------------7d52ee27210a3c\r\nContent-Disposition: form-data; name=\"表单名\"\r\n\r\n表单值\r\n
        /// </remarks>
        public byte[] CreateFieldData(string fieldName, string fieldValue)
        {
            string textTemplate = Boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
            string text = String.Format(textTemplate, fieldName, fieldValue);
            byte[] bytes = encoding.GetBytes(text);
            return bytes;
        }
        /**/
        /// <summary>
        /// 获取文件上传表单区域二进制数组
        /// </summary>
        /// <param name="fieldName">表单名</param>
        /// <param name="filename">文件名</param>
        /// <param name="contentType">文件类型</param>
        /// <param name="contentLength">文件长度</param>
        /// <param name="stream">文件流</param>
        /// <returns>二进制数组</returns>
        public byte[] CreateFieldData(string fieldName, string filename, string contentType, byte[] fileBytes)
        {
            string end = "\r\n";
            string textTemplate = Boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

            // 头数据
            string data = String.Format(textTemplate, fieldName, filename, contentType);
            byte[] bytes = encoding.GetBytes(data);



            // 尾数据
            byte[] endBytes = encoding.GetBytes(end);

            // 合成后的数组
            byte[] fieldData = new byte[bytes.Length + fileBytes.Length + endBytes.Length];

            bytes.CopyTo(fieldData, 0); // 头数据
            fileBytes.CopyTo(fieldData, bytes.Length); // 文件的二进制数据
            endBytes.CopyTo(fieldData, bytes.Length + fileBytes.Length); // \r\n

            return fieldData;
        }

        #region 属性
        public string Boundary
        {
            get
            {
                string[] bArray, ctArray;
                string contentType = ContentType;
                ctArray = contentType.Split(';');
                if (ctArray[0].Trim().ToLower() == "multipart/form-data")
                {
                    bArray = ctArray[1].Split('=');
                    return "--" + bArray[1];
                }
                return null;
            }
        }

        public string ContentType
        {
            get
            {
                return "multipart/form-data; boundary=---------------------------7d5b915500cee";
            }
        }
        #endregion
    }

    public class MyClass_456789
    {
        public MyClass_456789()
        {
            string[] files = { @"D:\Images\小甲\IMG_1276.JPG", @"D:\Images\小甲\IMG_1291.JPG" };

            CenterFilesTopView.Upload(Underly.FileType.Test, null,  files);

            //CreateBytes cb = new CreateBytes();
            //// 所有表单数据
            //ArrayList bytesArray = new ArrayList();
            //// 普通表单
            //bytesArray.Add(cb.CreateFieldData("jsonDataStr", ""));

            //// 读文件流
            //FileStream fs = new FileStream(@"d:\九大步骤.png", FileMode.Open,
            //    FileAccess.Read, FileShare.Read);

            //string ContentType = "application/octet-stream";
            //byte[] fileBytes = new byte[fs.Length];
            //fs.Read(fileBytes, 0, Convert.ToInt32(fs.Length));

            //// 文件表单
            //bytesArray.Add(cb.CreateFieldData("file1", "九大步骤.JPG", ContentType, fileBytes));

            //bytesArray.Add(cb.CreateFieldData("file2", "九大步骤1.JPG", ContentType, fileBytes));
            //// 合成所有表单并生成二进制数组
            //byte[] bytes = cb.JoinBytes(bytesArray);

            //// 返回的内容
            //byte[] responseBytes;

            //bool uploaded = cb.UploadData("http://uu.szhxd.net/api/Upload", bytes, out responseBytes);
        }

        static byte[] ReadBytes(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open,
              FileAccess.Read, FileShare.Read))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));

                return bytes;
            }
        }

        static public Yahv.Services.Models.UploadResult[] Upload(object query, params string[] fileNames)
        {
            CreateBytes cb = new CreateBytes();
            // 所有表单数据
            ArrayList bytesArray = new ArrayList();

            //// form 数据
            //bytesArray.Add(cb.CreateFieldData("jsonDataStr", ""));

            string ContentType = "application/octet-stream";

            int counter = 1;
            foreach (var fileName in fileNames)
            {
                // 文件表单
                bytesArray.Add(cb.CreateFieldData("file" + (counter++),
                    Path.GetFileName(fileName),
                    ContentType,
                    ReadBytes(fileName)));
            }

            byte[] bytes = cb.JoinBytes(bytesArray);

            // 返回的内容
            byte[] responseBytes;

            bool uploaded = cb.UploadData("http://uu.szhxd.net/api/Upload", bytes, out responseBytes);

            if (uploaded)
            {
                string message = Encoding.UTF8.GetString(responseBytes);
                return message.JsonTo<Yahv.Services.Models.UploadResult[]>();
            }

            return null;
        }
    }
}
