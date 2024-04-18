using System.Linq;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class DeliveryOrder : OrderExtends
    {

        /// <summary>
        /// 初始化
        /// </summary>
        public DeliveryOrder()
        {
            //this.ExecutionStatus = ExcuteStatus.香港待出库;
            this.EnterSuccess += DeliveryOrderExtends_EnterSuccess;
        }

        /// <summary>
        /// 表头保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeliveryOrderExtends_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            //订单项保存
            InsertOrderItems();
            //生成出库通知
            var result = Extends.NoticeExtends.CgOutNotice(this);
            HandleOutNotice(result);
        }


        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (PvWsOrderReponsitory reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
            {
                #region 主键设置
                this.ID = this.ID ?? Layers.Data.PKeySigner.Pick(PKeyType.Order, this.EnterCode);
                this.OutWaybill.ID = this.OutWaybill.ID ?? Layers.Data.PKeySigner.Pick(PKeyType.Waybill);
                #endregion

                #region 保存运单
                Task task1 = new Task(() =>
                {
                    this.OutWaybill.OrderID = this.ID;
                    this.OutWaybill.Enter();
                });
                task1.Start();
                #endregion

                //订单主表数据持久化
                this.OrderEnter(reponsitory);
                OrderOutputEnter(reponsitory);
                this.StatusLogEnter();

                //货物特殊要求插入
                this.OrderRequirementEnter(reponsitory);

                Task.Run(() =>
                {
                    //订单文件
                    new CenterFilesView().Upload(this.OrderFiles.Select(item => new CenterFileDescription
                    {
                        CustomName = item.CustomName,
                        Url = item.Url,
                        AdminID = item.AdminID,
                        WsOrderID = this.ID,
                        Type = item.Type,
                    }).ToArray());

                    //货物特殊文件
                    new CenterFilesView().Upload(Requirements.Where(item => item.RequireFiles != null).Select(item => new CenterFileDescription
                    {
                        CustomName = item.RequireFiles.CustomName,
                        Url = item.RequireFiles.Url,
                        AdminID = item.RequireFiles.AdminID,
                        WsOrderID = item.ID,
                        Type = item.RequireFiles.Type,
                    }).ToArray());

                });

                //代收货款申请
                if(this.Receive != null)
                {
                    var item = new PvWsOrder.Services.Models.ApplicationItem()
                    {
                        OrderID = this.ID,
                        Amount = this.Receive.TotalPrice,
                    };
                    this.Receive.Items = new PvWsOrder.Services.Models.ApplicationItem[] { item };
                    this.Receive.Sumbit();
                }

                task1.Wait();//等待运单持久化结束
            }
            this.OnEnterSuccess();
        }
    }
}
