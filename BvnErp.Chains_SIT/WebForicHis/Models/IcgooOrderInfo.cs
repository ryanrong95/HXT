using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WebForicHis.Models
{
    public class IcgooOrderInfo
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string IcgooOrder;

        public string ConnectionString { get; set; }
        public IcgooOrderInfo(string IcgooOrder)
        {

            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ScCustomsConnectionString"].ConnectionString;
            this.IcgooOrder = IcgooOrder;
        }

        public void GetPI()
        {
            //string data = DateTime.Now.ToString("yyyyMM");
            //string day = DateTime.Now.ToString("yyyyMMdd").Substring(6, 2);
            //var fileName = string.Empty;//this.IcgooOrder + "_pi.pdf";//TempFiles/
            //string dataPath = string.Concat("Order/", data, "/", day, "/");
            //var path = Path.Combine("D:/wl/admin/Files/", dataPath);

            string data = DateTime.Now.ToString("yyyyMM");
            string day = DateTime.Now.ToString("yyyyMMdd").Substring(6, 2);
            var fileName = this.IcgooOrder + "_pi.pdf";
            string dataPath = string.Concat("Order/", data, "/", day, "/");
            var path = Path.Combine("D:/wl/admin/Files/", dataPath);

            var isDownload = GetPDF(string.Format("http://baoguan.k0v.cn/api_get_pi/?cm={0}&file_type=pi", this.IcgooOrder), path, fileName);
      
            var OrderIDs = new Needs.Ccs.Services.Views.IcgooMapView().Where(t => t.IcgooOrder == this.IcgooOrder).Select(t => t.OrderID).ToList();
            if (OrderIDs.Count() > 0 && isDownload)
            {
                ////获取成功，写入数据
                //foreach (var orderid in OrderIDs)
                //{
                //    var orderfile = new Needs.Ccs.Services.Models.OrderFile()
                //    {
                //        OrderID = orderid,
                //        Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create("XDTAdmin"),
                //        Name = fileName,
                //        FileType = Needs.Ccs.Services.Enums.FileType.OriginalInvoice,
                //        FileFormat = "application/pdf",
                //        Url = dataPath + fileName,
                //        FileStatus = Needs.Ccs.Services.Enums.OrderFileStatus.Audited
                //    };
                //    orderfile.Enter();
                //}


                string[] ids = OrderIDs[0].Split('-');
                string mainOrderID = ids[0];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create("XDTAdmin");
                var ermAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == admin.ID);
                //var orderfile = new Needs.Ccs.Services.Models.MainOrderFile()
                //{
                //    MainOrderID = mainOrderID,
                //    Admin = admin,
                //    Name = fileName,
                //    FileType = Needs.Ccs.Services.Enums.FileType.OriginalInvoice,
                //    FileFormat = "application/pdf",
                //    Url = dataPath + fileName,
                //    FileStatus = Needs.Ccs.Services.Enums.OrderFileStatus.Audited
                //};
                //orderfile.Enter();

                //本地文件上传到服务器
                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.Invoice;
                var dic = new { CustomName = fileName, WsOrderID = mainOrderID, AdminID = ermAdmin.ID };
                var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(path + fileName, centerType, dic);

                string[] ID = { result[0].FileID };
                new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, ID);



                #region old
                ////2020-03-20 全部更新一次
                //string[] ids = OrderIDs[0].Split('-');

                //var mainOrderID = "";
                //if (OrderIDs[0].IndexOf("-") > 0)
                //{
                //    mainOrderID = ids[0];

                //    var orderfilebool = new Needs.Ccs.Services.Views.MainOrderFilesView().FirstOrDefault(t => t.MainOrderID == mainOrderID
                //   && t.FileType == Needs.Ccs.Services.Enums.FileType.OriginalInvoice && t.Name == fileName && t.Status == Needs.Ccs.Services.Enums.Status.Normal);


                //        var orderfile = new Needs.Ccs.Services.Models.MainOrderFile()
                //        {
                //            ID = orderfilebool?.ID,
                //            MainOrderID = mainOrderID,
                //            Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create("XDTAdmin"),
                //            Name = fileName,
                //            FileType = Needs.Ccs.Services.Enums.FileType.OriginalInvoice,
                //            FileFormat = "application/pdf",
                //            Url = dataPath + fileName,
                //            FileStatus = Needs.Ccs.Services.Enums.OrderFileStatus.Audited
                //        };
                //        orderfile.Enter();

                //}
                //else
                //{
                //    foreach (var id in OrderIDs)
                //    {
                //        var orderfilebool = new Needs.Ccs.Services.Views.MainOrderFilesView().FirstOrDefault(t => t.MainOrderID == id
                //        && t.FileType == Needs.Ccs.Services.Enums.FileType.OriginalInvoice && t.Name == fileName && t.Status == Needs.Ccs.Services.Enums.Status.Normal);


                //            var orderfile = new Needs.Ccs.Services.Models.MainOrderFile()
                //            {
                //                ID = orderfilebool?.ID,
                //                MainOrderID = id,
                //                Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create("XDTAdmin"),
                //                Name = fileName,
                //                FileType = Needs.Ccs.Services.Enums.FileType.OriginalInvoice,
                //                FileFormat = "application/pdf",
                //                Url = dataPath + fileName,
                //                FileStatus = Needs.Ccs.Services.Enums.OrderFileStatus.Audited
                //            };
                //            orderfile.Enter();

                //    }
                //}

                #endregion
            }
        }

        public bool GetPDF(string url, string SavePath, string FileName)
        {
            var result = false;
            try
            {
                // 创建一个HTTP请求
                var request = HttpWebRequest.CreateHttp(url);
                request.Method = "GET";

                //设置超时时间
                request.Timeout = 9000;

                //获取请求
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        //直到request.GetResponse()程序才开始向目标网页发送Post请求
                        System.IO.Stream responseStream = response.GetResponseStream();

                        //不存在就创建目录
                        if (!System.IO.Directory.Exists(SavePath))
                        {
                            System.IO.Directory.CreateDirectory(SavePath);
                        }

                        //创建本地文件写入流
                        System.IO.Stream stream = new System.IO.FileStream(SavePath + FileName, System.IO.FileMode.Create);

                        byte[] bArr = new byte[1024];
                        int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        while (size > 0)
                        {
                            stream.Write(bArr, 0, size);
                            size = responseStream.Read(bArr, 0, (int)bArr.Length);
                        }
                        stream.Close();
                        responseStream.Close();

                        result = true;
                    }
                }
                request.Abort();
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }

    }
}