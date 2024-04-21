using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using Yahv.Usually;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 项目明细
    /// </summary>
    public class ProjectProduct : Yahv.Linq.IUnique, IDataEntity
    {
        #region  属性
        public string ID { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>

        public string ProjectID { get; set; }

        public Project Project { get; set; }

        /// <summary>
        /// （实际下单）客户
        /// </summary>

        public string AssignClientID { get; set; }

        /// <summary>
        /// 标准型号ID
        /// </summary>
        public string SpnID { get; set; }

        public StandardPartNumber StandardPartNumber { get; set; }
        /// <summary>
        /// 单机用量
        /// </summary>
        public int UnitProduceQuantity { get; set; }
        /// <summary>
        /// 项目用量
        /// </summary>
        public int ProduceQuantity { get; set; }
        /// <summary>
        /// 预计成交量
        /// </summary>
        public int? ExpectQuantity { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 参考单价
        /// </summary>
        public decimal ExpectUnitPrice { set; get; }
        /// <summary>
        /// 报价单价
        /// </summary>
        public decimal? UnitPrice { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 项目状态
        /// </summary>
        public ProductStatus ProjectStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Yahv.Underly.AuditStatus Status { get; set; }

        public decimal expectTotal;

        /// <summary>
        /// 预计成交额
        /// </summary>
        public decimal ExpectTotal
        {

            get
            {
                if (expectTotal == 0)
                {
                    int qty = ExpectQuantity ?? 0;
                    this.expectTotal = ExpectUnitPrice * qty;

                }
                return this.expectTotal;

            }
            set
            {

                this.expectTotal = value;
            }


        }

        public string Reporter { get; set; }

        /// <summary>
        ///竞品
        /// </summary>
        public List<ProjectCompelete> lstProjectCompelete { get; set; }


        public bool IsApr { get; set; }
        #endregion

        public ProjectProduct()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.Status = AuditStatus.Waiting;
            this.ProjectStatus = ProductStatus.DO;
        }

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = new PvdCrmReponsitory())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    #region 添加型号
                    //添加
                    if (!reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.ProjectProducts>().Any(item => item.ProjectID == this.ProjectID && item.SpnID == this.SpnID))
                    {
                        this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.ProjectProduct);
                        reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.ProjectProducts()
                        {
                            ID = this.ID,
                            ProjectID = this.ProjectID,
                            AssignClientID = this.AssignClientID,
                            SpnID = this.SpnID,
                            UnitProduceQuantity = this.UnitProduceQuantity,
                            ProduceQuantity = this.ProduceQuantity,
                            ExpectQuantity = this.ExpectQuantity,
                            Currency = (int)this.Currency,
                            ExpectUnitPrice = this.ExpectUnitPrice,
                            UnitPrice = this.UnitPrice,
                            Summary = this.Summary,
                            CreateDate = this.CreateDate,
                            ModifyDate = this.ModifyDate,
                            ProjectStatus = (int)this.ProjectStatus,
                            Status = (int)this.Status
                        });
                    }
                    //修改
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.ProjectProducts>(new
                        {
                            ProjectID = this.ProjectID,
                            //AssignClientID = this.AssignClientID,
                            SpnID = this.SpnID,
                            UnitProduceQuantity = this.UnitProduceQuantity,
                            ProduceQuantity = this.ProduceQuantity,
                            ExpectQuantity = this.ExpectQuantity,
                            Currency = (int)this.Currency,
                            ExpectUnitPrice = this.ExpectUnitPrice,
                            UnitPrice = this.UnitPrice,
                            Summary = this.Summary,
                            ModifyDate = this.ModifyDate,
                            ProjectStatus = (int)this.ProjectStatus,
                            Status = (int)this.Status
                        }, item => item.ProjectID == this.ProjectID && item.SpnID == this.SpnID);
                    }
                    #endregion

                    #region   产品报备
                    //获取代理品牌的PM
                    var admin = Yahv.CrmPlus.Service.GetPmBySpnID.GetPm(this.SpnID);
                    if (admin.ID != null)
                    {
                        if (!reponsitory.GetTable<Layers.Data.Sqls.PvdCrm.ProjectReports>().Any(item => item.ProjectID == this.ProjectID && item.SpnID == this.SpnID))
                        {
                            reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.ProjectReports()
                            {
                                ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.ProjectReport),
                                ClientID = this.AssignClientID,
                                ProjectID = this.ProjectID,
                                SpnID = this.SpnID,
                                ReporterID = this.Reporter,
                                Status = (int)ReportStatus.Waiting,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                            });
                        }
                    }
                    #endregion

                    //if (this.lstProjectCompelete.Count > 0)
                    //{
                    //    reponsitory.Delete<Layers.Data.Sqls.PvdCrm.ProjectCompeletes>(item => item.ProjectProductID == this.ID && item.ProjectID == this.ProjectID);
                    //    reponsitory.Insert(this.lstProjectCompelete.Select(item => new Layers.Data.Sqls.PvdCrm.ProjectCompeletes
                    //    {
                    //        ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Compelete),
                    //        ProjectID = this.ProjectID,
                    //        ProjectProductID = this.ID,
                    //        SpnID = item.SpnID,
                    //        UnitPrice = item.UnitPrice,
                    //        CreatorID = item.CreatorID,
                    //        CreateDate = this.CreateDate,
                    //        ModifyDate = this.ModifyDate,
                    //        Status = (int)Yahv.Underly.DataStatus.Normal
                    //    }).ToArray());

                    //}
                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }




        public void Approve()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.ProjectProducts>(new
                {
                    this.Status,
                    this.Summary,
                }, item => item.ID == this.ID);
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        //public void Enable()
        //{
        //    using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
        //    {
        //        repository.Update<Layers.Data.Sqls.PvdCrm.Projects>(new
        //        {
        //            Status = DataStatus.Normal
        //        }, item => item.ID == this.ID);
        //        if (this != null && this.AbandonSuccess != null)
        //        {
        //            this.AbandonSuccess(this, new SuccessEventArgs(this));
        //        }
        //    }
        //}

        #endregion

        #region 事件
        public event SuccessHanlder EnterSuccess;



        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// EnterError
        /// </summary>

        public event ErrorHanlder EnterError;
        #endregion
    }


}
