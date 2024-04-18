using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Yahv.Web.Forms
{
    /// <summary>
    /// 上传文件对象
    /// </summary>
    sealed public class UploadFile
    {
        /// <summary>
        /// 获取上载文件的大小（以字节为单位）
        /// </summary>
        public int ContentLength { get { return this.file.ContentLength; } }

        /// <summary>
        /// 获取客户端发送的文件的 MIME 内容类型
        /// </summary>
        public string ContentType { get { return this.file.ContentType; } }

        /// <summary>
        /// 获取客户端上的文件的完全限定名称
        /// </summary>
        public string FileName { get { return this.file.FileName; } }
     
        /// <summary>
        /// 设置调用地址
        /// </summary>
        public string CallUrl { get; internal set; }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename">保存的文件的名称</param>
        public void SaveAs(string filename)
        {
            file.SaveAs(filename);
        }


        HttpPostedFile file;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="file">http post file</param>
        /// <param name="index">索引</param>
        internal UploadFile(HttpPostedFile file)
        {
            this.file = file;
        }
    }

    /// <summary>
    /// 上传文件参数
    /// </summary>
    public class FileUploaderEventArgs : EventArgs
    {
        /// <summary>
        /// 上传的文件对象
        /// </summary>
        public UploadFile[] Files { get; private set; }

        internal FileUploaderEventArgs(HttpFileCollection files)
        {
            UploadFile[] arry = this.Files = new UploadFile[files.Count];
            for (int index = 0; index < arry.Length; index++)
            {
                arry[index] = new UploadFile(files[index]);
            }
        }
    }

    /// <summary>
    /// 上传文件句柄
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">上传文件参数</param>
    public delegate void FileUploadedHandle(object sender, FileUploaderEventArgs e);
}
