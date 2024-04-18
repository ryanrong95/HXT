using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;

namespace Yahv.PvWsOrder.Services.Models
{
    public class AdoptTmepStock:IUnique
    {
        public string ID { get; set; }
        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 库位号
        /// </summary>
        public string ShelveID { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 暂存日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 暂存处理状态
        /// </summary>
        public TempStorageStatus TempStatus { get; set; }
        public string ForOrderID { get; set; }
        public string Summary { get; set; }

        public void UpdateEnterCode()
        {
            using (PvWmsRepository reponsitory = new PvWmsRepository())
            {
                reponsitory.Update<Layers.Data.Sqls.PvWms.TWaybills>(new {
                    EnterCode = this.EnterCode,
                    ModifyDate = DateTime.Now,
                }, item => item.ID == this.ID);
            }
        }

        public void UpdateForOrderID(string AdminID)
        {
            using (PvWmsRepository reponsitory = new PvWmsRepository())
            {
                reponsitory.Update<Layers.Data.Sqls.PvWms.TWaybills>(new
                {
                    ForOrderID = this.ForOrderID,
                    Summary = this.Summary,
                    Status = (int)TempStorageStatus.Completed,
                    CompleteDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    
                }, item => item.ID == this.ID);

                //协助库房 记录暂存操作日志
                string realName = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().SingleOrDefault(item => item.ID == AdminID)?.RealName;
                reponsitory.Insert(new Layers.Data.Sqls.PvWms.Logs_Operator
                {
                    ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.Logs_Operator),
                    Type = "Update",//this.Type.ToString(),
                    Conduct = "处理暂存",
                    MainID = this.ID,
                    CreatorID = AdminID,
                    Content = $"{realName} 在{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} 处理暂存，TWaybillID:{this.ID} OrderID:{this.ForOrderID}",
                    CreateDate = DateTime.Now,
                });

            }
        }
    }
}
