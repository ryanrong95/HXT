using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DownloadFile
    {
        /// <summary>
        /// 目标路径
        /// </summary>
        public string filePath { get; set; }

        /// <summary>
        /// 目标文件名称
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 源文件路径
        /// </summary>
        public string sourceFile { get; set; }

        public bool DownloadFiles()
        {
            bool flag = false;
            //打开上次下载的文件
            long SPosition = 0;
            //实例化流对象
            FileStream FStream;
            //判断要下载的文件夹是否存在
            if (File.Exists(filePath + "\\" + fileName))
            {
                //打开要下载的文件
                FStream = File.OpenWrite(filePath + "\\" + fileName);
                //获取已经下载的长度
                SPosition = FStream.Length;
                FStream.Seek(SPosition, SeekOrigin.Current);
            }
            else
            {
                //判断路径是否存在,如果路径不存在，创建文件会报错
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                FStream = new FileStream(filePath + "\\" + fileName, FileMode.Create, FileAccess.Write);
                SPosition = 0;

            }
            try
            {
                //打开网络连接
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create("http://files.cnblogs.com/files/xiandedanteng/" + fileName);
                if (SPosition > 0)
                    myRequest.AddRange((int)SPosition);             //设置Range值
                //向服务器请求,获得服务器的回应数据流
                Stream myStream = myRequest.GetResponse().GetResponseStream();
                //定义一个字节数据
                byte[] btContent = new byte[512];
                int intSize = 0;
                intSize = myStream.Read(btContent, 0, 512);
                while (intSize > 0)
                {
                    FStream.Write(btContent, 0, intSize);
                    intSize = myStream.Read(btContent, 0, 512);
                }
                //关闭流
                FStream.Close();
                myStream.Close();
                flag = true;        //返回true下载成功
            }
            catch (Exception)
            {
                FStream.Close();
                flag = false;       //返回false下载失败
            }
            return flag;
        }
    }
}

