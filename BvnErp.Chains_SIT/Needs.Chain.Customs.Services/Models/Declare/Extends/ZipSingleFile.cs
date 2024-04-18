using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ZipSingleFile
    {
        /// <summary>
        /// 待压缩文件路径
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// 压缩后的路径
        /// </summary>
        public string ZipedPath { get; set; }

        /// <summary>
        /// 压缩后的文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///压缩包里，文件名称 
        /// </summary>
        public string ContainedFileName { get; set; }

        /// <summary>
        /// 压缩后的文件名
        /// </summary>
        /// <param name="fileName"></param>
        public ZipSingleFile(string fileName)
        {
            this.FileName = fileName;
        }

        public void SetFilePath(string zipPath)
        {
            this.ZipedPath = zipPath;
        }

        public void Zip()
        {
            using (ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(ZipedPath+FileName)))
            {
                //设置压缩等级，等级越高压缩效果越明显，但占用CPU也会更多
                s.SetLevel(ICSharpCode.SharpZipLib.Zip.Compression.Deflater.DEFAULT_COMPRESSION);  
                using (FileStream fs = System.IO.File.OpenRead(File))
                {
                    //缓冲区，每次操作大小
                    byte[] buffer = new byte[4 * 1024];
                    //创建压缩包内的文件
                    ZipEntry entry = new ZipEntry(Path.GetFileName(ContainedFileName));    
                    entry.DateTime = DateTime.Now;
                    //将文件写入压缩包
                    s.PutNextEntry(entry);          

                    int sourceBytes;
                    do
                    {
                        //读取文件内容(1次读4M，写4M)
                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
                        //将文件内容写入压缩相应的文件
                        s.Write(buffer, 0, sourceBytes);                   
                    } while (sourceBytes > 0);
                }
                s.CloseEntry();
            }
        }

        public void ZipFileManifest()
        {
            FileStream zipFile = null;
            ZipOutputStream zipStream = null;
            ZipEntry zipEntry = null;
            try
            {
                zipFile = System.IO.File.OpenRead(File);
                byte[] buffer = new byte[zipFile.Length];
                zipFile.Read(buffer, 0, buffer.Length);
                zipFile.Close();
                zipFile = System.IO.File.Create(ZipedPath + FileName);
                zipStream = new ZipOutputStream(zipFile);
                zipStream.SetLevel(9);
                zipEntry = new ZipEntry(Path.GetFileName(ContainedFileName));
                zipEntry.Size = buffer.LongLength;
                zipEntry.DateTime = DateTime.Now;
                zipStream.PutNextEntry(zipEntry);
                zipStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {

            }
            finally
            {
                if (zipEntry != null)
                {
                    zipEntry = null;
                }
                if (zipStream != null)
                {
                    zipStream.Finish();
                    zipStream.Close();
                }
                if (zipFile != null)
                {
                    zipFile.Close();
                    zipFile = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
        }


    }
}
