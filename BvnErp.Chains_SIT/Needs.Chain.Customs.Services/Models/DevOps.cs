using Needs.Ccs.Services.Enums;
using Needs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DevOps
    {
        public DevOps()
        { }

        public void Excute()
        {
            //var view = new Views.OrdersViewBase<Needs.Ccs.Services.Models.Order>().Where(t => t.ClientID == "E6E9F4EBD1D093DF8E2813B2D648F9A8" || 
            //t.ClientID == "469D2ED6406B7DFCCE9B054A4E3B36B2").Select(t=>t.ID).ToList();
            //var view = new Views.OrdersViewBase<Needs.Ccs.Services.Models.Order>().Where(t => t.Client.ID == "469D2ED6406B7DFCCE9B054A4E3B36B2").Select(t => t.ID).ToList();
            var ss = new List<string>();
            ss.Add("WL15620211111502");
            ss.Add("WL15620220328503");
            ss.Add("WL15620220329501");
            ss.Add("WL15620220329502");
            ss.Add("WL15620220329503");
            ss.Add("WL15620220330501");
            ss.Add("WL15620220330502");
            ss.Add("WL15620220330503");
            ss.Add("WL15620220331501");
            ss.Add("WL15620220331502");
            ss.Add("WL15620220401501");
            ss.Add("WL15620220401502");
            ss.Add("WL15620220401503");
            ss.Add("WL15620220406501");
            ss.Add("WL15620220406502");
            ss.Add("WL15620220406503");
            ss.Add("WL15620220406504");
            ss.Add("WL15620220406505");
            ss.Add("WL15620220407501");
            ss.Add("WL15620220407502");
            ss.Add("WL15620220408501");
            ss.Add("WL15620220408502");
            ss.Add("WL15620220411501");
            ss.Add("WL15620220411502");
            ss.Add("WL15620220412501");
            ss.Add("WL15620220412502");
            ss.Add("WL15620220412503");
            ss.Add("WL15620220413501");
            ss.Add("WL15620220413502");
            ss.Add("WL15620220413503");
            ss.Add("WL15620220414501");
            ss.Add("WL15620220414502");
            ss.Add("WL15620220414503");
            ss.Add("WL15620220419501");
            ss.Add("WL15620220419502");
            ss.Add("WL15620220420501");
            ss.Add("WL15620220420502");
            ss.Add("WL15620220421501");
            ss.Add("WL15620220421502");
            ss.Add("WL15620220422501");
            ss.Add("WL15620220422502");
            ss.Add("WL15620220425501");
            ss.Add("WL15620220425502");
            ss.Add("WL15620220425503");
            ss.Add("WL15620220426501");
            ss.Add("WL15620220426502");
            ss.Add("WL15620220427501");
            ss.Add("WL15620220427502");
            ss.Add("WL15620220427503");
            ss.Add("WL15620220428501");
            ss.Add("WL15620220428502");
            ss.Add("WL15620220429501");
            ss.Add("WL15620220429502");
            ss.Add("WL15620220429503");
            ss.Add("WL15620220505501");
            ss.Add("WL15620220505502");
            ss.Add("WL15620220505503");
            ss.Add("WL15620220505504");
            ss.Add("WL15620220505505");
            ss.Add("WL15620220506501");
            ss.Add("WL15620220506502");
            ss.Add("WL15620220506503");
            ss.Add("WL15620220510501");
            ss.Add("WL15620220510502");
            ss.Add("WL15620220510503");
            ss.Add("WL15620220511501");
            ss.Add("WL15620220511502");
            ss.Add("WL15620220513501");
            ss.Add("WL15620220513502");
            ss.Add("WL15620220513503");
            ss.Add("WL15620220513504");
            ss.Add("WL15620220516501");
            ss.Add("WL15620220516502");
            ss.Add("WL15620220517501");
            ss.Add("WL15620220517502");
            ss.Add("WL15620220517503");
            ss.Add("WL15620220518501");
            ss.Add("WL15620220518502");
            ss.Add("WL15620220518503");
            ss.Add("WL15620220519501");
            ss.Add("WL15620220519502");
            ss.Add("WL15620220520501");
            ss.Add("WL15620220520502");
            ss.Add("WL15620220520503");
            ss.Add("WL15620220523501");
            ss.Add("WL15620220523502");
            ss.Add("WL15620220524501");
            ss.Add("WL15620220524502");
            ss.Add("WL15620220524503");
            ss.Add("WL15620220525501");
            ss.Add("WL15620220525502");
            ss.Add("WL15620220525503");
            ss.Add("WL15620220526501");
            ss.Add("WL15620220527501");
            ss.Add("WL15620220527502");
            ss.Add("WL15620220527503");
            ss.Add("WL15620220530501");
            ss.Add("WL15620220530502");
            ss.Add("WL15620220531501");
            ss.Add("WL15620220531502");
            ss.Add("WL15620220531503");
            ss.Add("WL15620220601501");
            ss.Add("WL15620220601502");
            ss.Add("WL15620220601503");
            ss.Add("WL15620220602501");
            ss.Add("WL15620220602502");
            ss.Add("WL15620220602503");
            ss.Add("WL15620220606501");
            ss.Add("WL15620220606502");
            ss.Add("WL15620220607501");
            ss.Add("WL15620220607502");
            ss.Add("WL15620220608501");
            ss.Add("WL15620220608502");
            ss.Add("WL15620220609501");
            ss.Add("WL15620220609502");
            ss.Add("WL15620220609503");
            ss.Add("WL15620220610501");
            ss.Add("WL15620220610502");
            ss.Add("WL15620220610503");
            ss.Add("WL15620220613501");
            ss.Add("WL15620220613502");
            ss.Add("WL15620220613503");
            ss.Add("WL15620220614501");
            ss.Add("WL15620220614502");
            ss.Add("WL15620220615501");
            ss.Add("WL15620220615502");
            ss.Add("WL15620220615503");
            ss.Add("WL15620220616501");
            ss.Add("WL15620220616502");
            ss.Add("WL15620220617501");
            ss.Add("WL15620220617502");
            ss.Add("WL15620220620501");
            ss.Add("WL15620220620502");
            ss.Add("WL15620220620503");
            ss.Add("WL15620220621501");
            ss.Add("WL15620220621502");
            ss.Add("WL15620220621503");
            ss.Add("WL15620220622501");
            ss.Add("WL15620220622502");
            ss.Add("WL15620220622503");
            ss.Add("WL15620220623501");
            ss.Add("WL15620220623502");
            ss.Add("WL15620220624501");
            ss.Add("WL15620220624502");
            ss.Add("WL15620220627501");
            ss.Add("WL15620220627502");
            ss.Add("WL15620220628501");
            ss.Add("WL15620220628502");
            ss.Add("WL15620220628503");
            ss.Add("WL15620220629501");
            ss.Add("WL15620220629502");
            ss.Add("WL15620220629503");
            ss.Add("WL15620220630501");
            ss.Add("WL15620220630502");
            ss.Add("WL15620220630503");
            ss.Add("WL15620220704501");
            ss.Add("WL15620220704502");
            ss.Add("WL15620220705501");
            ss.Add("WL15620220705502");
            ss.Add("WL15620220706501");
            ss.Add("WL15620220706502");


            foreach (var v in ss)
            {
                Do(v);
            }
        
        }

        public void Do(string orderid) {

            var order = new Views.OrdersView()[orderid];
            var ermAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == order.AdminID);
            var ErmAdminID = ermAdmin?.ID ?? "";
            var Orders = new Views.Orders2View().Where(item => item.MainOrderID == order.MainOrderID
                                                                 && item.OrderStatus != OrderStatus.Canceled
                                                                 && item.OrderStatus != OrderStatus.Returned
                                                                 && item.Status == Status.Normal).ToList();
            var orderIds = Orders.Select(t => t.ID).ToList();
            var decheads = new Views.DecHeadsView().Where(item => orderIds.Contains(item.OrderID));
            //如果这个主订单下的所有的子订单都已经制单了，才自动上传对账单，委托书

            var agentProxy = new Needs.Ccs.Services.Views.MainOrderAgentProxiesView().Where(t => t.Order.ID == Orders.FirstOrDefault().ID).FirstOrDefault();
            //保存文件
            string afileName = DateTime.Now.Ticks + ".pdf";
            FileDirectory afileDic = new FileDirectory(afileName);
            afileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
            afileDic.CreateDataDirectory();
            if (agentProxy.Client.ClientType == ClientType.External)
            {
                AgentProxyToPdf model = new AgentProxyToPdf();
                model.Orders = new List<Needs.Ccs.Services.Models.Order>();
                model.Orders = Orders;
                var orderagent = Orders.FirstOrDefault();
                model.ID = orderagent.MainOrderID;
                model.Client = agentProxy.Client;
                model.PackNo = Orders.Sum(t => t.PackNo);
                model.WarpType = orderagent.WarpType;
                model.Currency = orderagent.Currency;
                model.SaveAs(afileDic.FilePath);
            }
            else
            {
                //itextsharp生成，超过10页
                AgentProxyToPdf model = new AgentProxyToPdf();
                model.Orders = new List<Needs.Ccs.Services.Models.Order>();
                model.Orders = Orders;
                var order1 = Orders.FirstOrDefault();
                model.ID = order1.MainOrderID;
                model.Client = agentProxy.Client;
                model.PackNo = Orders.Sum(t => t.PackNo);
                model.WarpType = order1.WarpType;
                model.Currency = order1.Currency;
                model.SaveAs(afileDic.FilePath);
            }


            //先删除之前上传的委托书
            var aorigFiles = order.MainOrderFiles.Where(f => f.FileType == FileType.AgentTrustInstrument && f.Status == Status.Normal);
            foreach (var aorigFile in aorigFiles)
            {
                Needs.Ccs.Services.Models.MainOrderFile orderBill = new Needs.Ccs.Services.Models.MainOrderFile();
                orderBill.ID = aorigFile.ID;
                orderBill.Abandon();
                new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, aorigFile.ID);
            }

            var dicAgent = new { CustomName = afileName, WsOrderID = order.MainOrderID, AdminID = ErmAdminID };

            var centerTypeAgent = Needs.Ccs.Services.Models.ApiModels.Files.FileType.AgentTrustInstrument;
            //本地文件上传到服务器
            var atempPath = afileDic.FilePath;
            var resultAgent = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(atempPath, centerTypeAgent, dicAgent);
            string[] AgentID = { resultAgent[0].FileID };
            new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, AgentID);

        }

    }
}
