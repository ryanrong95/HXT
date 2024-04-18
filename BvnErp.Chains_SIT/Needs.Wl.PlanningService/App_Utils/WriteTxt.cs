using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class WriteTxt
    {
        private string Context { get; set; }
        private string CurrentDirectory { get; set; }
        private string FileName { get; set; }

        public WriteTxt(string context,string fileName)
        {
            this.Context = context;
            this.CurrentDirectory = System.Environment.CurrentDirectory;
            this.FileName = fileName;
        }

     
        public void Write()
        {
            string directoryPath = this.CurrentDirectory + "\\log";
            string filePath = directoryPath+"\\"+this.FileName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            FileStream fs = new FileStream(filePath, FileMode.Append);
            //获得字节数组
            byte[] data = System.Text.Encoding.Default.GetBytes(this.Context+"\r\n");
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }
    }
}
