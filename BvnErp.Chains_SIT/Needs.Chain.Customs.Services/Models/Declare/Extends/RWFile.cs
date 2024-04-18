using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class RWFile
    {
        public string SourcePath { get; set; }

        /// <summary>
        /// 目标文件
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// 目标路径
        /// </summary>
        public string TargetDirectory { get; set; }
        public void ReadAndWriteFile()
        {
            if (File.Exists(SourcePath))
            {
                if (!Directory.Exists(TargetDirectory))
                {
                    Directory.CreateDirectory(TargetDirectory);
                }
               
                using (FileStream fsRead = new FileStream(SourcePath, FileMode.Open)) 
                { 
                    using (FileStream fsWrite = new FileStream(TargetPath, FileMode.Create))                   
                    { 
                        byte[] byteArrayRead = new byte[1024 * 1024];                                        
                        while (true)
                        {                           
                            int readCount = fsRead.Read(byteArrayRead, 0, byteArrayRead.Length);                           
                            fsWrite.Write(byteArrayRead, 0, readCount);                           
                            if (readCount < byteArrayRead.Length)
                            {
                                break; 
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("源路径或者目标路径不存在。");
            }
        }
    }
}
