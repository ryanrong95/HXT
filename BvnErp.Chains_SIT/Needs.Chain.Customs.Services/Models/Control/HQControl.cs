using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Hanlders;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 北京总部（HQ：headquarters）管控
    /// </summary>
    public sealed class HQControl : OrderControlBase
    {
        /// <summary>
        /// 订单管控项
        /// </summary>
        private OrderControlItems items;
        public override OrderControlItems Items
        {
            get
            {
                if (this.items == null)
                {
                    using (var view = new Views.HQControlItemsView())
                    {
                        var query = view.Where(item => item.OrderID == this.Order.ID && item.ControlType == this.ControlType);
                        this.Items = new OrderControlItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.items = new OrderControlItems(value, new Action<OrderControlItem>(delegate (OrderControlItem item)
                {
                    item.OrderID = this.Order.ID;
                    item.ControlType = this.ControlType;
                }));
            }
        }

        /// <summary>
        /// 审批通过时发生
        /// </summary>
        public event OrderControledHanlder Approved;

        /// <summary>
        /// 审批未通过时发生
        /// </summary>
        public event OrderControledHanlder Rejected;

        public HQControl()
        {
            this.Approved += Control_Approved;
            this.Rejected += Control_Rejected;
        }

        private void Control_Approved(object sender, OrderControledEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var item in e.OrderControl.Items)
                {
                    if (e.OrderControl.ControlType == Enums.OrderControlType.CCC)
                    {
#pragma warning disable
#if PvData
                        //调用中心数据的接口，删除该型号的Ccc管控信息
                        var pvdataApi = new PvDataApiSetting();
                        var url = ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.DeleteSysControl;
                        var result = Needs.Utils.Http.ApiHelper.Current.JPost<Needs.Underly.JMessage>(url, new
                        {
                            partNumber = item.OrderItem.Model,
                            type = Enums.ProductControlType.CCC.GetHashCode()
                        });
#else
                        //更新产品管控库
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ProductControls>(ctrl => ctrl.Model == item.OrderItem.Model &&
                                                                    (Enums.ProductControlType)ctrl.Type == Enums.ProductControlType.CCC);
#endif
#pragma warning restore

                    }
                    else if (e.OrderControl.ControlType == Enums.OrderControlType.Forbid)
                    {
                        var type = item.OrderItem.Category.Type;
                        //从归类信息中移除禁运
                        type = type & ~Enums.ItemCategoryType.Forbid;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(new
                        {
                            Type = (int)type
                        }, cat => cat.ID == item.OrderItem.Category.ID);

#pragma warning disable
#if PvData
                        //调用中心数据的接口，删除该型号的禁运管控信息
                        var pvdataApi = new PvDataApiSetting();
                        var url = ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.DeleteSysControl;
                        var result = Needs.Utils.Http.ApiHelper.Current.JPost<Needs.Underly.JMessage>(url, new
                        {
                            partNumber = item.OrderItem.Model,
                            type = Enums.ProductControlType.Forbid.GetHashCode()
                        });
#endif
#pragma warning restore
                    }
                }

                //公司单需要修改订单状态到待报关
                if (e.OrderControl.Order.Type != Enums.OrderType.Outside)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                    {
                        OrderStatus = (int)Enums.OrderStatus.QuoteConfirmed
                    }, o => o.ID == e.OrderControl.Order.ID);
                }
            }

            base.CancelOrderHangUp();
        }

        private void Control_Rejected(object sender, OrderControledEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var item in e.OrderControl.Items)
                {
                    if (e.OrderControl.ControlType == Enums.OrderControlType.CCC)
                    {
                        var type = item.OrderItem.Category.Type;
                        //在归类信息中添加3C认证
                        type = type | Enums.ItemCategoryType.CCC;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(new
                        {
                            Type = (int)type
                        }, cat => cat.ID == item.OrderItem.Category.ID);
                        //更新订单特殊类型
                        var orderVoyage = new Views.OrderVoyagesOriginView(reponsitory).FirstOrDefault(ov => ov.Order.ID == item.OrderID &&
                                                                                                             ov.Type == Enums.OrderSpecialType.CCC);
                        if (orderVoyage == null)
                        {
                            string orderVoyageID = string.Concat(item.OrderID, Enums.OrderSpecialType.CCC).MD5();
                            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderVoyages
                            {
                                ID = orderVoyageID,
                                OrderID = item.OrderID,
                                Type = (int)Enums.OrderSpecialType.CCC,
                                Status = (int)Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now
                            });
                        }

#pragma warning disable
#if PvData
                        //调用中心数据的接口，更新Ccc管控历史记录
                        var pvdataApi = new PvDataApiSetting();
                        var url = ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.UpdateCccControl;
                        var result = Needs.Utils.Http.ApiHelper.Current.JPost<Needs.Underly.JMessage>(url, new
                        {
                            partNumber = item.OrderItem.Model,
                            manufacturer = item.OrderItem.Manufacturer,
                            isCcc = true
                        });
                        //同步3C审批结果给代仓储
                        var cp = new ClassifyProduct() { ID = item.OrderItem.ID, IsCCC = true };
                        SyncManager.Current.CccControl.For(cp).DoSync();
#endif
#pragma warning restore
                    }

                    //新增跟单员审核步骤
                    var step = new OrderControlStep()
                    {
                        OrderControlID = item.ID,
                        Step = Enums.OrderControlStep.Merchandiser
                    };
                    reponsitory.Insert(step.ToLinq());
                }
            }
        }

        void OnApproved()
        {
            if (this != null && this.Approved != null)
            {
                this.Approved(this, new OrderControledEventArgs(this));
            }
        }

        void OnRejected()
        {
            if (this != null && this.Rejected != null)
            {
                this.Rejected(this, new OrderControledEventArgs(this));
            }
        }

        /// <summary>
        /// 总部审批通过，无需跟单员下一步审核，以下两种情况调用：
        /// 1.同意报关员，产品无需3C认证，审批通过
        /// 2.同意禁运产品报关，审批通过
        /// </summary>
        public void Approve()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var item in this.Items)
                {
                    //将当前管控步骤置为“通过”
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                    {
                        ControlStatus = Enums.OrderControlStatus.Approved,
                        AdminID = this.Admin.ID,
                        UpdateDate = DateTime.Now
                    }, controlStep => controlStep.OrderControlID == item.ID &&
                                    (Enums.OrderControlStep)controlStep.Step == Enums.OrderControlStep.Headquarters);
                }
            }

            this.OnApproved();
        }

        /// <summary>
        /// 总部审批未通过，需要跟单员下一步审核，以下两种情况调用：
        /// 1.不同意报关员，产品需要3C认证，审批未通过，需要跟单员提交3C认证文件
        /// 2.拒绝禁运产品报关，审批未通过，需要跟单员将订单退回
        /// </summary>
        public void Reject()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var item in this.Items)
                {
                    //将当前管控步骤置为“拒绝”
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                    {
                        ControlStatus = Enums.OrderControlStatus.Rejected,
                        AdminID = this.Admin.ID,
                        UpdateDate = DateTime.Now
                    }, controlStep => controlStep.OrderControlID == item.ID &&
                                    (Enums.OrderControlStep)controlStep.Step == Enums.OrderControlStep.Headquarters);
                }
            }

            this.OnRejected();
        }

        /// <summary>
        /// 总部审批可转第三方，需要跟单员下一步审核，以下两种情况调用：
        /// 1.不同意报关员，产品需要3C认证，审批未通过，需要跟单员提交3C认证文件
        /// 2.拒绝禁运产品报关，审批未通过，需要跟单员将订单退回
        /// </summary>
        public void Turn()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var item in this.Items)
                {
                    //将当前管控步骤置为“可转第三方”
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                    {
                        ControlStatus = Enums.OrderControlStatus.Turned,
                        AdminID = this.Admin.ID,
                        UpdateDate = DateTime.Now
                    }, controlStep => controlStep.OrderControlID == item.ID &&
                                    (Enums.OrderControlStep)controlStep.Step == Enums.OrderControlStep.Headquarters);
                }
            }

            this.OnRejected();
        }
    }
}
