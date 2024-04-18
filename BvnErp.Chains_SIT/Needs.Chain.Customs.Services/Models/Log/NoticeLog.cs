using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class NoticeLog : IUnique
    {
        public string ID { get; set; }
        public string MainID { get; set; }
        public string Title { get; set; }
        public string Context { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 是否将消息设置为已读
        /// 默认为false
        /// 当处理消息时，设置为true，防止以下情况
        /// 报关归订单A，型号A为3C,发送通知，再归订单A，型号B也为3C时，如果没有这个字段，则会把消息置为已读
        /// </summary>
        public bool Readed { get; set; }
        public DateTime? ReadDate { get; set; }
        public List<string> AdminIDs { get; set; }
        public Enums.SendNoticeType NoticeType { get; set; }
        public NoticeSettings Settings { get; set; }
        public string OrderID { get; set; }

        public NoticeLog()
        {
            this.Readed = false;
            this.CreateDate = DateTime.Now;
            this.AdminIDs = new List<string>();
            this.Settings = NoticeSettings.Current;
        }

        private void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Logs_Notices>().Count(item => item.MainID == this.MainID && item.Type == (int)this.NoticeType);
                if (count == 0)
                {
                    foreach (var t in this.AdminIDs)
                    {
                        string sqlinsert = "insert into [dbo].[Logs_Notices] (Title,Context,CreateDate,Readed,AdminID,MainID,Type)" +
                             " values " +
                             "({0}, {1}, {2}, {3}, {4},{5},{6})";
                        object[] paras = new object[]
                        {
                            this.Title,
                            this.Context,
                            this.CreateDate,
                            this.Readed,
                            t,
                            this.MainID,
                            (int)this.NoticeType
                        };

                        reponsitory.Command(sqlinsert, paras);
                    }

                }
                else
                {
                    if (this.Readed)
                    {
                        this.ReadDate = DateTime.Now;
                        this.Readed = true;
                        string sqlupdate = "update [dbo].[Logs_Notices] set " +
                                     "Readed = 1," +
                                     "ReadDate = '" + this.ReadDate +
                                     "' where MainID = '" + this.MainID + "' and Type= " + (int)this.NoticeType;

                        reponsitory.Command(sqlupdate);
                    }
                }
            }
        }

        public void SendNotice()
        {
            this.Title = this.NoticeType.GetDescription() + "待处理";
            this.Context = this.MainID + this.Title + " 请尽快处理";

            switch (this.NoticeType)
            {
                //跟单审批
                case Enums.SendNoticeType.ClassifyDone:
                    if (this.AdminIDs.Count() > 0)
                    {
                        getAdminMap(this.AdminIDs.FirstOrDefault());
                    }
                    break;

                //上传了代理委托书，对账单，MainID是主订单号
                case Enums.SendNoticeType.AgentProxyUploaded:
                case Enums.SendNoticeType.OrderBillUploaded:
                    getMerchandiser(this.MainID + "-01");
                    break;

                //报关审批，产地变更，需要重新归类
                case Enums.SendNoticeType.DecNoticePending:
                case Enums.SendNoticeType.TaxError:
                case Enums.SendNoticeType.OriginChange:
                case Enums.SendNoticeType.ManufactureChange:
                case Enums.SendNoticeType.ModelChange:
                    getAdminMenuMap(Settings.DeclareMenu);
                    break;

                //跟单审批,报关归类时
                case Enums.SendNoticeType.CCC:
                case Enums.SendNoticeType.OriginCertificate:
                //超过垫款上限，在Order 挂起的地方触发
                case Enums.SendNoticeType.ExceedLimit:
                case Enums.SendNoticeType.ForbidRejected:
                    getMerchandiser(this.MainID);
                    break;

                //总部审批               
                case Enums.SendNoticeType.Forbid:
                    getAdminMenuMap(Settings.HQMenu);
                    break;

                case Enums.SendNoticeType.InvoicePending:
                    getAdminMenuMap(Settings.FinanceMenu);
                    break;

                //收款通知
                case Enums.SendNoticeType.ReceivingPending:
                //张庆永审批结束，通知谁付款
                case Enums.SendNoticeType.PayPayExchange:
                    getAdminMap(this.AdminIDs.FirstOrDefault());
                    break;

                case Enums.SendNoticeType.PayExchangeAudit:
                    getMerchandiser(this.OrderID);
                    break;

                case Enums.SendNoticeType.HQCCC:
                case Enums.SendNoticeType.PayExChangeApprove:
                    getAdminMenuMap(Settings.ManagerMenu);
                    break;

            }

            this.Enter();
        }

        public void UpdateNotice()
        {
            this.Enter();
        }

        private void getAdminMenuMap(string MenuID)
        {
            this.AdminIDs.Clear();
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var preproductsView = new Needs.Ccs.Services.Views.Origins.AdminMenuMapOrigin(reponsitory);
                var result = from c in preproductsView
                             where c.MenuID == MenuID
                             select c.AdminID;

                this.AdminIDs = result.ToList();
            }
        }

        private void getMerchandiser(string OrderID)
        {
            this.AdminIDs.Clear();
            var order = new Needs.Ccs.Services.Views.OrdersView()[OrderID];
            string originAdminID = order.Client.Merchandiser.ID;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var preproductsView = new Needs.Ccs.Services.Views.Origins.PvbErmOrigin(reponsitory);
                var result = from c in preproductsView
                             where c.OriginID == originAdminID
                             select c.ID;

                this.AdminIDs.Add(result.FirstOrDefault());
            }
        }

        private void getAdminMap(string originAdminID)
        {
            this.AdminIDs.Clear();
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var preproductsView = new Needs.Ccs.Services.Views.Origins.PvbErmOrigin(reponsitory);
                var result = from c in preproductsView
                             where c.OriginID == originAdminID
                             select c.ID;

                if (result.Count() > 0)
                {
                    this.AdminIDs.Add(result.FirstOrDefault());
                }
            }
        }
    }
}
