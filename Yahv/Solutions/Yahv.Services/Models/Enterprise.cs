using Layers.Data.Sqls;
using Layers.Linq;
using System.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 基本信息
    /// </summary>
    public class Enterprise : IUnique
    {
        public Enterprise()
        {
            this.Status = ApprovalStatus.Normal;
        }
        #region 属性
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID
        {
            get
            {
                return this.id ?? this.Name.MD5();

            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 管理编码
        /// </summary>
        /// <chenhan>保障局部唯一</chenhan>
        public string AdminCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 地域、地区（废弃）
        ///// </summary>
        public string District { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 企业法人
        /// </summary>

        public string Corporation { set; get; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { set; get; }
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string Uscc { set; get; }
        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
        #endregion

        #region  
        //添加付款人。获取该付款人（企业）的id.
        public bool IsExist()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                return repository.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>().Any(item => item.ID == this.ID);
            }
        }
        //新增付款人（企业）
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Enterprises>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Enterprises>(new
                    {
                        AdminCode = this.AdminCode,
                        //District = this.District,
                        Status = this.Status,
                        RegAddress = this.RegAddress,
                        Corporation = this.Corporation,
                        Uscc = this.Uscc,
                        Place = this.Place
                    }, item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvbCrm.Enterprises
                    {
                        ID = this.ID,
                        Name = this.Name,
                        AdminCode = this.AdminCode == null ? "" : this.AdminCode,
                        Status = (int)this.Status,
                       // District = this.District,
                        RegAddress = this.RegAddress,
                        Corporation = this.Corporation,
                        Uscc = this.Uscc,
                        Place = this.Place
                    });
                }
            }
        }
        #endregion

    }
}
