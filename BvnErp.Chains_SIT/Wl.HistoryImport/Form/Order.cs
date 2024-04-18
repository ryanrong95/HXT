using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wl.HistoryImport
{
    public partial class Order : Form
    {
        public Order()
        {
            InitializeComponent();
        }

        private void btnOrderImport_Click(object sender, EventArgs e)
        {
            //跟单员
            string adminID = "";
            var admin = new Needs.Ccs.Services.Views.AdminsTopView().Where(item => item.ID == adminID).FirstOrDefault();

            //客户
            string clientID = "";
            var client = new Needs.Wl.Admin.Plat.Views.MyClientsView((IGenericAdmin)admin).Where(item => item.ID == clientID).FirstOrDefault();
            //客户供应商
            string supplierID = "";
            var supplier = client.Suppliers[supplierID];

            //订单
            var order = new Needs.Ccs.Services.Models.Order();
            order.Client = client;
            order.OrderConsignee = new OrderConsignee();
            order.OrderConsignee.OrderID = order.ID;
            order.OrderConsignor = new OrderConsignor();
            order.OrderConsignor.OrderID = order.ID;
            order.AdminID = adminID;
            order.ClientAgreement = client.Agreement;

            //香港交货信息
            //order.OrderConsignee.ClientSupplier = supplier;
            //order.OrderConsignee.Type = (HKDeliveryType)Enum.Parse(typeof(HKDeliveryType), hkDeliveryType);
            //if (order.OrderConsignee.Type == HKDeliveryType.SentToHKWarehouse)
            //{
            //    order.OrderConsignee.Contact = null;
            //    order.OrderConsignee.Mobile = null;
            //    order.OrderConsignee.Address = null;
            //    order.OrderConsignee.PickUpTime = null;
            //    order.OrderConsignee.WayBillNo = waybillNO;
            //}
            //else
            //{
            //    order.OrderConsignee.Contact = supplierContact;
            //    order.OrderConsignee.Mobile = supplierContactMobile;
            //    order.OrderConsignee.Address = supplierAddress;
            //    order.OrderConsignee.PickUpTime = DateTime.Parse(pickupTime);
            //    order.OrderConsignee.WayBillNo = null;

            //    //提货单
            //    if (order.OrderConsignee.Type == HKDeliveryType.PickUp)
            //    {
            //        string docType = "msword";
            //        string docxType = "vnd.openxmlformats-officedocument.wordprocessingml.document";
            //        var thisDeliveryFile = new OrderFile
            //        {
            //            OrderID = order.ID,
            //            Admin = admin,
            //            Name = deliveryFilePlus[0].Name,
            //            FileType = FileType.DeliveryFiles,
            //            FileFormat = Convert.ToString(deliveryFilePlus[0].FileFormat).Replace(docType, "doc").Replace(docxType, "docx"),
            //            Url = Convert.ToString(deliveryFilePlus[0].VirtualPath).Replace(@"/", @"\")
            //        };
            //        order.Files.Add(thisDeliveryFile);
            //    }
            //}
        }
    }
}
