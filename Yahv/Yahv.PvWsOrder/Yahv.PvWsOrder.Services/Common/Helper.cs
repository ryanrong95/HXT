using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Yahv.Services;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Npoi;

namespace Yahv.PvWsOrder.Services.Common
{
    public sealed class Helper
    {
        /// <summary>
        /// 深圳芯达通供应链有限公司ID
        /// </summary>
        public static string XdtCompanyID
        {
            get
            {
                return "DBAEAB43B47EB4299DD1D62F764E6B6A";
            }
        }

        /// <summary>
        /// 下单时产品导入
        /// </summary>
        public static object ProductImport(HttpPostedFile file)
        {
            Services.Common.FileDirectory dic = new Services.Common.FileDirectory(file.FileName, FileType.ProductImportFile);
            var filePath = dic.SaveLocal(file);

            //生成DataTable
            var workbook = ExcelFactory.CreateByTemplate(filePath);
            var npoi = new NPOIHelper(workbook);
            DataTable dt = npoi.ExcelToDataTable(false);

            //获取产品数据
            var products = new List<dynamic>();
            for (int i = 6; i < dt.Rows.Count; i++)
            {
                //必填项:型号、品牌、数量、总价
                if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()) || string.IsNullOrEmpty(dt.Rows[i][6].ToString()) ||
                    string.IsNullOrEmpty(dt.Rows[i][9].ToString()) || string.IsNullOrEmpty(dt.Rows[i][11].ToString()))
                {
                    continue;
                }

                dynamic product = new System.Dynamic.ExpandoObject();
                product.CustomName = dt.Rows[i][4].ToString();
                product.PartNumber = dt.Rows[i][6].ToString();
                product.Manufacturer = dt.Rows[i][5].ToString();
                product.Qty = decimal.Parse(dt.Rows[i][9].ToString());

                //product.DateCode = dt.Rows[i][3].ToString();
                var origin = dt.Rows[i][7].ToString()?.Trim();
                if (string.IsNullOrEmpty(origin))
                {
                    product.Origin = Origin.USA;
                }
                else
                {
                    var relOrigin = ExtendsEnum.ToArray<Origin>()
                        .Where(item => item.GetOrigin().Code == origin || item.GetOrigin().ChineseName == origin).FirstOrDefault();
                    if (relOrigin == 0)
                    {
                        product.Origin = Origin.USA;
                    }
                    else
                    {
                        product.Origin = relOrigin;
                    }
                }
                product.UnitPrice = decimal.Parse(dt.Rows[i][10].ToString());
                product.TotalPrice = decimal.Parse(dt.Rows[i][11].ToString());
                product.TaxCode = dt.Rows[i][1].ToString();
                product.Currency = "";
                product.Unit = "007";
                product.GrossWeight = 0.02;
                product.Volume = 0.01;
                product.ProductUniqueCode = dt.Rows[i][1].ToString();
                products.Add(product);
            }
            //删除本地文件
            System.IO.File.Delete(filePath);
            return products;
        }

        /// <summary>
        /// 下单时产品导入
        /// </summary>
        public static object ProductImport2(HttpPostedFile file)
        {
            Services.Common.FileDirectory dic = new Services.Common.FileDirectory(file.FileName, FileType.ProductImportFile);
            var filePath = dic.SaveLocal(file);

            //生成DataTable
            var workbook = ExcelFactory.CreateByTemplate(filePath);
            var npoi = new NPOIHelper(workbook);
            DataTable dt = npoi.ExcelToDataTable(false);

            //获取产品数据
            var products = new List<dynamic>();
            for (int i = 6; i < dt.Rows.Count; i++)
            {
                //必填项:型号、品牌、数量、总价
                if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()) || string.IsNullOrEmpty(dt.Rows[i][6].ToString()) ||
                    string.IsNullOrEmpty(dt.Rows[i][9].ToString()) || string.IsNullOrEmpty(dt.Rows[i][11].ToString()))
                {
                    continue;
                }

                dynamic product = new System.Dynamic.ExpandoObject();
                product.CustomName = dt.Rows[i][4].ToString();
                product.PartNumber = dt.Rows[i][6].ToString();
                product.Manufacturer = dt.Rows[i][5].ToString();
                product.Qty = decimal.Parse(dt.Rows[i][9].ToString());

                //product.DateCode = dt.Rows[i][3].ToString();
                var origin = dt.Rows[i][7].ToString()?.Trim();
                if (string.IsNullOrEmpty(origin))
                {
                    product.Origin = Origin.USA.ToString();
                }
                else
                {
                    var relOrigin = ExtendsEnum.ToArray<Origin>()
                        .Where(item => item.GetOrigin().Code == origin || item.GetOrigin().ChineseName == origin).FirstOrDefault();
                    if (relOrigin == 0)
                    {
                        product.Origin = Origin.USA.ToString();
                    }
                    else
                    {
                        product.Origin = relOrigin.ToString();
                    }
                }
                product.UnitPrice = decimal.Parse(dt.Rows[i][10].ToString());
                product.TotalPrice = decimal.Parse(dt.Rows[i][11].ToString());
                product.TaxCode = dt.Rows[i][1].ToString();
                product.Currency = "";
                product.Unit = "007";
                product.GrossWeight = 0.02;
                product.Volume = 0.01;
                product.ProductUniqueCode = dt.Rows[i][1].ToString();
                products.Add(product);
            }
            //删除本地文件
            System.IO.File.Delete(filePath);
            return products;
        }

        /// <summary>
        /// 根据内外单客户区分交货库房
        /// 收货人(内单：WLT，外单：CY)
        /// 内单XL开头，ICG（ICG）
        /// </summary>
        /// <param name="enterCode">客户入仓号</param>
        /// <returns>畅运/万路通</returns>
        public static string GetWarehouseName(string enterCode)
        {
            //var interClientFlag = enterCode.StartsWith("XL") || enterCode.StartsWith("ICG") ? true : false;
            //var receiptWHName = interClientFlag == true ? WhSettings.HK[ConfigurationManager.AppSettings["HKWLTID"]].Enterprise.Name :
            //    WhSettings.HK[ConfigurationManager.AppSettings["HKCYID"]].Enterprise.Name;
            var receiptWHName = WhSettings.HK[ConfigurationManager.AppSettings["HKCYID"]].Enterprise.Name;
            return receiptWHName;
        }
    }
}
