using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PIUploader
    {
        private string FileName { get; set; }

        private string MainOrderID { get; set; }

        private string FilePath { get; set; }

        public PIUploader(string fileName, string mainOrderID, string filePath)
        {
            this.FileName = fileName;
            this.MainOrderID = mainOrderID;
            this.FilePath = filePath;
        }

        public void Execute()
        {
            try
            {
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create("XDTAdmin");
                var ermAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == admin.ID);

                //本地文件上传到服务器
                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.Invoice;
                var dic = new { CustomName = this.FileName, WsOrderID = this.MainOrderID, AdminID = ermAdmin.ID };
                var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(this.FilePath, centerType, dic);

                string[] ID = { result[0].FileID };
                new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, ID);
            }
            catch (Exception ex)
            {
                string actionStr = "PIUploader报错|" + this.FileName + "|" + this.MainOrderID + "|" + this.FilePath + "|";

                if (!string.IsNullOrEmpty(actionStr) && actionStr.Length > 200)
                {
                    actionStr = actionStr.Substring(0, 200);
                }

                ex.CcsLog(actionStr);
            }
        }
    }
}
