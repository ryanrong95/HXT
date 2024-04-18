using Needs.Ccs.Services.Hanlders;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public partial class AttachApproval
    {
        private Dictionary<Enums.ApprovalType, Enums.OrderControlType> dicApprovalType2OrderControlType = new Dictionary<Enums.ApprovalType, Enums.OrderControlType>
        {
            { Enums.ApprovalType.GenerateBillApproval, Enums.OrderControlType.GenerateBillApproval },
            { Enums.ApprovalType.DeleteModelApproval, Enums.OrderControlType.DeleteModelApproval },
            { Enums.ApprovalType.ChangeQuantityApproval, Enums.OrderControlType.ChangeQuantityApproval },
            { Enums.ApprovalType.SplitOrderApproval, Enums.OrderControlType.SplitOrderApproval },
        };

        private string Applicant { get; set; }

        private Enums.ApprovalType ApprovalType { get; set; }

        private string MainOrderID { get; set; }

        private string TinyOrderID { get; set; }

        private string OrderItemID { get; set; }

        private string EventInfo { get; set; }

        private string OrderControlStepID { get; set; }

        public string ReferenceInfo { get; set; }

        public bool IsUnApprovedConflictEvent { get; set; } = false;

        public string StrUnApprovedConflictEventTypes { get; set; } = string.Empty;

        /// <summary>
        /// 增加审批检查未审批事件
        /// </summary>
        public event AttachApprovalCheckUnApprovedHanlder CheckUnApprovedApply;

        /// <summary>
        /// 发起申请的初始化函数
        /// </summary>
        public AttachApproval(
            string applicant,
            Enums.ApprovalType approvalType,
            string mainOrderID,
            string tinyOrderID,
            string orderItemID,
            string eventInfo)
        {
            this.Applicant = applicant;
            this.ApprovalType = approvalType;
            this.MainOrderID = mainOrderID;
            this.TinyOrderID = tinyOrderID;
            this.OrderItemID = orderItemID;
            this.EventInfo = eventInfo;

            switch (this.ApprovalType)
            {
                case Enums.ApprovalType.GenerateBillApproval:
                    this.CheckUnApprovedApply += CheckUnApprovedForGenerateBillApproval; //如果有已经发起，但还未审批的申请，则自动撤销该申请
                    break;
                case Enums.ApprovalType.DeleteModelApproval:
                    this.CheckUnApprovedApply += CheckUnApprovedForDeleteModelApproval;
                    break;
                case Enums.ApprovalType.ChangeQuantityApproval:
                    this.CheckUnApprovedApply += CheckUnApprovedForChangeQuantityApproval;
                    break;
                case Enums.ApprovalType.SplitOrderApproval:
                    this.CheckUnApprovedApply += CheckUnApprovedForSplitOrderApproval;
                    break;
                default:
                    break;
            }

            DoCheckUnApprovedApply();
        }

        #region 执行检查未审批事件

        /// <summary>
        /// 执行检查未审批事件
        /// </summary>
        void DoCheckUnApprovedApply()
        {
            if (this.CheckUnApprovedApply != null)
            {
                this.CheckUnApprovedApply(this, new AttachApprovalCheckUnApprovedEventArgs(this));
            }
        }

        /// <summary>
        /// 为“重新生成对账单”检查未审批的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckUnApprovedForGenerateBillApproval(object sender, AttachApprovalCheckUnApprovedEventArgs e)
        {
            string mainOrderID = e.AttachApproval.MainOrderID;

            string referenceInfo = this.GetReferenceInfoHtmlForGenerateBill(mainOrderID);

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
                var orderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();

                var unApprovedIDs = (from orderControl in orderControls
                                     join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID
                                     where orderControl.MainOrderID == mainOrderID
                                        && orderControl.ControlType == (int)Enums.OrderControlType.GenerateBillApproval
                                        && orderControl.Status == (int)Enums.Status.Normal
                                        && orderControlStep.Step == (int)Enums.OrderControlStep.Headquarters
                                        && orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Auditing
                                        && orderControlStep.Status == (int)Enums.Status.Normal
                                     select new
                                     {
                                         OrderControlID = orderControl.ID,
                                         OrderControlStepID = orderControlStep.ID,
                                     }).ToList();

                if (unApprovedIDs != null && unApprovedIDs.Any())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                    {
                        ControlStatus = (int)Enums.OrderControlStatus.Cancel,
                        UpdateDate = DateTime.Now,
                        ReferenceInfo = referenceInfo,
                    }, item => unApprovedIDs.Select(t => t.OrderControlStepID).ToArray().Contains(item.ID));
                }
            }
        }

        /// <summary>
        /// 为“删除型号审批”检查未审批的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckUnApprovedForDeleteModelApproval(object sender, AttachApprovalCheckUnApprovedEventArgs e)
        {
            string tinyOrderID = e.AttachApproval.TinyOrderID;
            CheckUnApprovedConflictEvent(tinyOrderID);
        }

        /// <summary>
        /// 为“修改数量审批”检查未审批的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckUnApprovedForChangeQuantityApproval(object sender, AttachApprovalCheckUnApprovedEventArgs e)
        {
            string tinyOrderID = e.AttachApproval.TinyOrderID;
            CheckUnApprovedConflictEvent(tinyOrderID);
        }

        /// <summary>
        /// 为“拆分订单审批”检查未审批的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckUnApprovedForSplitOrderApproval(object sender, AttachApprovalCheckUnApprovedEventArgs e)
        {
            string tinyOrderID = e.AttachApproval.TinyOrderID;
            CheckUnApprovedConflictEvent(tinyOrderID);
        }

        /// <summary>
        /// 检查是否有未审批的冲突事件
        /// </summary>
        /// <param name="tinyOrderID"></param>
        private void CheckUnApprovedConflictEvent(string tinyOrderID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
                var orderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();


                Enums.OrderControlType[] conflictEventTypes =
                {
                    Enums.OrderControlType.DeleteModelApproval,
                    Enums.OrderControlType.ChangeQuantityApproval,
                    Enums.OrderControlType.SplitOrderApproval,
                };
                var conflictEventTypesInt = conflictEventTypes.Select(t => (int)t).ToArray();

                var unApproved = (from orderControl in orderControls
                                  join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID
                                  where orderControl.OrderID == tinyOrderID
                                     && conflictEventTypesInt.Contains(orderControl.ControlType)
                                     && orderControl.Status == (int)Enums.Status.Normal
                                     && orderControlStep.Step == (int)Enums.OrderControlStep.Headquarters
                                     && orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Auditing
                                     && orderControlStep.Status == (int)Enums.Status.Normal
                                  select new
                                  {
                                      OrderControlID = orderControl.ID,
                                      OrderControlStepID = orderControlStep.ID,
                                      ControlType = (Enums.OrderControlType)orderControl.ControlType,
                                  }).ToList();

                if (unApproved != null && unApproved.Any())
                {
                    this.IsUnApprovedConflictEvent = true;
                    List<Enums.ApprovalType> unAppeoveTypes = new List<Enums.ApprovalType>();
                    foreach (var item in unApproved)
                    {
                        unAppeoveTypes.Add(dicApprovalType2OrderControlType.FirstOrDefault(t => t.Value == item.ControlType).Key);
                    }

                    StringBuilder sbUnApprovedConflictEventTypes = new StringBuilder();
                    foreach (var item in unAppeoveTypes)
                    {
                        sbUnApprovedConflictEventTypes.Append(item.GetDescription() + ",");
                    }

                    this.StrUnApprovedConflictEventTypes = sbUnApprovedConflictEventTypes.ToString().Trim(',');
                }
                else
                {
                    this.IsUnApprovedConflictEvent = false;
                }
            }
        }

        #endregion

        /// <summary>
        /// 撤销人和审批人的初始化函数
        /// </summary>
        public AttachApproval(string orderControlStepID)
        {
            this.OrderControlStepID = orderControlStepID;
        }

        /// <summary>
        /// 产生未审批消息
        /// (记录下要执行的函数 OrderBill 类中的 GenerateBillAfterChangeRate)
        /// </summary>
        public void GenerateUnApprovalInfo()
        {
            string newOrderControlID = Guid.NewGuid().ToString("N");

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderControls
                {
                    ID = newOrderControlID,
                    OrderID = this.TinyOrderID,
                    OrderItemID = this.OrderItemID,
                    ControlType = (int)dicApprovalType2OrderControlType[this.ApprovalType],
                    Status = (int)Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    EventInfo = this.EventInfo,
                    Applicant = this.Applicant,
                    MainOrderID = this.MainOrderID,
                });

                Enums.OrderControlStep step = Enums.OrderControlStep.Headquarters;


                string newOrderControlStepID = string.Concat(newOrderControlID, step).MD5();
                this.OrderControlStepID = newOrderControlStepID;

                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderControlSteps
                {
                    ID = newOrderControlStepID,
                    OrderControlID = newOrderControlID,
                    Step = (int)step,
                    ControlStatus = (int)Enums.OrderControlStatus.Auditing,
                    Status = (int)Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                });
            }

            //记录日志 Begin
            var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(this.Applicant);
            this.Log(newOrderControlID, admin.RealName + "提交了该申请");
            //记录日志 End
        }

        /// <summary>
        /// 执行目标操作(比如重新生成对账单)
        /// “自动审批处”或“人工审批处”执行
        /// </summary>
        public void ExecuteTargetOperation()
        {
            //通过 this.OrderControlStepID 找到 this.ApprovalType, this.Applicant, this.EventInfo
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var orderControls = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
                var orderControlSteps = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();

                var oneInfo = (from orderControl in orderControls
                               join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID

                               where orderControl.Status == (int)Enums.Status.Normal
                                  && orderControlStep.Status == (int)Enums.Status.Normal
                                  && orderControlStep.ID == this.OrderControlStepID
                               select new
                               {
                                   OrderControlType = (Enums.OrderControlType)orderControl.ControlType,
                                   Applicant = orderControl.Applicant,
                                   EventInfo = orderControl.EventInfo,
                               }).FirstOrDefault();

                this.ApprovalType = dicApprovalType2OrderControlType.FirstOrDefault(t => t.Value == oneInfo.OrderControlType).Key;
                this.Applicant = oneInfo.Applicant;
                this.EventInfo = oneInfo.EventInfo;
            }

            switch (this.ApprovalType)
            {
                case Enums.ApprovalType.GenerateBillApproval:
                    var admin1 = Needs.Underly.FkoFactory<Admin>.Create(this.Applicant);

                    EventInfoGenerateBill eventInfoGenerateBill = JsonConvert.DeserializeObject<EventInfoGenerateBill>(this.EventInfo);
                    if (eventInfoGenerateBill != null && eventInfoGenerateBill.TinyOrderInfos != null && eventInfoGenerateBill.TinyOrderInfos.Any())
                    {
                        foreach (var tinyOrderInfo in eventInfoGenerateBill.TinyOrderInfos)
                        {
                            ReGenerateBill reGenerateBill = new ReGenerateBill()
                            {
                                OrderID = tinyOrderInfo.TinyOrderID,
                                CustomsExchangeRate = tinyOrderInfo.NewCustomsExchangeRate,
                                RealExchangeRate = tinyOrderInfo.NewRealExchangeRate,
                                OrderBillType = (Enums.OrderBillType)tinyOrderInfo.NewOrderBillTypeInt,
                                RealAgencyFee = tinyOrderInfo.NewAgencyFeeUnitPrice,
                            };

                            //执行方法 Begin 
                            
                            Assembly assembly = Assembly.Load("WebApp"); //dll名字
                            Type type = assembly.GetType("WebApp.Order.Bill.ExchangeRateEdit"); //类名
                            Object obj = Activator.CreateInstance(type); //实例化
                            MethodInfo method = type.GetMethod("DoGenerateBill"); //加载方法
                            method.Invoke(obj, new Object[] { reGenerateBill, admin1 }); //执行   
                            
                            //执行方法 End
                        }
                    }


                    break;
                case Enums.ApprovalType.DeleteModelApproval:
                    var admin2 = Needs.Underly.FkoFactory<Admin>.Create(this.Applicant);

                    EventInfoDeleteModel eventInfoDeleteModel = JsonConvert.DeserializeObject<EventInfoDeleteModel>(this.EventInfo);
                    if (eventInfoDeleteModel != null)
                    {
                        //执行方法 Begin

                        Assembly assembly = Assembly.Load("WebApp"); //dll名字
                        Type type = assembly.GetType("WebApp.Order.Detail2"); //类名
                        Object obj = Activator.CreateInstance(type); //实例化
                        MethodInfo method = type.GetMethod("DoDeleteModelAuto"); //加载方法
                        method.Invoke(obj, new Object[] { eventInfoDeleteModel.OrderItemID, eventInfoDeleteModel.TinyOrderID, admin2 }); //执行

                        //执行方法 End
                    }

                    break;
                case Enums.ApprovalType.ChangeQuantityApproval:
                    EventInfoChangeQuantity eventInfoChangeQuantity = JsonConvert.DeserializeObject<EventInfoChangeQuantity>(this.EventInfo);
                    if (eventInfoChangeQuantity != null)
                    {
                        //执行方法 Begin

                        Assembly assembly = Assembly.Load("WebApp"); //dll名字
                        Type type = assembly.GetType("WebApp.Order.Detail2"); //类名
                        Object obj = Activator.CreateInstance(type); //实例化
                        MethodInfo method = type.GetMethod("DoChangeQuantityAuto"); //加载方法
                        method.Invoke(obj, new Object[] { eventInfoChangeQuantity.OrderItemID, eventInfoChangeQuantity.TinyOrderID, (int)(eventInfoChangeQuantity.NewQuantity) }); //执行

                        //执行方法 End
                    }

                    break;
                case Enums.ApprovalType.SplitOrderApproval:
                    EventInfoSplitOrder eventInfoSplitOrder = JsonConvert.DeserializeObject<EventInfoSplitOrder>(this.EventInfo);
                    if (eventInfoSplitOrder != null)
                    {
                        //执行方法 Begin

                        Assembly assembly = Assembly.Load("WebApp"); //dll名字
                        Type type = assembly.GetType("WebApp.Order.Packing.Display"); //类名
                        Object obj = Activator.CreateInstance(type); //实例化
                        MethodInfo method = type.GetMethod("DoSplitOrderAuto"); //加载方法
                        method.Invoke(obj, new Object[] { eventInfoSplitOrder.TinyOrderID, eventInfoSplitOrder.Packs, this.Applicant }); //执行

                        //执行方法 End
                    }

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 撤销申请
        /// </summary>
        public void CancelApply(Needs.Ccs.Services.Models.Admin admin, string referenceInfo)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                {
                    ControlStatus = (int)Enums.OrderControlStatus.Cancel,
                    UpdateDate = DateTime.Now,
                    ReferenceInfo = referenceInfo,
                }, item => item.ID == this.OrderControlStepID);
            }

            //记录日志 Begin
            string orderControlID = string.Empty;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var orderControlStep = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>().Where(t => t.ID == this.OrderControlStepID).FirstOrDefault();
                if (orderControlStep != null)
                {
                    orderControlID = orderControlStep.OrderControlID;
                }
            }

            this.Log(orderControlID, admin.RealName + "撤销了该申请");
            //记录日志 End
        }

        /// <summary>
        /// 审批通过
        /// </summary>
        public void ApproveSuccess(Needs.Ccs.Services.Models.Admin admin, string referenceInfo, bool isAuto = false)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                {
                    ControlStatus = (int)Enums.OrderControlStatus.Approved,
                    AdminID = admin.ID,
                    UpdateDate = isAuto ? (DateTime.Now.AddMinutes(30)) : DateTime.Now,
                    ReferenceInfo = referenceInfo,
                }, item => item.ID == this.OrderControlStepID);
            }
            
            Task.Run(() =>
            {
                //记录日志 Begin
                string orderControlID = string.Empty;
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var orderControlStep = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>().Where(t => t.ID == this.OrderControlStepID).FirstOrDefault();
                    if (orderControlStep != null)
                    {
                        orderControlID = orderControlStep.OrderControlID;
                    }
                }

                this.Log(orderControlID, admin.RealName + "通过了该申请", isAuto);
                //记录日志 End
            });            
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        public void ApproveRefuse(Needs.Ccs.Services.Models.Admin admin, string referenceInfo, string reason)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(new
                {
                    ControlStatus = (int)Enums.OrderControlStatus.Rejected,
                    AdminID = admin.ID,
                    UpdateDate = DateTime.Now,
                    ApproveReason = reason,
                    ReferenceInfo = referenceInfo,
                }, item => item.ID == this.OrderControlStepID);
            }

            //记录日志 Begin
            string orderControlID = string.Empty;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var orderControlStep = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>().Where(t => t.ID == this.OrderControlStepID).FirstOrDefault();
                if (orderControlStep != null)
                {
                    orderControlID = orderControlStep.OrderControlID;
                }
            }

            this.Log(orderControlID, admin.RealName + "拒绝了该申请");
            //记录日志 End

        }

    }
}
