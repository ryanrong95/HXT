using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yahv.PsWms.DappForm.Services.FilePrints
{
    public class ImageApp : FilePrintBase
    {
        public ImageApp(string printer) : base(printer)
        {
        }

        public override void Print(string fileName)
        {
            //应用程序域抛异常
            //throw new Exception("5");
            ThreadPool.QueueUserWorkItem(//线程池
                               (p_item) =>
                               {
                                   object missing = Missing.Value;

                                   Application app = new Application();//创建Word应用程序对象
                                   Document doc = app.Documents.Add(ref missing, ref missing, ref missing, ref missing);//建立新文档
                                   Range range = doc.Paragraphs[1].Range;//得到段落范围
                                   doc.PageSetup.TopMargin = 0f;
                                   doc.PageSetup.BottomMargin = 0f;
                                   doc.PageSetup.LeftMargin = 0f;
                                   doc.PageSetup.RightMargin = 0f;


                                   object p_ranges = range;//创建object对象

                                   //定义该插入的图片是否为外部链接
                                   object linkToFile = false;               //默认,这里貌似设置为bool类型更清晰一些
                                                                            //定义要插入的图片是否随Word文档一起保存
                                   object saveWithDocument = true;              //默认

                                   doc.InlineShapes.AddPicture(fileName, ref linkToFile, ref saveWithDocument, p_ranges);

                                   DirectoryInfo di = new DirectoryInfo(AppContext.BaseDirectory);

                                   FileInfo fi = new FileInfo(Path.Combine(@"..\", di.Name + "\\Doc", DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc"));
                                   if (!fi.Directory.Exists)
                                   {
                                       fi.Directory.Create();
                                   }

                                   object path = fi.FullName;

                                   doc.SaveAs2(ref path, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
                                   app.Quit();

                                   FilePrintBase fpb = new WordApp(base.Printer);
                                   fpb.Print(fi.FullName);

                                   Thread.Sleep(10000);
                                   if (File.Exists(fi.FullName))
                                   {
                                       try
                                       {
                                           File.Delete(fi.FullName);
                                       }
                                       catch
                                       {
                                       }
                                   }
                               }

                               );

        }
    }
}
