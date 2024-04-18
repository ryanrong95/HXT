using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.Order
{
    public class DyjPIRequest : PIRequest
    {
        public string DyjOrder;

        public List<string> OrderIDs;

        public DyjPIRequest(string DyjOrder) : base("dyj")
        {
            this.DyjOrder = DyjOrder;
        }

        public override void Process()
        {
            var Api = base.ApiSetting.Apis[ApiType.DYJPI];
            HttpRequest request = new HttpRequest
            {
                Timeout = this.ApiSetting.Timeout
            };

            try
            {
                var message = request.GetMessage(Api.Url, this.DyjOrder);
                var ResponseResult = message.JsonTo<DyjPIViewModel>();

                string data = DateTime.Now.ToString("yyyyMM");
                string day = DateTime.Now.ToString("yyyyMMdd").Substring(6, 2);
                var fileName = string.Empty;
                string dataPath = string.Concat("Order/", data, "/", day, "/");
                var path = System.IO.Path.Combine(Api.SavePIPath, dataPath);

                var OrderIDs = new Needs.Ccs.Services.Views.IcgooMapView().Where(t => t.IcgooOrder == this.DyjOrder).Select(t => t.OrderID).ToList();
                if (OrderIDs.Count() > 0)
                {
                    foreach (var pi in ResponseResult.data)
                    {
                        fileName = pi.PI文件路径.Replace("TempFiles/", "").Replace("DownloadFiles/", "");
                        //http://office.51db.com:81/TempFiles/194415263071_83_114_PI.pdf
                        var isDownload = request.GetPDF(string.Format("http://office.51db.com:81/{0}", pi.PI文件路径), path, fileName);

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
                            var dic = new { CustomName = fileName, WsOrderID = mainOrderID, AdminID = ermAdmin.ID};                            
                            var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(path + fileName, centerType, dic);

                            string[] ID = { result[0].FileID };
                            new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
