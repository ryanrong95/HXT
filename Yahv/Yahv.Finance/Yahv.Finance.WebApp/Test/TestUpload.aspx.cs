using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Test
{
    public partial class TestUpload : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //测试上传文件

            List<string> filePaths = new List<string>();
            filePaths.Add(@"D:\00_测试文件\ya1.jpg");
            filePaths.Add(@"D:\00_测试文件\ya2.jpg");

            List<FilesMap> filesMaps = new List<FilesMap>();
            filesMaps.Add(new FilesMap { Name = "qqqqID", Value = "23524" });
            filesMaps.Add(new FilesMap { Name = "rrrrID", Value = "11133" });



            //var result1 = FilesDescriptionRoll.UploadFileAndData(FileDescType.PaymentVoucher, new
            //{
            //    CreatorID = "aaaa",
            //    FilesMaps = JsonConvert.SerializeObject(filesMaps),
            //}, filePaths.ToArray());

            //var result2 = FilesDescriptionRoll.UploadFile(FileDescType.PaymentVoucher, filePaths.ToArray());

        }

    }
}