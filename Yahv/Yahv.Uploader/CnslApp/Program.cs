using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Views;

namespace CnslApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //string[] files;
            //files = new System.IO.DirectoryInfo(@"E:\Images&Videos\蛋车").GetFiles().Select(item => item.FullName).ToArray();

            //CenterFilesTopView.Upload(Yahv.Underly.FileType.Test, new
            //{
            //    WaybillID = "",
            //    NoticeID = "",
            //    StorageID = "",
            //    InputID = "",
            //    AdminID = "",
            //    ClientID = "",
            //    CustomName = "",
            //    PayID = "",
            //}, files);

            string file = (@"D:\盯盯拍#777.pdf");
            CenterFilesTopView.Upload(Yahv.Underly.FileType.Invoice, new
            {
                WsOrderID = "asdfasdfasdf"
            }, file);

        }
    }
}
