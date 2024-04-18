using Needs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 各个PI生成器的抽象类
    /// </summary>
    public abstract class PIBuilder
    {
        protected string FileName { get; set; }

        protected List<Views.OrderItemSupplierViewModel> Items { get; set; }

        protected string PINO { get; set; }

        protected string ClientName { get; set; }

        public PIBuilder(string fileName, List<Views.OrderItemSupplierViewModel> items, string piNo, string clientName)
        {
            this.FileName = fileName;
            this.Items = items;
            this.PINO = piNo;
            this.ClientName = clientName;
        }

        public abstract void Execute();
    }

    public class PIGener
    {
        private string OrderID { get; set; }

        private string ClientName { get; set; }

        public PIGener(string orderID, string ClientName)
        {
            this.OrderID = orderID;
            this.ClientName = ClientName;
        }

        public void Execute()
        {
            try
            {
                //查 PI NO
                string piNo = string.Empty;

                var icgooOrderMap = new Needs.Ccs.Services.Views.Origins.IcgooOrderMapOrigin()
                    .Where(t => t.OrderID == this.OrderID && t.Status == Enums.Status.Normal).FirstOrDefault();

                if (icgooOrderMap != null && !string.IsNullOrEmpty(icgooOrderMap.IcgooOrder))
                {
                    piNo = icgooOrderMap.IcgooOrder;
                }

                //查该订单所有包含供应商的型号信息
                var orderItems = new Needs.Ccs.Services.Views.OrderItemSupplierView().GetInfos(this.OrderID);

                var groupedItems = (from orderItem in orderItems
                                    group orderItem by new { orderItem.SupplierID, orderItem.SupplierName } into g
                                    select new
                                    {
                                        SupplierID = g.Key.SupplierID,
                                        SupplierName = g.Key.SupplierName,
                                        OrderItems = g.Where(t => t.SupplierID == g.Key.SupplierID).ToList(),
                                    }).ToList();

                foreach (var groupedItem in groupedItems)
                {
                    PIBuilder pIBuilder = null;
                    string fileName = "";
                    string filePath = "";

                    string id_tail = this.OrderID;
                    //string[] ids1 = this.OrderID.Split('-');
                    //string id_tail = ids1[1];

                    switch (groupedItem.SupplierName.Trim().ToLower())
                    {
                        case "ic360 electronics limited":
                            fileName = piNo + "_" + id_tail + "_103_" + "128" + "_PI.pdf";
                            filePath = GetFilePath(fileName);
                            pIBuilder = new PI103IC360(filePath, groupedItem.OrderItems, piNo, this.ClientName);
                            break;
                        case "ic360 group limited":
                            fileName = piNo + "_" + id_tail + "_105_" + "128" + "_PI.pdf";
                            filePath = GetFilePath(fileName);
                            pIBuilder = new PI105IC360GROUP(filePath, groupedItem.OrderItems, piNo, this.ClientName);
                            break;
                        case "hk huanyu electronics technology co.,limited":
                            fileName = piNo + "_" + id_tail + "_112_" + "128" + "_PI.pdf";
                            filePath = GetFilePath(fileName);
                            pIBuilder = new PI112HY(filePath, groupedItem.OrderItems, piNo, this.ClientName);
                            break;
                        case "hong kong changyun international logistics co.,limited":
                            fileName = piNo + "_" + id_tail + "_114_" + "128" + "_PI.pdf";
                            filePath = GetFilePath(fileName);
                            pIBuilder = new PI114CY(filePath, groupedItem.OrderItems, piNo, this.ClientName);
                            break;
                        case "kb electronics development ltd":
                            fileName = piNo + "_" + id_tail + "_122_" + "128" + "_PI.pdf";
                            filePath = GetFilePath(fileName);
                            pIBuilder = new PI122KB(filePath, groupedItem.OrderItems, piNo, this.ClientName);
                            break;
                        case "anda international trade group limited":
                            fileName = piNo + "_" + id_tail + "_83_" + "128" + "_PI.pdf";
                            filePath = GetFilePath(fileName);
                            pIBuilder = new PI83Anda(filePath, groupedItem.OrderItems, piNo, this.ClientName);
                            break;
                        case "hk lianchuang electronics co.,limited":
                            fileName = piNo + "_" + id_tail + "_96_" + "128" + "_PI.pdf";
                            filePath = GetFilePath(fileName);
                            pIBuilder = new PI96LC(filePath, groupedItem.OrderItems, piNo, this.ClientName);
                            break;
                        default:
                            break;
                    }

                    if (pIBuilder != null)
                    {
                        pIBuilder.Execute();

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            string[] ids = this.OrderID.Split('-');
                            string mainOrderID = ids[0];
                            new PIUploader(fileName, mainOrderID, filePath).Execute();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ex.CcsLog("PIGener报错|TinyOrderID = " + this.OrderID);
            }
        }

        private string GetFilePath(string fileName)
        {
            FileDirectory fileDic = new FileDirectory(fileName);
            fileDic.SetChildFolder(SysConfig.DeclareDirectory);
            fileDic.CreateDataDirectory();
            return fileDic.FilePath;
        }

    }
}
