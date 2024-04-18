using Aspose.Cells;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PrintFile
    {
        /// <summary>
        /// 运输批次号
        /// </summary>
        public string VoyageNo { get; set; }
        /// <summary>
        /// 模板路径
        /// </summary>
        public string TemplateFilePath { get; set; }
        /// <summary>
        /// 总件数
        /// </summary>
        public int TotalPackNo { get; set; }
        /// <summary>
        /// 总重量
        /// </summary>
        public decimal TotalWeight { get; set; }

          
        /// <summary>
        /// 生成文件
        /// </summary>
        public string GenerateFile()
        {
            string fileName = this.VoyageNo + ChainsGuid.NewGuidUp() +".pdf";
            FileDirectory fileDic = new FileDirectory(fileName);
            fileDic.SetChildFolder(SysConfig.Voyage);
            fileDic.CreateDataDirectory();

          
            Workbook workBook = new Workbook(this.TemplateFilePath);
            WorkbookDesigner designer = new WorkbookDesigner(workBook);

            var VoyageInfo = new VoyagesView().Where(t => t.ID == VoyageNo).FirstOrDefault();

            designer.SetDataSource("DriverName", VoyageInfo.DriverName); //司机姓名
            designer.SetDataSource("CarrierName", VoyageInfo.CarrierName); //牌头公司
            designer.SetDataSource("DriverCode", VoyageInfo.DriverCode); //证件号
            designer.SetDataSource("VehicleLicence", VoyageInfo.VehicleLicence); //境内车牌
            designer.SetDataSource("DriverMobile", VoyageInfo.DriverMobile); //大陆手机
            designer.SetDataSource("HKLicense", VoyageInfo.HKLicense); //港境外车牌
            designer.SetDataSource("DriverHKMobile", VoyageInfo.DriverHKMobile); //香港手机
            designer.SetDataSource("DriverSize", VoyageInfo.VehicleSize); //尺寸
            designer.SetDataSource("DriverCardNo", VoyageInfo.DriverCardNo); //司机卡号
            designer.SetDataSource("VehicleType", VoyageInfo.VehicleType.HasValue ? (((VehicleType)VoyageInfo.VehicleType.Value).GetDescription()) : ""); //车型
            
            designer.SetDataSource("VehicleWeight", VoyageInfo.VehicleWeight  + "KG"); //车重
            designer.SetDataSource("LotNumber", this.VoyageNo); //车辆批次号
            designer.SetDataSource("TotalParts", this.TotalPackNo.ToString() + "件"); //总件数
            designer.SetDataSource("TotalWeight", this.TotalWeight.ToString()+ "KG"); //总重量
            designer.SetDataSource("CurrentDateTime", DateTime.Now.ToString("yyyy-MM-dd")); //当前日期
            designer.SetDataSource("HKSealNumber", VoyageInfo.HKSealNumber); //当前日期

            var manifest = new Needs.Ccs.Services.Views.ManifestConsignmentsView().Where(t => t.ID == VoyageNo).FirstOrDefault();
            if (manifest != null)
            {
                string CustomsCode = manifest.Manifest.CustomsCode;
                var basePort = new Needs.Ccs.Services.Views.BaseCustomMasterView().Where(t => t.Code == CustomsCode).FirstOrDefault();
                designer.SetDataSource("Location", basePort.Name); //口岸
            }
           
            designer.Process();
            designer = null;
            workBook.Save(fileDic.FilePath, SaveFormat.Pdf);

            return fileDic.VirtualPath;
        }
    }
}