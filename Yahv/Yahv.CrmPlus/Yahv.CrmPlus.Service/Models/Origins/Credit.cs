using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace YaHv.CrmPlus.Services.Models.Origins
{
    /// <summary>
    /// 信用
    /// </summary>
    public class Credit : IUnique
    {
        public Credit()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
        }

        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 授信方
        /// </summary>

        public string MakerID { set; get; }
        /// <summary>
        /// 接收方
        /// </summary>
        public string TakerID { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public Enterprise Maker { internal set; get; }
        /// <summary>
        /// 申请人姓名
        /// </summary>
        public Enterprise Taker { internal set; get; }
        /// <summary>
        /// 信用类型：我方公司是授信方还是接收方
        /// </summary>
        public CreditType Type { set; get; }
        public string SiteUserID { set; get; }
        /// <summary>
        /// 结算类型
        /// </summary>
        public ClearType ClearType { set; get; }
        /// <summary>
        /// 几个月
        /// </summary>
        public int Months { set; get; }
        /// <summary>
        /// 以上月份后，多少天
        /// </summary>
        public int Days { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { internal set; get; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { internal set; get; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsAvailable { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        public event ErrorHanlder EnterError;
        public event ErrorHanlder Repeat;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    var exist = repository.ReadTable<Layers.Data.Sqls.PvdCrm.Credits>().Any(item => item.Type == (int)this.Type && item.MakerID == this.MakerID && item.TakerID == this.TakerID);
                    if (exist)
                    {
                        this.Repeat(this, new ErrorEventArgs());
                        return;
                    }
                    else
                    {
                        this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Credit);
                        repository.Insert(new Layers.Data.Sqls.PvdCrm.Credits
                        {
                            ID = this.ID,
                            MakerID = this.MakerID,
                            TakerID = this.TakerID,
                            ClearType = (int)this.ClearType,
                            Type = (int)this.Type,
                            Months = this.Months,
                            Days = this.Days,
                            CreateDate = this.CreateDate,
                            ModifyDate = this.ModifyDate,
                            Summary = this.Summary,
                            IsAvailable = true,
                        });
                    }
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvdCrm.Credits>(new {
                        ClearType = (int)this.ClearType,
                        Months = this.Months,
                        Days = this.Days,
                        ModifyDate = this.ModifyDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion
    }

}
