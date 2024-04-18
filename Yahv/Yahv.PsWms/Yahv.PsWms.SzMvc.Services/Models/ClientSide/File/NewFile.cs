using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Models
{
    /// <summary>
    /// 新增文件
    /// </summary>
    public class NewFile
    {
        /// <summary>
        /// 文件名(a.jpg)
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件目录前缀
        /// </summary>
        public PsOrderFileType FileDirPrefix { get; set; }

        /// <summary>
        /// 将要保存在数据库中的地址(2019/11/1/abc2203lde.jpg)
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 带有http前缀的文件地址(http://192.168.11.22/2019/11/1/abc2203lde.jpg)
        /// </summary>
        public string FullURL { get; set; }

        /// <summary>
        /// 文件在硬盘上的全路径(用于保存文件)(D:\uuws.b1b.com\2019\11\1\abc2203lde.jpg)
        /// </summary>
        public string FullName { get; set; }

        public NewFile(string fileName, PsOrderFileType fileDirPrefix)
        {
            this.FileName = fileName;
            this.FileDirPrefix = fileDirPrefix;
            this.GenrateURL();
        }

        /// <summary>
        /// 产生 URL
        /// </summary>
        private void GenrateURL()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timeSpanStr = Guid.NewGuid().ToString("N").Substring(0, 8) + Convert.ToInt64(ts.TotalMilliseconds).ToString(); //dd35dfd41610279687495
            string fileExt = System.IO.Path.GetExtension(this.FileName); //.jpg

            DateTime now = DateTime.Now;
            string fileSavePath = ConfigurationManager.AppSettings["FileSavePath"]; //D:\Projects_vs2015\Yahv\Yahv.PsWms\Yahv.PsWms.SzMvc\Files
            //string fileSavePath = HttpContext.Current.Server.MapPath("~/Files");
            string directoryPath = Path.Combine(this.FileDirPrefix.ToString(), now.Year.ToString(), now.Month.ToString().PadLeft(2, '0'), now.Day.ToString().PadLeft(2, '0')); //PackingFile\2021\01\10
            string fullFileName = Path.Combine(fileSavePath, directoryPath, timeSpanStr + fileExt); //D:\Projects_vs2015\Yahv\Yahv.PsWms\Yahv.PsWms.SzMvc\Files\PackingFile\2021\01\10\4daf95cd1610281354427.jpg

            FileInfo fi = new FileInfo(fullFileName);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            this.FullName = fullFileName; //D:\Projects_vs2015\Yahv\Yahv.PsWms\Yahv.PsWms.SzMvc\Files\PackingFile\2021\01\10\4daf95cd1610281354427.jpg
            this.URL = Path.Combine(directoryPath, timeSpanStr + fileExt).Replace(@"\", @"/"); //PackingFile/2021/01/10/4daf95cd1610281354427.jpg
            string fileUrlPrefix = ConfigurationManager.AppSettings["FileUrlPrefix"];
            this.FullURL = string.Join(@"/", fileUrlPrefix, this.URL); //http://localhost:6662/Files/PackingFile/2021/01/10/d85e804b1610282095365.jpg
        }

    }
}
