using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 组织机构
    /// </summary>
    public class League : Yahv.Linq.IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;

        #endregion

        #region 属性
        string id;
        /// <summary>
        /// ID
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Join("", this.Category, this.FatherID, this.Name).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 分类（工作关系、询报价区域管理）
        /// </summary>
        public Category Category { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地区、公司、职位、部门 （人员均需要在职位下，职位可以在任意节点下）
        /// </summary>
        public LeagueType Type { get; set; }

        /// <summary>
        /// 正常、停用
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// 角色职务 Type== 职位  时 ，才能设置 职务角色
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// 内部公司ID
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public LeagueSubs Subs { get { return new LeagueSubs(this); } }
        #endregion

        #region 持久化
        /// <summary>
        /// 添加/修改
        /// </summary>
        /// <param name="entity">组织机构</param>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //查询是否包含该组织机构
                if (!repository.ReadTable<Leagues>().Any(item => item.ID == this.ID))
                {
                    this.Status = Status.Normal;

                    repository.Insert(new Leagues
                    {
                        ID = this.ID,
                        Name = this.Name,
                        FatherID = this.FatherID,
                        Type = (int)this.Type,
                        Status = (int)this.Status,
                        Category = (int)this.Category,
                        RoleID = this.RoleID,
                        EnterpriseID = this.EnterpriseID,
                    });
                }
                else
                {
                    repository.Update<Leagues>(new
                    {
                        //ID = this.ID,
                        //Name = this.Name,
                        //FatherID = this.FatherID,
                        Type = (int)this.Type,
                        Status = (int)this.Status,
                        RoleID = this.RoleID,
                        EnterpriseID = this.EnterpriseID,
                        //Category = (int)this.Category,
                    }, item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 假删除
        /// </summary>
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Leagues>(new
                {
                    Status = (int)Status.Delete
                }, item => item.ID == this.ID);
            }

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
        #endregion
    }
}