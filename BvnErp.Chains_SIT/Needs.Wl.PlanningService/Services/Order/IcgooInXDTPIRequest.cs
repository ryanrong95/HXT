using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.Order
{
    public class IcgooInXDTPIRequest : PIRequest
    {
        public string IcgooOrder;

        public List<string> OrderIDs;

        public IcgooInXDTPIRequest(string IcgooOrder, List<string> OrderIDs) : base("icgooXDT")
        {
            this.IcgooOrder = IcgooOrder;
            this.OrderIDs = OrderIDs;
        }


        public override void Process()
        {
            var Api = base.ApiSetting.Apis[ApiType.ICGOOPI];
            var Headers = base.ApiSetting.Headers;
            Dictionary<string, string> headers = Headers;
            HttpRequest request = new HttpRequest
            {
                Timeout = this.ApiSetting.Timeout,
                Headers = headers
            };

            string data = DateTime.Now.ToString("yyyyMM");
            string day = DateTime.Now.ToString("yyyyMMdd").Substring(6, 2);
            var fileName = this.IcgooOrder + "_pi.pdf";
            string dataPath = string.Concat("Order/", data, "/", day, "/");
            var path = Path.Combine(Api.SavePIPath, dataPath);

            var isDownload = request.GetPDF(string.Format(Api.Url, this.IcgooOrder), path, fileName);

            //获取成功，写入数据
            if (isDownload)
            {
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

            }

        }
    }
}
