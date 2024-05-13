using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Words;

namespace WebApp.Services
{
    class Class1
    {

        public Class1()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertHtml(@"<html><body><img src='C:/Users/zhouy09/Desktop/Test/底部图片--公司愿景.jpg'></body></html>");
            doc.Save("E:/DownLoadWord/DocumentBuilder.InsertTableFromHtml Out.doc");
        }
    }
}
