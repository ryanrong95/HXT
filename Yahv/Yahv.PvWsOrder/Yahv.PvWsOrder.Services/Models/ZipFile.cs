using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models
{
    public class ZipFile
    {
        /// <summary>
        /// 待压缩文件名称
        /// </summary>
        public List<string> Files { get; set; }

        /// <summary>
        /// 压缩后的路径
        /// </summary>
        public string ZipedPath { get; set; }

        public string FileName { get; set; }

        /// <summary>
        /// 压缩后的文件名
        /// </summary>
        /// <param name="fileName"></param>
        public ZipFile(string fileName)
        {
            this.FileName = fileName;
        }

        public void SetFilePath(string zipPath)
        {
            this.ZipedPath = zipPath;
        }

        public bool ZipFiles()
        {
            bool isSuccess = true;
            try
            {
                Zip(this.ZipedPath + this.FileName);
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return isSuccess;
        }

        /// <summary>  
        ///  压缩多个文件  
        /// </summary>  
        /// <param name="fileName">压缩包文件名</param>    
        /// <returns></returns>  
        private void Zip(string fileName)
        {
            var files = this.Files.Where(f => File.Exists(f)).ToList();
            if (files.Count > 0)
            {
                ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(fileName));
                //zipOutputStream.SetLevel(ICSharpCode.SharpZipLib.Zip.Compression.Deflater.DEFAULT_COMPRESSION);
                zipOutputStream.SetLevel(9);
                //zipOutputStream(6);
                ZipFileDictory(files, zipOutputStream);
                zipOutputStream.Finish();
                zipOutputStream.Close();
            }
        }

        private void ZipFileDictory(List<string> files, ZipOutputStream zipOutputStream)
        {
            ZipEntry entry = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();
            try
            {
                //创建当前文件夹  
                //entry = new ZipEntry("/");  //加上 “/” 才会当成是文件夹创建  
                //s.PutNextEntry(entry);
                //s.Flush();
                foreach (string file in files)
                {
                    //打开压缩文件  
                    fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    entry = new ZipEntry(Path.GetFileName(file));
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;


                    zipOutputStream.PutNextEntry(entry);
                    zipOutputStream.Write(buffer, 0, buffer.Length);
                }
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
                if (entry != null)
                    entry = null;
                GC.Collect();
            }
        }
    }
}
